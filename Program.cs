using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
const string AuthScheme = "cookie";


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
// builder.Services.AddScoped<AuthService>();
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie"); // You can add multiple authentication schemes, but you need to specify one as default
    ; // Add cookie authentication
builder.Services.AddAuthorization(builder =>
{
    builder.AddPolicy("eur", policy =>
    {
        policy.RequireAuthenticatedUser().AddAuthenticationSchemes(AuthScheme) // the name of the authentication scheme
            .RequireClaim("passport_type", "eur");
    });
});
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
app.UseAuthorization();


app.MapGet("/", () => "Hello World!");
app.MapGet("/username", () =>
{
    return "Hello world"; 
});

app.MapGet("/unsecure", (HttpContext ctx) =>
{
    return ctx.User.FindFirst("usr")?.Value ?? "empty";
});

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(AuthScheme);
    return "Sign out successfully";
});


app.MapGet("/",(HttpContext context) =>
{
}).RequireAuthorization("eur");

app.MapGet("/login", async (HttpContext ctx) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "xuanan"));
    claims.Add(new Claim("passport_type", "eur" ));
    var identity = new ClaimsIdentity(claims, AuthScheme);
    var principal = new ClaimsPrincipal(identity);
    await ctx.SignInAsync(AuthScheme, principal);
    return "Sign in successfully";
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