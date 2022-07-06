using TP3VeilleSignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(options => options.WithOrigins(app.Configuration["Origin"]).AllowAnyHeader().AllowAnyMethod().AllowCredentials());

app.MapHub<ChatHub>("/chat");

app.Run();
