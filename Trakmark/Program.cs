using Microsoft.AspNetCore.Components.Authorization;
using Trakmark.Components;
using Trakmark.Components.Account;
using Trakmark.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddScoped<
    AuthenticationStateProvider,
    IdentityRevalidatingAuthenticationStateProvider
>();

builder.Services.AddAppAuthentication(builder.Configuration);
builder.Services.AddAppDatabase(builder.Configuration, builder.Environment);
builder.Services.AddAppIdentity();
builder.Services.AddAppTelemetry(builder.Configuration);

var app = builder.Build();

await app.SeedRolesAsync();

app.UseAppMiddleware();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapAdditionalIdentityEndpoints();

app.Run();
