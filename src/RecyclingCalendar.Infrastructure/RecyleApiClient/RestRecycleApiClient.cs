using System.Text.Json;
using Microsoft.Extensions.Options;
using RecyclingCalendar.Core.DTO;
using RecyclingCalendar.Core.Interfaces;
using RecyclingCalendar.Infrastructure.Settings;

namespace RecyclingCalendar.Infrastructure.RecyleApiClient;

public class RestRecyclingApiClient : IRecyclingApiClient
{
    private static Semaphore _semaphore = new Semaphore(1, 1);

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private const string ZipCodeUrl = "https://recycleapp.be/api/app/v1/zipcodes";
    private const string StreetUrl = "https://recycleapp.be/api/app/v1/streets";
    private const string RecyclingEventUrl = "https://recycleapp.be/api/app/v1/collections";
    private const string AuthenticateUrl = "https://recycleapp.be/api/app/v1/access-token";

    private readonly HttpClient _httpClient;
    private readonly RecyclingApiSettings _settings;

    private JwtTokenWrapper? _jwtToken;

    public RestRecyclingApiClient(IOptions<RecyclingApiSettings> settings)
    {
        _settings = settings.Value;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
        _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        _httpClient.DefaultRequestHeaders.Add("Referer", "https://recycleapp.be/calendar");
        _httpClient.DefaultRequestHeaders.Host = "recycleapp.be";
        _httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:95.0) Gecko/20100101 Firefox/95.0");
        _httpClient.DefaultRequestHeaders.Add("x-consumer", _settings.Consumer);
    }

    public async Task<IList<ZipCode>> FindAllZipCodes()
    {
        return await FindZipCodesByCode(null);
    }

    public async Task<IList<ZipCode>> FindZipCodesByCode(string? zipCode)
    {
        var baseQuery = zipCode switch
        {
            null => $"{ZipCodeUrl}?size=200",
            _ => $"{ZipCodeUrl}?q={zipCode}&size=200"
        };
        var result = new List<ZipCode>();
        var firstFetch =
            JsonSerializer.Deserialize<PaginatedResponse<ZipCodeResponseItem>>(await Fetch($"{baseQuery}"),
                SerializerOptions) ??
            throw new InvalidOperationException();
        result.AddRange(firstFetch.Items.Select(item => new ZipCode(item.Code, item.Id)));
        for (var i = 2; i <= firstFetch.Pages; i++)
        {
            var fetchResult =
                JsonSerializer.Deserialize<PaginatedResponse<ZipCodeResponseItem>>(await Fetch($"{baseQuery}&page={i}"),
                    SerializerOptions) ??
                throw new InvalidOperationException();
            result.AddRange(fetchResult.Items.Select(item => new ZipCode(item.Code, item.Id)));
        }

        return result;
    }

    public async Task<IList<Street>> FindAllStreets(string zipCodeId)
    {
        return await FindStreetsByName(zipCodeId, null);
    }

    public async Task<IList<Street>> FindStreetsByName(string zipCodeId, string? name)
    {
        var baseQuery = name switch
        {
            null => $"{StreetUrl}?zipcodes={zipCodeId}&size=200",
            _ => $"{StreetUrl}?q={name}&zipcodes={zipCodeId}&size=200"
        };
        var result = new List<Street>();
        var firstFetch =
            JsonSerializer.Deserialize<PaginatedResponse<StreetResponseItem>>(await Fetch($"{baseQuery}"),
                SerializerOptions) ??
            throw new InvalidOperationException();
        result.AddRange(firstFetch.Items.Select(item => new Street(item.Name, item.Id)));
        for (var i = 2; i <= firstFetch.Pages; i++)
        {
            var fetchResult =
                JsonSerializer.Deserialize<PaginatedResponse<StreetResponseItem>>(await Fetch($"{baseQuery}&page={i}"),
                    SerializerOptions) ??
                throw new InvalidOperationException();
            result.AddRange(fetchResult.Items.Select(item => new Street(item.Name, item.Id)));
        }

        return result;
    }

    public async Task<IList<RecyclingEvent>> FindRecyclingEventsBy(string zipCodeId, string streetId, int houseNumber)
    {
        var currentYear = DateTime.Now.Year;
        var baseQuery =
            $"{RecyclingEventUrl}?zipcodeId={zipCodeId}&streetId={streetId}&houseNumber={houseNumber}&fromDate={new DateOnly(currentYear, 1, 1).ToString("yyyy-MM-dd")}&untilDate={new DateOnly(currentYear, 12, 31).ToString("yyyy-MM-dd")}&size=200";
        var recyclingEventItems = new List<RecyclingEventResponseItem>();
        var firstFetch =
            JsonSerializer.Deserialize<PaginatedResponse<RecyclingEventResponseItem>>(await Fetch($"{baseQuery}"),
                SerializerOptions) ??
            throw new InvalidOperationException();
        recyclingEventItems.AddRange(firstFetch.Items);
        for (var i = 2; i <= firstFetch.Pages; i++)
        {
            var fetchResult =
                JsonSerializer.Deserialize<PaginatedResponse<RecyclingEventResponseItem>>(await Fetch($"{baseQuery}&page={i}"),
                    SerializerOptions) ??
                throw new InvalidOperationException();
            recyclingEventItems.AddRange(fetchResult.Items);
        }

        return BuildRecyclingEvents(recyclingEventItems);
    }

    private IList<RecyclingEvent> BuildRecyclingEvents(List<RecyclingEventResponseItem> recyclingEventItems)
    {
        var result = new Dictionary<DateOnly, RecyclingEvent>();
        recyclingEventItems.ForEach(e =>
        {
            var eventDate = DateOnly.FromDateTime(e.Timestamp);
            if (!result.ContainsKey(eventDate))
            {
                result.Add(eventDate, new RecyclingEvent(eventDate));
            }

            var recyclingEvent = result[eventDate];
            recyclingEvent.Summary += $" {e.Fraction.Name.Fr}";
            recyclingEvent.Description += $"\n{e.Fraction.Name.Fr}";
        });
        return result.Values.ToList();
    }

    private async Task<Stream> Fetch(string uri)
    {
        await EnsureAuthenticate();
        _httpClient.DefaultRequestHeaders.Remove("x-correlation-id");
        _httpClient.DefaultRequestHeaders.Add("x-correlation-id", Guid.NewGuid().ToString());
        return await _httpClient.GetStreamAsync(uri);
    }

    private async Task EnsureAuthenticate()
    {
        _semaphore.WaitOne();
        if (!(_jwtToken?.IsValid() ?? false))
        {
            await Authenticate();
        }

        _semaphore.Release();
    }

    private async Task Authenticate()
    {
        var authenticationClient = new HttpClient();
        authenticationClient.DefaultRequestHeaders.Accept.Clear();
        authenticationClient.DefaultRequestHeaders.Add("Accept", "application/json");
        authenticationClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        authenticationClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
        authenticationClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        authenticationClient.DefaultRequestHeaders.Add("Referer", "https://recycleapp.be/calendar");
        authenticationClient.DefaultRequestHeaders.Host = "recycleapp.be";
        authenticationClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:95.0) Gecko/20100101 Firefox/95.0");
        authenticationClient.DefaultRequestHeaders.Add("x-consumer", _settings.Consumer);
        authenticationClient.DefaultRequestHeaders.Add("x-secret", _settings.Secret);

        var stream = authenticationClient.GetStreamAsync(AuthenticateUrl);
        var accessToken = await JsonSerializer.DeserializeAsync<AccessTokenResponse>(await stream, SerializerOptions) ??
                          throw new InvalidOperationException();

        _jwtToken = new JwtTokenWrapper(accessToken.AccessToken);
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        _httpClient.DefaultRequestHeaders.Add("Authorization", accessToken.AccessToken);
    }
}