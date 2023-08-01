using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenHelper
{
    private readonly string _key;

    public TokenHelper(string key)
    {
        _key = key;
    }

    /// <summary>
    /// Generates the token.
    /// </summary>
    /// <param name="claims">The claims.</param>
    /// <param name="expirationHours">Number of hours before the token expires.</param>
    /// <returns></returns>
    public string GenerateToken(Dictionary<string, string> claims, int expirationHours = 72)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.Select(x => new Claim(x.Key, x.Value))),
            Expires = DateTime.UtcNow.AddHours(expirationHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Validates the token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="expectedAction">The expected action.</param>
    /// <returns></returns>
    public bool ValidateToken(string token, string expectedAction)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_key);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var action = jwtToken.Claims.FirstOrDefault(x => x.Type == "action")?.Value;

            return action == expectedAction;
        }
        catch
        {
            // Token validation failed.
            return false;
        }
    }

    /// <summary>
    /// Gets the token claims.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="action">The action.</param>
    /// <returns></returns>
    public Dictionary<string, string> GetTokenClaims(string token, string action)
    {
        var claims = new Dictionary<string, string>();

        if (ValidateToken(token, action))
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            foreach (var claim in jwtToken.Claims)
                claims.Add(claim.Type, claim.Value);
        }

        return claims;
    }
}
