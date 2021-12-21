using Eshopworld.EDA.Samples.Events;

namespace Eshopworld.EDA.Samples.Models
{
    public class StormModel
    {
        public string Id { get; set; }

        public StormClassification StormClassification { get; set; }

        public WeatherUpdateLocation Location { get; set; }
    }
}