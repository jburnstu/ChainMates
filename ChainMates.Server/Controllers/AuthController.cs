using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ReactApp1.Server;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ReactApp1.Server.DTOs.Auth;

[ApiController]
[Route("chainmates/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AppDbContext context, ILogger<AuthController> logger )
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var author = await _context.Author
            .FirstOrDefaultAsync(a => a.EmailAddress == dto.EmailAddress);

        if (author == null || author.Password != dto.Password)
            return Unauthorized();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, author.Id.ToString()),
            new Claim(ClaimTypes.Name, author.EmailAddress)
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("Cookies", principal);

        // Log Set-Cookie header if present to help debug client cookie issues
        if (HttpContext.Response.Headers.TryGetValue("Set-Cookie", out var sc))
        {
            _logger.LogInformation("Set-Cookie after login: {cookie}", sc.ToString());
            Console.WriteLine("Set-Cookie after login: " + sc.ToString());
        }

        return Ok("Successfully Logged In");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        Console.WriteLine("INSIDE REGISTER");
        Debug.WriteLine("INSIDE REGISTER");
        _logger.LogInformation("INSIDE REGISTER");
        // Check if user exists
        var existing = await _context.Author
            .FirstOrDefaultAsync(a => a.EmailAddress == dto.EmailAddress);

        if (existing != null)
            return BadRequest("User already exists");

        var author = new Author
        {
            EmailAddress = dto.EmailAddress,
            Password = dto.Password, 
            DisplayName = dto.DisplayName
        };

        _context.Author.Add(author);
        await _context.SaveChangesAsync();

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, author.Id.ToString()),
        new Claim(ClaimTypes.Name, author.EmailAddress)
    };

        var identity = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("Cookies", principal);

        return Ok("Successfully Registered");
    }
}
