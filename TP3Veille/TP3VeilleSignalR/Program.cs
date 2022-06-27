using TP3VeilleSignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<ChatHub>("/chat");

app.Run();
