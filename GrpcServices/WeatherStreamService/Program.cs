
using WeatherStreamService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGrpcService<WeatherStreamService.Services.WeatherService>();


app.Run();