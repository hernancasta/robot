using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RobotHMI.Data;
using Shared.Command;
using Shared.Command.Preset;
using Shared.Messaging;
using Shared.Redis;
using Shared.Redis.Messaging;
using Shared.Redis.Streaming;
using Shared.Serialization;
using Shared.Streaming;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddStreaming();
builder.Services.AddRedisStreaming();
builder.Services.AddMessaging();
builder.Services.AddRedisMessaging();
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddSerialization();
builder.Services.AddSingleton<RobotClientService>();

builder.Services.AddHostedService(serviceCollection => serviceCollection.GetRequiredService<RobotClientService>());
builder.Services.AddSingleton<ICommandHandler<PresetCommand>, CommandHandler<PresetCommand>>(); //Service to send commands to motor drive.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
