using System;
using Eshopworld.EDA.Samples.Events;

namespace Eshopworld.EDA.Samples.Models
{
    public class WeatherForecastModel
    {
        public  string Id { get; set; }

        public DateTime Date { get; set; }

        public WeatherUpdateLocation Location { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
