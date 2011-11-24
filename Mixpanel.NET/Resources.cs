using System;

namespace Mixpanel.NET {
  public static class Resources {
    public static string BaseUrl { get { return "http://api.mixpanel.com"; }}
    public static string Track { get { return BaseUrl + "/track"; }}
  }

  public abstract class TrackingEvent
  {
    public DateTime? Time { get; set; }
  }
}           