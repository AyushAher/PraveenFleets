using Domain;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Configuration;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public readonly IWeatherForecast WeatherForecast;

        public WeatherForecastController(IWeatherForecast weatherForecast)
        {
            WeatherForecast = weatherForecast;
        }

        [HttpGet]
        public async Task<ApiResponse<WeatherForecast[]>> Get()
            => await WeatherForecast.GetForecast();
    }
}