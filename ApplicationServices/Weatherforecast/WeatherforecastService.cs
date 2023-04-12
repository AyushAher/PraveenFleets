using Domain;
using Interfaces;
using Microsoft.Extensions.Logging;
using Shared.Configuration;

namespace ApplicationServices.Weatherforecast
{
    public class WeatherForecastService : IWeatherForecast
    {
        private readonly ILogger _logger;

        public WeatherForecastService(ILogger<WeatherForecastService> logger)
        {
            _logger = logger;
        }

        private static readonly string[] Summaries = {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public async Task<ApiResponse<WeatherForecast[]>> GetForecast()
        {
            try
            {
                var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                    .ToArray();

                _logger.LogInformation("Success");

                return await ApiResponse<WeatherForecast[]>.SuccessAsync(result);
            }
            catch (Exception exception)
            {
                return await ApiResponse<WeatherForecast[]>.FatalAsync(exception, _logger);
            }

        }
    }
}