namespace Mixpanel.NET
{
    public class MixpanelClientOptions
    {
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