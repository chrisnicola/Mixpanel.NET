using System.Collections.Generic;

namespace Mixpanel.NET
{
  public interface IEventTracker {
    bool Track(string @event, Dictionary<string, object> properties);
    bool Track<T>(T @event, string bucket = null, bool test = false);
  }
}