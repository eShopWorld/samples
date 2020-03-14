using Eshopworld.EDA.Samples.Models;
using FluentValidation;

namespace Eshopworld.EDA.Samples.Validators
{
    public class WeatherForcastModelValidator : AbstractValidator<WeatherForecastModel>
    {
        public WeatherForcastModelValidator()
        {
            RuleFor(x => x.Date).NotNull();
            RuleFor(x => x.TemperatureC).NotEmpty();
            RuleFor(x => x.Summary).NotEmpty();
        }
    }
}