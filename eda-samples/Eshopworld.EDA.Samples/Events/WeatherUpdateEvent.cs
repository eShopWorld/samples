﻿using Eshopworld.Core;

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

    /// <summary>
    /// Simple enum to illustrate local selection and callback to API.
    /// </summary>
    public enum WeatherUpdateLocation
    {
        Ireland = 1,
        UK = 2, 
        France = 3
    }
}