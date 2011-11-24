using System.IO;
using System.Net;

namespace Mixpanel.NET
{
  /// <summary>
  /// This helper class and interface largly exists to improve readability and testability since there is 
  /// no way to do that with the WebRequest class cleanly.
  /// </summary>
  public interface IMixpanelHttp {
    string Get(string uri);      
  }

  public class MixpanelHttp : IMixpanelHttp {
    public string Get(string uri) {
      var request = WebRequest.Create(uri);
      var response = request.GetResponse();
      var responseStream = response.GetResponseStream();
      return responseStream == null 
        ? null
        : new StreamReader(responseStream).ReadToEnd();
    }
  }
}