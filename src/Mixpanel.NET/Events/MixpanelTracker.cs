using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Linq;

namespace Mixpanel.NET.Events
{
    public class MixpanelTracker : MixpanelClientBase, IEventTracker
    {
        private readonly TrackerOptions _options;

        /// <summary>
        /// Creates a new Mixpanel tracker for a given API token
        /// </summary>
        /// <param name="token">The API token for MixPanel</param>
        /// <param name="http">An implementation of IMixpanelHttp, <see cref="MixpanelHttp"/>
        /// Determines if class names and properties will be serialized to JSON literally.
        /// If false (the default) spaces will be inserted between camel-cased words for improved 
        /// readability on the reporting side.
        /// </param>
        /// <param name="options">Optional: Specific options for the API <see cref="TrackerOptions"/></param>
        public MixpanelTracker(string token, IMixpanelHttp http = null, TrackerOptions options = null)
            : base(token, http)
        {
            _options = options ?? new TrackerOptions();
        }

        public bool Track(string @event, IDictionary<string, object> properties)
        {
            var propertyBag = new Dictionary<string, object>(properties);
            // Standardize token and time values for Mixpanel
            propertyBag["token"] = token;
            if (_options.SetEventTime)
            {
                propertyBag["time"] =
                    propertyBag.Where(x => x.Key.ToLower() == "time")
                    .Select(x => x.Value)
                    .FirstOrDefault() ?? DateTime.UtcNow;

                propertyBag.Remove("Time");
            }

            var data =
                new JavaScriptSerializer()
                .Serialize(new Dictionary<string, object>
                {
                    { "event", @event },
                    { "properties", propertyBag }
                });

            var values = "data=" + data.Base64Encode();

            if (_options.Test) values += "&test=1";

            var contents = _options.UseGet
              ? http.Get(Resources.Track(_options.ProxyUrl), values)
              : http.Post(Resources.Track(_options.ProxyUrl), values);

            return contents == "1";
        }

        public bool Track(MixpanelEvent @event)
        {
            return Track(@event.Event, @event.Properties);
        }

        public bool Track<T>(T @event)
        {
            return Track(@event.ToMixpanelEvent(_options.LiteralSerialization));
        }
    }
}