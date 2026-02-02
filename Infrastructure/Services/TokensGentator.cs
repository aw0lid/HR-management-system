using Domain.Entites;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Application.Write.Contracts;

namespace Infrastructure.Services
{
    public class TokensGentator : ITokenService
    {
        public readonly IConfiguration _config;


        public TokensGentator (IConfiguration configuration)
        {
            _config = configuration;
        }

        public string GenerateJwtToken(User user)
        {
            var handler = new JsonWebTokenHandler();
            handler.MapInboundClaims = false;
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
                Claims = new Dictionary<string, object>
                {
                    [System.Security.Claims.ClaimTypes.NameIdentifier] = user.UserId.ToString(),
                    [System.Security.Claims.ClaimTypes.Name] = user.UserName,
                    [System.Security.Claims.ClaimTypes.Role] = user.Role.ToString(),
                    ["empId"] = user.EmployeeId.ToString(),
                    ["status"] = user.status.ToString()
                }
            };

            return handler.CreateToken(tokenDescriptor);
        }

        public (string PlainToken, string HashedToken) CreateSecureToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            
            string plainToken = Convert.ToHexString(randomNumber);
            string hashedToken = HashToken(plainToken);

            return (plainToken, hashedToken);
        }

        public string HashToken(string plainToken)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainToken));
            return Convert.ToHexString(hashBytes); 
        }
    }
} 