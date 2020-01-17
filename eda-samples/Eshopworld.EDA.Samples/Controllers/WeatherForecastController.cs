using System;
using System.Collections.Generic;
using System.Linq;
using Eshopworld.Core;
using Eshopworld.EDA.Samples.Events;
using Eshopworld.EDA.Samples.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Eshopworld.EDA.Samples.Controllers
{
    public static class Constants
    {
        public const string EventTypeHeader = "Esw-EventType";
    }

    public interface IWeatherService
    {
        void DoBusinessLogic();
    }

    public interface IStormService
    {
        void DoBusinessLogic();
    }

    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly IStormService _stormService;

        public WebhookController(
            IWeatherService weatherService, 
            IStormService stormService)
        {
            _weatherService = weatherService;
            _stormService = stormService;
        }

        /// <summary>
        /// A generic and singular webhook endpoint which can consume any number of generic events, deserialise them into their respective types and process the data based on request business logic
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(JObject payload)
        {
            //todo move to helper extension method

            var @event = Request.GetEswEventTypeHeader();

            if (@event == null)
            {
                //todo check new error and validation patterns in dotnet core 3
                return BadRequest(new { Message = $"{Constants.EventTypeHeader} was not found" });
            }

            switch (@event)
            {
                case nameof(WeatherUpdateEvent):
                    _weatherService.DoBusinessLogic();
                    break;
                case nameof(StormNotificationEvent):
                    _stormService.DoBusinessLogic();
                    break;
                default:
                    throw new NotImplementedException($"The specified event type {nameof(@event)} was not found");
            }

            return NoContent();
        }
    }

    /// <summary>
    /// This is a sample controller which illustrations how to push messages to EDA using an event model using light events where the model is enriched.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = {

            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IBigBrother _bigBrother;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly Guid _sampleId = Guid.Parse("738C2996-2CA3-4E05-B8D5-912A9F49EAE5");

        public WeatherForecastController(
            IBigBrother bigBrother,
            ILogger<WeatherForecastController> logger)
        {
            _bigBrother = bigBrother;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<WeatherForecastModel> Get(Guid? id)
        {
            var rng = new Random();
            if (id != null)
            {
                return Enumerable.Range(1, 5).Select(index => new WeatherForecastModel
                {
                    Id = $"https://localhost:44315/api/v1/weather/{id}",
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)],
                    Location = WeatherUpdateLocation.Ireland
                }).ToArray();
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecastModel
            {
                Id = $"https://localhost:44315/api/v1/weather/{Guid.NewGuid()}",
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weatherModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([NotNull] WeatherForecastModel weatherModel)
        {
            if (weatherModel == null) throw new ArgumentNullException(nameof(weatherModel));
            if (weatherModel.Date == null) throw new ArgumentNullException(nameof(weatherModel.Date));
            if (weatherModel.TemperatureC == default) throw new ArgumentNullException(nameof(weatherModel.TemperatureC));
            if (string.IsNullOrWhiteSpace(weatherModel.Summary)) throw new ArgumentNullException(nameof(weatherModel.Summary));

            var @event = new WeatherUpdateEvent
            {
                Id = $"https://localhost:44315/api/v1/weather/{_sampleId}",
                Location = WeatherUpdateLocation.Ireland
            };

            _bigBrother.Publish(@event);

            return NoContent();
        }
    }
}
