using System.Collections.Generic;

namespace Mixpanel.NET.Events
{
  public interface IEventTracker {
    bool Track(string @event, IDictionary<string, object> properties);
    bool Track(MixpanelEvent @event);
    bool Track<T>(T @event);
  }
}