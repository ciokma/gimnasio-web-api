using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using gimnasio_web_api.Repositories;
using gimnasio_web_api.DTOs;
using BCrypt.Net;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using gimnasio_web_api.Models;

namespace gimnasioNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAdministradorRepository _repository;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

           
        

        public AuthController(IAdministradorRepository repository, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _repository = repository;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] UserAdminLoginDto userAdmin)
        {
            // Obtener el administrador de la base de datos
            var administrador = await _repository.GetByUsernameAsync(userAdmin.Username);

            // Verificar si el administrador existe y si la contraseña proporcionada coincide con la almacenada
            if (administrador == null || !BCrypt.Net.BCrypt.Verify(userAdmin.Password, administrador.Clave))
            {
                return Unauthorized("Credenciales incorrectas");
            }

            // Generar el token JWT si la autenticación es exitosa
            var token = GenerateJwtToken(administrador);
            return Ok(new { token });
        }

        private string GenerateJwtToken(Administrador admin)
        {
            DateTime expiresAt = DateTime.UtcNow.AddMinutes(60);
            long expiresAtUnix = new DateTimeOffset(expiresAt).ToUnixTimeSeconds();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.Usuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                          new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                          ClaimValueTypes.Integer64),
                new Claim("id", admin.Id.ToString()),
                new Claim("email", admin.Email),
                new Claim("role", "Administrador"),
                new Claim(JwtRegisteredClaimNames.Exp, expiresAtUnix.ToString(), ClaimValueTypes.Integer64) // Opcional
            };
            
            var jsonWebTokenSecret = Environment.GetEnvironmentVariable("JsonWebTokenSecret") ?? "";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jsonWebTokenSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "gymsys.com",
                audience: "gymsys.com",
                claims: claims,
                expires: expiresAt, // Esto ya maneja la expiración
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
