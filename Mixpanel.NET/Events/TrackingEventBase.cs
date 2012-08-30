using System;

namespace Mixpanel.NET.Events
{
  public abstract class TrackingEventBase
  {
    public TrackingEventBase() {
      Time = DateTime.UtcNow;
    }
    public DateTime? Time { get; set; }
  }
}