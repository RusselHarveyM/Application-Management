using Basecode.Data.Models;
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
    /// <param name="action">The action name.</param>
    /// <param name="id">The identifier to store.</param>
    /// <param name="expirationMinutes">The expiration minutes.</param>
    /// <returns></returns>
    public string GenerateToken(string action, int id, int expirationMinutes = 30)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim("action", action),
                new Claim("id", id.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
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
            var action = jwtToken.Claims.First(x => x.Type == "action").Value;

            return action == expectedAction;
        }
        catch
        {
            // Token validation failed.
            return false;
        }
    }

    /// <summary>
    /// Gets the claim value.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="claimType">Type of the claim.</param>
    /// <returns></returns>
    public string GetClaimValue(string token, string claimType)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        return jwtToken.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }

    /// <summary>
    /// Gets the identifier from token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="action">The action.</param>
    /// <returns></returns>
    public int GetIdFromToken(string token, string action)
    {
        int userScheduleId = 0;

        if (ValidateToken(token, action))
        {
            var idClaim = GetClaimValue(token, "id");
            if (int.TryParse(idClaim, out int id))
            {
                userScheduleId = id;
            }
        }

        return userScheduleId;
    }
}
