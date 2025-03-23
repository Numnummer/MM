using Grpc.Core;
using Grpc.Net.Client;
using WeatherStreamService;

using var channel = GrpcChannel.ForAddress("http://localhost:5223");
var client = new WeatherService.WeatherServiceClient(channel);


var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) => {
    e.Cancel = true; // Prevent the process from terminating
    cts.Cancel();
};

using var call = client.GetWeatherStream(new WeatherRequest { Location = "Tokyo" });

try
{
    await foreach (var response in call.ResponseStream.ReadAllAsync(cts.Token))
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} погода на {response.Timestamp} = {response.Weather}");
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Операция отменена.");
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
}