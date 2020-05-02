using System.Collections.Generic;

namespace Mixpanel.NET.Engage {
  public interface IEngage {
    bool Delete(string distinctId);
    bool Set(string distinctId, IDictionary<string, object> setProperties, string ip);
	bool Append(string distinctId, IDictionary<string, object> appendProperties, string ip);
    bool Increment(string distinctId, IDictionary<string, object> incrementProperties, string ip);
  }
}