using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDataProtection();
builder.Logging.AddConsole();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/username", (HttpContext ctx, IDataProtectionProvider idp) =>
{
    
    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.Contains("usr"));
    if (authCookie == null)
    {
        ctx.Response.StatusCode = 401;
        return "Unauthorized";
    }

    return authCookie;
});
app.MapGet("/login", (HttpContext ctx, IDataProtectionProvider idp) =>
{
    var protector = idp.CreateProtector("auth-cookie");
    ctx.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:xuanan")}";
    return "ok";
});



app.Run();