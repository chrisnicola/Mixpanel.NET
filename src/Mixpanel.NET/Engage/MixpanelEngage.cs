using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Mixpanel.NET.Events;

namespace Mixpanel.NET.Engage {
  public class MixpanelEngage : MixpanelClientBase, IEngage {
    private const string EngagePropertyKeyIp = "$ip";

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
    public bool Set(string distinctId, IDictionary<string, object> setProperties)
    {
        return Engage(distinctId, setProperties);
    }

    public bool Increment(string distinctId, IDictionary<string, object> incrementProperties)
    {
        return Engage(distinctId, incrementProperties: incrementProperties);
    }

    private bool Engage(string distinctId, IDictionary<string, object> setProperties = null, 
      IDictionary<string, object> incrementProperties = null) {
      var dictionary = CreateEngageDictionary(distinctId);

        dictionary = AppendSetProperties(setProperties, dictionary);
        dictionary = AppendIncrementProperties(incrementProperties, dictionary);

        var data = new JavaScriptSerializer().Serialize(dictionary);

      var values = "data=" + data.Base64Encode();

      var contents = _options.UseGet
        ? http.Get(Resources.Engage(_options.ProxyUrl), values)
        : http.Post(Resources.Engage(_options.ProxyUrl), values);

      return contents == "1";
    }

      private IDictionary<string, object> CreateEngageDictionary(string distinctId)
      {
          return new Dictionary<string, object> {{"$token", token}, {"$distinct_id", distinctId}};
      }

      private IDictionary<string, object> AppendIncrementProperties(IDictionary<string, object> incrementProperties, IDictionary<string, object> dictionary)
      {
          return AppendProperties("$add", incrementProperties, dictionary);
      }

      private IDictionary<string, object> AppendSetProperties(IDictionary<string, object> setProperties, IDictionary<string, object> dictionary)
      {
          return AppendProperties("$set", setProperties, dictionary);
      }

      private IDictionary<string, object> AppendProperties(string propertiesKey, IDictionary<string, object> properties, IDictionary<string, object> dictionary)
      {
          if (properties == null) return dictionary;
          dictionary.Add(propertiesKey, properties);
          dictionary = StandardizeIpProperty(dictionary, properties);
          return dictionary;
      }

      private IDictionary<string, object> StandardizeIpProperty(IDictionary<string, object> data,
          IDictionary<string, object> properties)
      {
          if (!properties.ContainsKey(EngagePropertyKeyIp))
              return data;

          data[EngagePropertyKeyIp] = properties[EngagePropertyKeyIp];
          properties.Remove(EngagePropertyKeyIp);

          return data;
      }
  }
}