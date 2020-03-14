using System;
using Eshopworld.EDA.Samples.Events;
using Eshopworld.EDA.Samples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Eshopworld.EDA.Samples.Controllers
{
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
        /// A generic and singular webhook endpoint which can consume any number of generic events,
        /// deserialise them into their respective types and process the data based on request business logic
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(JObject payload)
        {
            var @event = Request.GetEswEventTypeHeader();

            if (@event == null)
            {
                //todo check new error and validation patterns in dotnet core 3
                return BadRequest(new { Message = $"{Constants.EventTypeHeader} was not found" });
            }

            switch (@event)
            {
                case nameof(WeatherUpdateEvent):
                    
                    var weatherModel = payload.GetModel<WeatherForecastModel>();
                    _weatherService.DoBusinessLogic(weatherModel);
                    return NoContent();
                
                case nameof(StormNotificationEvent):

                    var stormModel = payload.GetModel<StormModel>();
                    _stormService.DoBusinessLogic(stormModel);
                    return NoContent();
                
                default:
                    throw new NotImplementedException($"The specified event type {nameof(@event)} was not found");
            }
        }
    }
}