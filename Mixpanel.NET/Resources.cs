namespace Mixpanel.NET {
  public static class Resources {
    public static string MixpanelUrl { get { return "http://api.mixpanel.com"; }}

    public static string Track(string proxy = null) {
      proxy = proxy ?? MixpanelUrl;
      return proxy + "/track";
    }

    public static string Engage(string proxy = null) {
      proxy = proxy ?? MixpanelUrl;
      return proxy + "/engage";
    }
  }
}