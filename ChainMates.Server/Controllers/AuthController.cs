using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ChainMates.Server;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ChainMates.Server.DTOs.Auth;

[ApiController]
[Route("chainmates/auth")]
public class AuthController : ControllerBase
{
    // DISCLOSURE: I used AI to generate a lot of the code in this file. I've since made sure I understand how it works --
    // ultimately this is a kind of toy-example of a project, so authentification was never a high priority, other than 
    // to give an indication of how it could be worked into the structure elsewhere.

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
        //Doesn't use displayname at all currently
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


        return Ok("Successfully Logged In");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
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


        // Then sign the user in immediately
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


    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return Ok("Done!");
    }
}
