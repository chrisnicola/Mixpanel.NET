using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

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
      
    public static IDictionary<string,string> UriParameters(this string source) {
      if (!Uri.IsWellFormedUriString(source, UriKind.RelativeOrAbsolute)) return null;
      var query = new Uri(source).Query;
      return query.TrimStart('?').Split('&').ToDictionary(x => x.Split('=')[0], x => x.Substring(x.Split('=')[0].Length + 1));
    }

    public static MixpanelEvent ParseEvent(this string data) {
      return new JavaScriptSerializer().Deserialize<MixpanelEvent>(data);
    }

    public static string SplitCamelCase(this string value)
    {
      var regex = new Regex("(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");
      return regex.Replace(value, " ");
    }
  }
}