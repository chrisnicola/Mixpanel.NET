using System.Collections.Generic;

namespace Mixpanel.NET.Engage {
  public interface IEngage {
    bool Set(string distinctId, IDictionary<string, object> setProperties);
    bool Increment(string distinctId, IDictionary<string, object> incrementProperties);
  }
}