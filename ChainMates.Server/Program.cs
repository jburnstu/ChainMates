using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ChainMates.Server;
using ChainMates.Server.Services;
using System.Diagnostics;
using ChainMates.Server.Services.Interfaces;
using ChainMates.Server.Rules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<AuthorService>();
builder.Services.AddScoped<StoryService>();
builder.Services.AddScoped<ISegmentService, SegmentService>();
builder.Services.AddScoped<ISegmentRules, SegmentRules>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<InitialLoadService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserService>();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.Cookie.HttpOnly = true;
        // Allow cross-site cookie for dev proxy (Vite). Requires Secure.
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

try
{
    var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();


// Use configured URLs from environment/launchSettings. This lets the dev proxy
// (and tools that set ASPNETCORE_HTTPS_PORT/ASPNETCORE_URLS) control the binding.
app.Run();


}
catch (Exception e)
{
    Debug.WriteLine(e.ToString());
    throw;
}