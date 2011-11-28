using System;

namespace Mixpanel.NET
{
  public abstract class TrackingEventBase
  {
    public TrackingEventBase() {
      Time = DateTime.UtcNow;
    }
    public DateTime? Time { get; set; }
  }
}