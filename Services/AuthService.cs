using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SimpleMarketplace.Api.Entities;
using Google.Apis.Auth;
using System.Net.Http;
using System.Net.Http.Json;

namespace SimpleMarketplace.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
        
        public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string token)
        {
            try
            {
                // 1. Intentar validar como ID Token (JWT)
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _config["Google:ClientId"] }
                };

                return await GoogleJsonWebSignature.ValidateAsync(token, settings);
            }
            catch (Exception)
            {
                // 2. Si falla (probablemente es un Access Token), intentar obtener info desde Google UserInfo API
                try
                {
                    using var client = new HttpClient();
                    var response = await client.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={token}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadFromJsonAsync<GoogleUserInfoResponse>();
                        if (content != null)
                        {
                            return new GoogleJsonWebSignature.Payload
                            {
                                Subject = content.Sub,
                                Email = content.Email,
                                GivenName = content.Given_name,
                                FamilyName = content.Family_name,
                                Picture = content.Picture,
                                EmailVerified = content.Email_verified
                            };
                        }
                    }
                }
                catch { /* ignored */ }
                
                return null;
            }
        }

        // Clase auxiliar para mapear la respuesta de la API de Google UserInfo
        private class GoogleUserInfoResponse
        {
            public string Sub { get; set; } = "";
            public string Email { get; set; } = "";
            public string Given_name { get; set; } = "";
            public string Family_name { get; set; } = "";
            public string Picture { get; set; } = "";
            public bool Email_verified { get; set; }
        }

        public string GenerateJwtToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim(ClaimTypes.Role, "user"),
                new Claim("provider", usuario.Provider ?? "local")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateAdminJwtToken(Administrador admin)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.Name, $"{admin.Nombre} {admin.Apellido}"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim("nivelAcceso", admin.NivelAcceso ?? "basico")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
