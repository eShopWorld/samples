using Eshopworld.Core;

namespace Eshopworld.EDA.Samples.Events
{
    public class WeatherUpdateEvent : DomainEvent
    {
        /// <summary>
        /// Full id of the update event
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The location of the weather update which is used for selection. ie: I want this event or not.
        /// </summary>
        public WeatherUpdateLocation Location { get; set; } 
    }
}