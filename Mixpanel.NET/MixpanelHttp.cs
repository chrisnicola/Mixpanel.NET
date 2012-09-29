using System.IO;
using System.Net;
using System.Text;

namespace Mixpanel.NET
{
  /// <summary>
  /// This helper class and interface largly exists to improve readability and testability since there is 
  /// no way to do that with the WebRequest class cleanly.
  /// </summary>
  public interface IMixpanelHttp {
    string Get(string uri, string query);
    string Post(string uri, string body);
  }

  public class MixpanelHttp : IMixpanelHttp {
    public string Get(string uri, string query) {
      var request = WebRequest.Create(uri + "?" + query);
      var response = request.GetResponse();
      var responseStream = response.GetResponseStream();
      return responseStream == null 
        ? null
        : new StreamReader(responseStream).ReadToEnd();
    }

    public string Post(string uri, string body) {
      var request = WebRequest.Create(uri);
      request.Method = "POST";
      request.ContentType = "application/x-www-form-urlencoded";
      var bodyBytes = Encoding.UTF8.GetBytes(body);
      request.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);
      var response = request.GetResponse();
      var responseStream = response.GetResponseStream();
      return responseStream == null 
        ? null
        : new StreamReader(responseStream).ReadToEnd();
    }
  }
}