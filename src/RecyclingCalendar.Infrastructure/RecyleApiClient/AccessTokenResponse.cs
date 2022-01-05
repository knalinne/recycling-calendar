namespace RecyclingCalendar.Infrastructure.RecyleApiClient;

public class AccessTokenResponse
{
    public DateTime ExpiresAt { get; set; } = DateTime.MinValue;
    public string AccessToken { get; set; } = null!;
}