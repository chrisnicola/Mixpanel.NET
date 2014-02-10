using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Mixpanel.NET.Engage
{
    public class MixpanelEngage : MixpanelClientBase, IEngage
    {
        private readonly EngageOptions _options;

        /// <summary>
        /// Creates a new Mixpanel Engage client for a given API token
        /// </summary>
        /// <param name="token">The API token for MixPanel</param>
        /// <param name="http">Optional: An implementation of IMixpanelHttp, <see cref="MixpanelHttp"/>
        /// Determines if class names and properties will be serialized to JSON literally.
        /// If false (the default) spaces will be inserted between camel-cased words for improved 
        /// readability on the reporting side.
        /// </param>
        /// <param name="options">Optional: Specific options for the API <see cref="EngageOptions"/></param>
        public MixpanelEngage(string token, IMixpanelHttp http = null, EngageOptions options = null)
            : base(token, http)
        {
            _options = options ?? new EngageOptions();
        }

        private bool Engage(string distinctId,
                            IDictionary<string, object> setProperties = null,
                            IDictionary<string, object> incrementProperties = null,
                            string ip = null,
                            bool delete = false)
        {
            // Standardize token and time values for Mixpanel
            var dictionary = new Dictionary<string, object> { { "$token", token }, { "$distinct_id", distinctId } };

            //enables the possibility to explicitly set $ip outside $set
            if (!string.IsNullOrWhiteSpace(ip)) dictionary.Add("$ip", ip);

            if (setProperties != null) dictionary.Add("$set", setProperties);

            if (incrementProperties != null) dictionary.Add("$add", incrementProperties);

            if (delete) dictionary.Add("$delete","");

            var data = new JavaScriptSerializer().Serialize(dictionary);

            var values = "data=" + data.Base64Encode();

            var contents = _options.UseGet
              ? http.Get(Resources.Engage(_options.ProxyUrl), values)
              : http.Post(Resources.Engage(_options.ProxyUrl), values);

            return contents == "1";
        }

        // sets the "Address" and "Birthday"
        // properties of user 13793
        //{
        //    "$token": "36ada5b10da39a1347559321baf13063",
        //    "$distinct_id": "13793",
        //    "$ip": "123.123.123.123",
        //    "$set": {
        //        "Address": "1313 Mockingbird Lane",
        //        "Birthday": "1948-01-01"
        //    }
        //}
        public bool Set(string distinctId, IDictionary<string, object> setProperties, string ip = null)
        {
            return Engage(distinctId, setProperties, ip: ip);
        }

        public bool Increment(string distinctId, IDictionary<string, object> incrementProperties, string ip = null)
        {
            return Engage(distinctId, incrementProperties: incrementProperties, ip: ip);
        }

        // This removes the user 13793 from Mixpanel
        //{
        //    "$token": "36ada5b10da39a1347559321baf13063",
        //    "$distinct_id": "13793",
        //    "$delete": ""
        //}
        public bool Delete(string distinctId)
        {
            return Engage(distinctId,null,null,null,true);
        }
    }
}