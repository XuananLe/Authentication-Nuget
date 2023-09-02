using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Logging.AddConsole();
var app = builder.Build();

app.Use(async (context, next) =>
{
    var authCookie = context.Request.Headers["Cookie"].FirstOrDefault(x => x.Contains("usr"));
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.Log(LogLevel.Information, $"authCookie: {authCookie}");
    if (authCookie == null)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized");
        return;
    }

    await next();
});



app.MapGet("/", () => "Hello World!");
app.MapGet("/username", (HttpContext ctx, IDataProtectionProvider idp) =>
{
    return "Hello world"; 
});
app.MapGet("/login", (AuthService auth) =>
{
    auth.SignIn();
    return "ok";
});

app.Run();

public class AuthService
{
    private readonly IDataProtectionProvider _idp;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IDataProtectionProvider idp, IHttpContextAccessor httpContextAccessor)
    {
        _idp = idp;
        _httpContextAccessor = httpContextAccessor;
    }

    public AuthService(IDataProtectionProvider idp)
    {
        _idp = idp;
    }

    public AuthService()
    {
    }

    public void SignIn()
    {
        _idp.CreateProtector("auth-cookie").Protect("usr:xuanan");
        var protector = _idp.CreateProtector("auth-cookie");
        _httpContextAccessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:xuanan")}";
    }
}