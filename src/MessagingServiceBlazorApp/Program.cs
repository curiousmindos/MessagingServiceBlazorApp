using MessagingServiceBlazorApp.Components;
using MessagingServiceBlazorApp.Configurations;
using MessagingServiceBlazorApp.Features.Messaging.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
.AddInteractiveServerComponents();
builder.Services.AddBlazorBootstrap();

var configuration = builder.Configuration;
builder.Services.Configure<MessagingSettings>(configuration.GetSection(nameof(MessagingSettings)));

builder.Services.AddScoped<IMessagingBrokerClient, MessagingBrokerClient>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
