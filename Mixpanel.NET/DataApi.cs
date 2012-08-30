using System.Security.Cryptography;
using System.Text;

namespace Mixpanel.NET
{
  public class DataApi {
    readonly string _apiKey;
    readonly string _apiSecret;

    public DataApi(string apiKey, string apiSecret) {
      _apiKey = apiKey;
      _apiSecret = apiSecret;
    }
  }
}