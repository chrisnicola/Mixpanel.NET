namespace Mixpanel.NET.Events
{
    /// <summary>
    /// Class provides all options available when sending data to Mixpanel.
    /// </summary>
    public class TrackerOptions : MixpanelClientOptions
    {
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
    }
}