using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Eshopworld.EDA.Samples.Controllers
{
    public static class Extensions
    {
        /// <summary>
        /// Converts JObject to concrete type.
        /// </summary>
        /// <typeparam name="TR"></typeparam>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static TR GetModel<TR>(this JObject payload)
        {
            var obj =  payload.ToObject<TR>();
            return obj;
        }

        /// <summary>
        /// Helper to parse out the esw event type
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetEswEventTypeHeader(this HttpRequest request)
        {
            var response = request.Headers.TryGetValue(Constants.EventTypeHeader, out var @event);

            return !response ? null : @event.ToString();
        }
    }
}