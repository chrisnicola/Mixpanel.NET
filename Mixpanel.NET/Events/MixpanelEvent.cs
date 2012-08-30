using System.Collections.Generic;

namespace Mixpanel.NET.Events
{
  /// <summary>
  /// Structure which represents a standard Mixpanel event. Mixpanel events are each
  /// given a name which is sent as "event" and a property bag sent as "properties".
  /// The entire event is serialized to JSON and so the property bag must contain 
  /// only types supported by JSON serialization and should be flat (no nested properties).
  /// 
  /// This class is also useful for testing or building proxies where deserialization of the 
  /// events may be needed.
  /// </summary>
  public class MixpanelEvent
  {
    public MixpanelEvent() { }

    public MixpanelEvent(string name, IDictionary<string,object> properties = null) {
      Event = name;
      Properties = properties ?? new Dictionary<string, object>();
    }

    public string Event { get; set; }

    public IDictionary<string, object> Properties { get; set; }
  }
}