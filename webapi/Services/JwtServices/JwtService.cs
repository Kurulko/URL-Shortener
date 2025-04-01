using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using URL_ShortenerAPI.Data.Auth;
using URL_ShortenerAPI.Data.Models;
using URL_ShortenerAPI.Data.Settings;
using URL_ShortenerAPI.Repositories;

namespace URL_ShortenerAPI.Services.JwtServices;

public class JwtService : IJwtService
{
    readonly JwtSettings jwtSettings;
    readonly UserRepository userRepository;
    public JwtService(JwtSettings jwtSettings, UserRepository userRepository)
    {
        this.jwtSettings = jwtSettings;
        this.userRepository = userRepository;
    }

    public virtual async Task<TokenModel> GenerateJwtTokenAsync(User user)
    {
        JwtSecurityTokenHandler tokenHandler = new();

        var roles = await userRepository.GetUserRolesAsync(user.Id);

        var rolesClaims = GetRolesClaims(user, roles);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(rolesClaims),
            Expires = DateTime.UtcNow.AddDays(jwtSettings.ExpirationDays),
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
            SigningCredentials = GetSigningCredentials()
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        var tokenModel = new TokenModel()
        {
            TokenStr = tokenHandler.WriteToken(securityToken),
            ExpirationDays = jwtSettings.ExpirationDays,
            Roles = roles.ToArray()
        };

        return tokenModel;
    }

    SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    List<Claim> GetRolesClaims(User user, IEnumerable<string> roles)
    {
        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        foreach (string role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        return claims;
    }
}
