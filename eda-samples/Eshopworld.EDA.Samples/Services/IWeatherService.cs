using Eshopworld.EDA.Samples.Models;

namespace Eshopworld.EDA.Samples.Services
{
    public interface IWeatherService
    {
        void DoBusinessLogic(WeatherForecastModel weatherModel);
    }
}