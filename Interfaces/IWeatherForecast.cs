using Domain;
using Shared.Configuration;

namespace Interfaces
{
    public interface IWeatherForecast : IService
    {
        public Task<ApiResponse<WeatherForecast[]>> GetForecast();
    }
}
