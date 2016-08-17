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

		private bool Engage(string distinctId, string ip, Dictionary<string, object> data)
		{
			// Standardize token and time values for Mixpanel
			var dictionary = new Dictionary<string, object> { { "$token", token }, { "$distinct_id", distinctId } };

			if (!string.IsNullOrWhiteSpace(ip))
				dictionary.Add("$ip", ip);

			foreach (var pair in data)
				dictionary.Add(pair.Key, pair.Value);

			var values = "data=" + new JavaScriptSerializer().Serialize(dictionary).Base64Encode();
			var contents = _options.UseGet
			  ? http.Get(Resources.Engage(_options.ProxyUrl), values)
			  : http.Post(Resources.Engage(_options.ProxyUrl), values);

			return contents == "1";
		}

		public bool Delete(string distinctId)
		{
			return Engage(distinctId, null, new Dictionary<string, object>() {
				{ "$delete", string.Empty }
			});
		}

		public bool Set(string distinctId, IDictionary<string, object> setProperties, string ip = null)
		{
			return Engage(distinctId, null, new Dictionary<string, object>() {
				{ "$set", setProperties.FormatProperties() }
			});
		}

		public bool Append(string distinctId, IDictionary<string, object> appendProperties, string ip = null)
		{
			return Engage(distinctId, null, new Dictionary<string, object>()
			{
				{ "$append", appendProperties }
			});
		}

		public bool Increment(string distinctId, IDictionary<string, object> incrementProperties, string ip = null)
		{
			return Engage(distinctId, null, new Dictionary<string, object>() {
				{ "$add", incrementProperties.FormatProperties() }
			});
		}

		public bool Unset(string distinctId, IList<string> properties, string ip = null)
		{
			return Engage(distinctId, null, new Dictionary<string, object>() {
				{ "$unset", properties }
			});
		}

	}
}