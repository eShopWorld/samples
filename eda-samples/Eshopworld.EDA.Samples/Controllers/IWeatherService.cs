using Eshopworld.EDA.Samples.Models;

namespace Eshopworld.EDA.Samples.Controllers
{
    public interface IWeatherService
    {
        void DoBusinessLogic(WeatherForecastModel weatherModel);
    }
}