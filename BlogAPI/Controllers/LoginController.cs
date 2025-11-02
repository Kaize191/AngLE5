using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogDataLibrary.Data;
using BlogDataLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly SqlData _db;
    private readonly IConfiguration _config;

    public LoginController(SqlData db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin credentials)
    {
        if (credentials is null ||
            string.IsNullOrWhiteSpace(credentials.Username) ||
            string.IsNullOrWhiteSpace(credentials.Password))
        {
            return BadRequest(new { message = "The credentials field is required." });
        }

        var user = _db.Authenticate(credentials.Username, credentials.Password);
        if (user is null)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        var token = GenerateToken(user);

        return Ok(new
        {
            token,
            user = new
            {
                id = user.Id,
                userName = user.UserName,
                firstName = user.FirstName,
                lastName = user.LastName
            }
        });
    }

   
    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register([FromBody] UserModel user)
    {
        if (user is null ||
            string.IsNullOrWhiteSpace(user.UserName) ||
            string.IsNullOrWhiteSpace(user.Password) ||
            string.IsNullOrWhiteSpace(user.FirstName) ||
            string.IsNullOrWhiteSpace(user.LastName))
        {
            return BadRequest(new { message = "All fields are required." });
        }

        _db.RegisterUser(user.UserName, user.FirstName, user.LastName, user.Password);
        return Ok(new { message = "Registration successful!" });
    }

 
    private string GenerateToken(UserModel user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var jwt = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
