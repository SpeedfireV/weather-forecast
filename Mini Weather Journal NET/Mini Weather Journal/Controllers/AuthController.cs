using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mini_Weather_Journal.DTO;
using Mini_Weather_Journal.Models;

namespace Mini_Weather_Journal.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly DatabaseContext _dbContext;

    public AuthController(IConfiguration config, DatabaseContext dbContext)
    {
        _config = config;
        _dbContext = dbContext;
    }

    private string JwtToken(User user)
    {
        
        // 2. Create claims
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
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
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email already in use.");

        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            Email = dto.Email,
            Username = dto.Username
        };

        user.PasswordHash = hasher.HashPassword(user, dto.Password);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return Ok(new { token = JwtToken(user) });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var attr = new EmailAddressAttribute();
            User user;
            if (attr.IsValid(dto.Login))
            {
                user = await _dbContext.Users.FirstAsync(u => u.Email == dto.Login);
            }
            else
            {
                user = await _dbContext.Users.FirstAsync(u => u.Username == dto.Login);
            }
            var passwordHasher = new PasswordHasher<User>();
            string storedHash = user.PasswordHash;
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid credentials.");
            }
            return Ok(new { token = JwtToken(user) });
        }
        catch (InvalidOperationException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}