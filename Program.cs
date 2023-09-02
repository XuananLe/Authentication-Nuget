using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
// builder.Services.AddScoped<AuthService>();
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie"); // You can add multiple authentication schemes, but you need to specify one as default
    ; // Add cookie authentication
builder.Logging.AddConsole();
var app = builder.Build();

// app.Use(async (context, next) =>
// {
//     var authCookie = context.Request.Headers["Cookie"].FirstOrDefault(x => x.Contains("usr"));
//     var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
//     logger.Log(LogLevel.Information, $"authCookie: {authCookie}");
//     if (authCookie == null)
//     {
//         context.Response.StatusCode = 401;
//         await context.Response.WriteAsync("Unauthorized");
//         return;
//     }
//
//     await next();
// });

app.UseAuthentication();



app.MapGet("/", () => "Hello World!");
app.MapGet("/username", (HttpContext ctx, IDataProtectionProvider idp) =>
{
    return "Hello world"; 
});
app.MapGet("/login", async (HttpContext ctx) =>
{
    var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, "John Doe"),
        new Claim(ClaimTypes.Email, "john@example.com"),
        new Claim(ClaimTypes.Role, "User")
    };
    foreach (var VARIABLE in claims)
    {
        logger.Log(LogLevel.Information, VARIABLE.ToString());
    }
    
    var identity = new ClaimsIdentity(claims, "cookie");
    var principal = new ClaimsPrincipal(identity);

    await ctx.SignInAsync("cookie", principal);
    return "Logged in successfully";
});

app.Run();

// public class AuthService
// {
//     private readonly IDataProtectionProvider _idp;
//     private readonly IHttpContextAccessor _httpContextAccessor;
//
//     public AuthService(IDataProtectionProvider idp, IHttpContextAccessor httpContextAccessor)
//     {
//         _idp = idp;
//         _httpContextAccessor = httpContextAccessor;
//     }
//
//     public AuthService(IDataProtectionProvider idp)
//     {
//         _idp = idp;
//     }
//
//     public AuthService()
//     {
//     }
//
//     public void SignIn()
//     {
//         _idp.CreateProtector("auth-cookie").Protect("usr:xuanan");
//         var protector = _idp.CreateProtector("auth-cookie");
//         _httpContextAccessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:xuanan")}";
//     }
// }