namespace Mixpanel.NET.Events
{
  /// <summary>
  /// Class provides all options available when sending data to Mixpanel.
  /// </summary>
  public class TrackerOptions {
    /// <summary>
    /// If true the tracker will automatically set the event timestamp for each event, unless the 
    /// property "time" or "Time" has already been set to something else.  Otherwise no time will
    /// be set if it doesn't exist.
    /// default: true
    /// </summary>
    public bool SetEventTime = true;

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

    /// <summary>
    /// Mixpanel supports both GET and POST for submitting tracking data. We recommend POST since it
    /// the proper way of submitting data via HTTP, however GET support is availabe if desired by
    /// setting this flag to true. 
    /// default: false 
    /// </summary>
    public bool UseGet { get; set; }
  }
}