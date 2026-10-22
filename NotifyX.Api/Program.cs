// using Microsoft.AspNetCore.SignalR;
using NotifyX.Api;
using NotifyX.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Add dependencies to the container.
builder.Services.AddConfiguration(builder.Configuration);

// builder.Services.AddSingleton<IUserIdProvider, QueryStringUserIdProvider>();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("NotificationTest");

app.UseHttpsRedirection();
app.MapControllers();

app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();