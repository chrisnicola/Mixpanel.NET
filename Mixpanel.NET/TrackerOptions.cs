namespace Mixpanel.NET
{
  /// <summary>
  /// Class provides all options available when sending data to Mixpanel.
  /// </summary>
  public class TrackerOptions {
    /// <summary>
    /// Specify a bucket that this tracker's data should be placed into. Even if set this value can be 
    /// overridden by including a 'Bucket' property value in the event data itself.
    /// see: http://mixpanel.com/api/docs/guides/platform 
    /// default: null
    /// </summary>
    public string Bucket { get; set; }

    /// <summary>
    /// If true then the test flag is used when sending data to Mixpanel.
    /// default: false
    /// </summary>
    public bool Test { get; set; }

    /// <summary>
    /// If true then when classes are seralized the class and property names are sent literally
    /// otherwise, if false, spaces are inserted in the camel-case naming to make it more readable.
    /// default: false
    /// </summary>
    public bool LiteralSerialization { get; set; }

    /// <summary>
    /// If you forward events to mixpanel via a proxy you can set the URL here.  If not set
    /// then the default value set in <see cref="Resources.MixpanelUrl"/> is used.
    /// default: null
    /// </summary>
    public string ProxyUrl { get; set; }
  }
}