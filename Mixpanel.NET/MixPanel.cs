using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Mixpanel.NET
{
  public class MixPanel {
    readonly string _token;

    public MixPanel(string token) {
      _token = token;
    }

    public bool Track(string @event, Dictionary<string, object> properties, bool test = false) {
      properties["token"] = _token;
      if (!properties.ContainsKey("Time") || !properties.ContainsKey("time"))
      {
        properties["time"] = DateTime.UtcNow;
      }
      var data = new JavaScriptSerializer().Serialize(new Dictionary<string, object>
      {
        {"event", @event}, {"properties", properties}
      });
      var bytes = Encoding.UTF8.GetBytes(data);
      var data64 = Convert.ToBase64String(bytes);
      var requestUriString = string.Format("{0}/?data={1}", Resources.Track, data64);
      if (test) requestUriString += "&test=1";
      var request = WebRequest.Create(requestUriString);
      var response = request.GetResponse();
      var contents = new StreamReader(response.GetResponseStream()).ReadToEnd();
      return contents == "1";
    }
  }
}