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
                    
                    var weatherModel = payload.GetModel<WeatherUpdateEvent, WeatherForecastModel>();
                    _weatherService.DoBusinessLogic(weatherModel);
                    return NoContent();
                
                case nameof(StormNotificationEvent):

                    var stormModel = payload.GetModel<StormNotificationEvent, StormModel>();
                    _stormService.DoBusinessLogic(stormModel);
                    return NoContent();
                
                default:
                    throw new NotImplementedException($"The specified event type {nameof(@event)} was not found");
            }
        }
    }

    public static class ModelExtensions
    {
        /// <summary>
        /// This extension method does a bit of work for you.
        /// It first converts the JObject to the specified typed event T
        /// After this it makes a call back to the origin server using the Id within the model to get the full details of the update, which triggered the event in the first place.
        /// When this is returned we do some magic and give you back the TR type you specify, if a mapping exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static TR GetModel<T, TR>(this JObject payload)
        {
            //to introduce the callback.
            //get the payload
            //convert and return.

            var obj =  payload.ToObject<TR>();
            return obj;
        }
    }
}