using FightIpsum.Components;
using FightIpsum.Endpoints.FightIpsum;
using FightIpsum.Endpoints.FightIpsum.Services;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#if DEBUG
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("API", httpcontext => 
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpcontext.Connection.RemoteIpAddress?.ToString(),
            factory : _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1)
            })
    );
});
#else
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("API", httpcontext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpcontext.Request.Headers["X-Forwarded-For"].ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1)
            })
    );
});
#endif

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFluentUIComponents();

builder.Services.AddScoped<IFightIpsumService, FightIpsumService>();

var app = builder.Build();

app.UseRateLimiter();
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapGroup("/api/fightipsum")
    .MapFightIpsumEndpoint();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

