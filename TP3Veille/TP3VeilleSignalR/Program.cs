using TP3VeilleSignalR.Data;
using TP3VeilleSignalR.Hubs;
using TP3VeilleSignalR.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ChatDbSettings>(builder.Configuration.GetSection("ChatDbSettings"));
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IConversationsService, ConversationsService>();

builder.Services.AddSignalR();

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(options 
    => options.WithOrigins(app.Configuration["Origin"])
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
);

app.MapHub<ChatHub>("/chat");

app.Run();
