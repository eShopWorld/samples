using Microsoft.AspNetCore.Http;

namespace Eshopworld.EDA.Samples.Controllers
{
    public static class HeaderExtensions
    {
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