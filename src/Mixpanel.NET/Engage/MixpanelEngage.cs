using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Mixpanel.NET.Events;

namespace Mixpanel.NET.Engage {
  public class MixpanelEngage : MixpanelClientBase, IEngage {
    private readonly EngageOptions _options;

    /// <summary>
    /// Creates a new Mixpanel Engage client for a given API token
    /// </summary>
    /// <param name="token">The API token for MixPanel</param>
    /// <param name="http">Optional: An implementation of IMixpanelHttp, <see cref="MixpanelHttp"/>
    /// Determines if class names and properties will be serialized to JSON literally.
    /// If false (the default) spaces will be inserted between camel-cased words for improved 
    /// readability on the reporting side.
    /// </param>
    /// <param name="options">Optional: Specific options for the API <see cref="EngageOptions"/></param>
    public MixpanelEngage(string token, IMixpanelHttp http = null, EngageOptions options = null)
      : base(token, http) {
      _options = options ?? new EngageOptions();
    }

    private bool Engage(string distinctId, IDictionary<string, object> setProperties = null, 
      IDictionary<string,object> setOnceProperties = null, IDictionary<string, object> incrementProperties = null,
      IDictionary<string, object> appendProperties = null, IDictionary<string, object> transactionProperties = null) {
      // Standardize token and time values for Mixpanel
      var dictionary = 
        new Dictionary<string, object> {{"$token", token}, {"$distinct_id", distinctId}};

      if (setProperties != null) dictionary.Add("$set", setProperties);

      if (setOnceProperties != null) dictionary.Add("$set_once", setProperties);

      if (incrementProperties != null) dictionary.Add("$add", incrementProperties);

      if (appendProperties != null) {
          appendProperties.Add("$transactions", transactionProperties);
          dictionary.Add("$append", appendProperties);
      }

      var data = new JavaScriptSerializer().Serialize(dictionary);

      var values = "data=" + data.Base64Encode();

      var contents = _options.UseGet
        ? http.Get(Resources.Engage(_options.ProxyUrl), values)
        : http.Post(Resources.Engage(_options.ProxyUrl), values);

      return contents == "1";
    }

    public bool Set(string distinctId, IDictionary<string, object> setProperties) {
      return Engage(distinctId, setProperties);
    }

    public bool SetOnce(string distictId, IDictionary<string, object> setOnceProperties) {
        return Engage(distictId, setOnceProperties);
    }

    public bool Increment(string distinctId, IDictionary<string, object> incrementProperties) {
        return Engage(distinctId, incrementProperties: incrementProperties);
    }

    public bool Append(string distinctId, IDictionary<string, object> appendProperties, IDictionary<string, object> transactionProperties) {
        return Engage(distinctId, appendProperties: appendProperties, transactionProperties: transactionProperties);
    }
  }
}