using System.IdentityModel.Tokens.Jwt;

namespace RecyclingCalendar.Infrastructure.RecyleApiClient;

public class JwtTokenWrapper
{
    private static readonly TimeSpan TokenValidationTimeReducer = TimeSpan.FromMinutes(5);
    public string JwtToken { get; }
    public JwtSecurityToken DecodedToken{ get; }

    public JwtTokenWrapper(string jwtToken)
    {
        this.JwtToken = jwtToken;
        var handler = new JwtSecurityTokenHandler();
        DecodedToken = handler.ReadJwtToken(jwtToken);
    }

    public bool IsValid()
    {
        return DecodedToken.ValidTo.ToLocalTime() > DateTime.Now.Subtract(TokenValidationTimeReducer);
    }
}