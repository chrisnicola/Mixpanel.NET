using System;

namespace Mixpanel.NET
{
  public abstract class TrackingEvent
  {
    public TrackingEvent() {
      Time = DateTime.UtcNow;
    }
    public DateTime? Time { get; set; }
  }
}