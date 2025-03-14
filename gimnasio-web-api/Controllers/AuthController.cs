using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gimnasio_web_api.Data;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using gimnasio_web_api.DTOs;
using System.Configuration;

namespace gimnasioNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        public AuthController(AppDbContext context, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] UserAdminLoginDto userAdmin)
        {
            if (userAdmin.Username == "admin" && userAdmin.Password == "password")
            {
                var token = GenerateJwtToken(userAdmin.Username);
                return Ok(new { token });
            }
            return Unauthorized();
        }

        private string GenerateJwtToken(string username)
        {
            long iat = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, iat.ToString())
            };
            var jsonWebTokenSecret = _configuration["JsonWebTokenSecret"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jsonWebTokenSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "gymsys.com",
                audience: "gymsys.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}