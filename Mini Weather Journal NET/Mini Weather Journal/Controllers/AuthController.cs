using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Mini_Weather_Journal.DTO;

namespace Mini_Weather_Journal.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        // 1. Validate user credentials (hardcoded for demo)
        if (dto.Username != "admin" || dto.Password != "admin123")
            return Unauthorized();

        // 2. Create claims
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, dto.Username),
            new Claim(ClaimTypes.Name, dto.Username)
        };

        // 3. Create signing key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 4. Create JWT token
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        // 5. Return token
        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}