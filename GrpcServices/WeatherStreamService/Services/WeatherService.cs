
using Grpc.Core;
using Newtonsoft.Json;

namespace WeatherStreamService.Services;

public class WeatherService(HttpClient httpClient):WeatherStreamService.WeatherService.WeatherServiceBase
{
    public override async Task GetWeatherStream(WeatherRequest request, IServerStreamWriter<WeatherResponse> responseStream, ServerCallContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        DateTime startTime = DateTime.UtcNow;
        DateTime weatherTime = startTime;

        while (!context.CancellationToken.IsCancellationRequested)
        {
            // Запрос к Open-Meteo с использованием времени прогноза
            var forecastData = await GetWeatherDataAsync(request.Location, weatherTime);
            string timestamp = weatherTime.ToString("dd.MM.yyyy HH:mm");
            
            await responseStream.WriteAsync(new WeatherResponse
            {
                Timestamp = timestamp,
                Weather = forecastData
            });

            // 2 часа интервал
            weatherTime = weatherTime.AddHours(2);

            // Ожидание 1 секунда
            await Task.Delay(1000, context.CancellationToken);
        }
    }

    private async Task<string> GetWeatherDataAsync(string location, DateTime time)
    {
        string apiUrl = $"https://api.open-meteo.com/v1/forecast?latitude=35&longitude=139&hourly=temperature_2m&start={time:yyyy-MM-ddTHH:mm:ss}&timezone=UTC";
        
        var response = await httpClient.GetStringAsync(apiUrl);
        dynamic weatherData = JsonConvert.DeserializeObject(response);
        // Предполагаем, что данные о температуре находятся по пути.
        return $"{weatherData.hourly.temperature_2m[0]}C"; // Возврат первой температуры
    }
}