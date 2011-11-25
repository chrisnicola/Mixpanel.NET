using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Linq;

namespace Mixpanel.NET {
  public class MixpanelTracker : IEventTracker
  {
    readonly IMixpanelHttp _http;
    readonly TrackerOptions _options;
    readonly string _token;

    public MixpanelTracker(string token, TrackerOptions options = null) : this(token, new MixpanelHttp(), options) { }

    /// <summary>
    /// Creates a new Mixpanel tracker for a given API token
    /// </summary>
    /// <param name="token">The API token for MixPanel</param>
    /// <param name="http">An implementation of IMixpanelHttp, <see cref="MixpanelHttp"/></param>
    /// <param name="literialSerialization">
    /// Determines if class names and properties will be serialized to JSON literally.
    /// If false (the default) spaces will be inserted between camel-cased words for improved 
    /// readability on the reporting side.
    /// </param>
    public MixpanelTracker(string token, IMixpanelHttp http, TrackerOptions options = null) {
      _token = token;
      _http = http;
      _options = options ?? new TrackerOptions();
    }

    public bool Track(string @event, Dictionary<string, object> properties) {
      properties["token"] = _token;
      if (!properties.ContainsKey("Time") || !properties.ContainsKey("time")) 
        properties["time"] = DateTime.UtcNow;
      if ((!properties.ContainsKey("Bucket") || !properties.ContainsKey("bucket")) && _options.Bucket != null)
        properties["bucket"] = _options.Bucket;
      var data = new JavaScriptSerializer().Serialize(new Dictionary<string, object> {
        {"event", @event}, {"properties", properties}
      });
      var requestUriString = string.Format("{0}/?data={1}", Resources.Track(_options.ProxyUrl), data.Base64Encode());
      if (_options.Test) requestUriString += "&test=1";
      var contents = _http.Get(requestUriString);
      return contents == "1";
   }    

    public bool Track<T>(T @event, string bucket = null, bool test = false) {
      var name = _options.LiteralSerialization ? @event.GetType().Name : @event.GetType().Name.SplitCamelCase();
      var properties = @event.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
      var propertyBag = properties.ToDictionary(
        x => _options.LiteralSerialization ? x.Name : x.Name.SplitCamelCase(),
        x => x.GetValue(@event, null));
      return Track(name, propertyBag);
    }
  }

  public class MixpanelEvent {
    public MixpanelEvent() {
      Properties = new Dictionary<string, object>();      
    }
    public MixpanelEvent(string name, Dictionary<string, object> properties = null) {
      Event = name;  
      Properties = properties ?? new Dictionary<string, object>();
    }

    public string Event { get; set; }
    public Dictionary<string, object> Properties { get; set; }
  }
}