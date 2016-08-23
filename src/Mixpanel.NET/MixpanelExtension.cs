using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Mixpanel.NET.Events;

namespace Mixpanel.NET {
  public static class MixpanelExtension {
    public static string Base64Encode(this string source) {
      var bytes = Encoding.UTF8.GetBytes(source);
      return Convert.ToBase64String(bytes);
    }

    public static string Base64Decode(this string source) {
      var bytes = Convert.FromBase64String(source);
      return Encoding.UTF8.GetString(bytes);
    }
      
    public static IDictionary<string,string> UriParameters(this string query) {
      return query.TrimStart('?').Split('&').ToDictionary(x => x.Split('=')[0], x => x.Substring(x.Split('=')[0].Length + 1));
    }

    public static MixpanelEvent ToMixpanelEvent(this object @event, bool literalSerialization = false) {
      var name = literalSerialization ? @event.GetType().Name : @event.GetType().Name.SplitCamelCase();
      var properties = @event.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
      var propertyBag = properties.ToDictionary(
        x => literalSerialization ? x.Name : x.Name.SplitCamelCase(),
        x => x.GetValue(@event, null));
      return new MixpanelEvent(name, propertyBag);
    }

    public static MixpanelEvent ParseEvent(this string data) {
      return new JavaScriptSerializer().Deserialize<MixpanelEvent>(data);
    }

    public static string SplitCamelCase(this string value) {
      var regex = new Regex("(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");
      return regex.Replace(value, " ");
    }

    public static string ComputeHash(this string value) {
      var bytes = Encoding.UTF8.GetBytes(value);
      var hash = MD5.Create().ComputeHash(bytes);
      var hexDigest = hash.Aggregate("", (x,y) => x + y.ToString("X").ToLower());
      return hexDigest;
    }
 
    /// <summary>
    /// Converts the time to "sortable" format which MixPanel understands
    /// https://mixpanel.com/docs/properties-or-segments/property-data-types
    /// http://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
    /// </summary>
    public static string FormatDate(this DateTime value) {
      return value.ToString("s");
    }

    public static IDictionary<string, object> FormatProperties(this IDictionary<string, object> values) {
      var output = new Dictionary<string, object>();
      foreach (var prop in values)
      {
	      var value = prop.Value as IDictionary<string, object>;
	      if (value != null)
	      {
		      output[prop.Key] = FormatProperties(value);
	      }
		  else if (prop.Value is DateTime)
	      {
		      output[prop.Key] = ((DateTime) prop.Value).FormatDate();
	      }
	      else
	      {
		      output[prop.Key] = prop.Value;
	      }
      }
	    return output;
    }
  }
}