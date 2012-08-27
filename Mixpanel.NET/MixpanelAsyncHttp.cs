using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Mixpanel.NET
{
    public class MixpanelAsyncHttp : IMixpanelHttp
    {
        public string Get(string uri, string query)
        {
            var request = WebRequest.Create(uri + "?" + query);
            RequestAsync(request, new byte[0]);
            return "1";
        }

        public string Post(string uri, string body)
        {
            var request = WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var bodyBytes = Encoding.UTF8.GetBytes(body);

            RequestAsync(request, bodyBytes);

            return "1";
        }

        private static void RequestAsync(WebRequest request, byte[] bodyBytes)
        {
            request.BeginGetRequestStream((r1) =>
            {
                try
                {
                    var stream = request.EndGetRequestStream(r1);
                    stream.BeginWrite(bodyBytes, 0, bodyBytes.Length, (r2) =>
                    {
                        try
                        {
                            stream.EndWrite(r2);
                            stream.Dispose();

                            request.BeginGetResponse((r3) =>
                            {
                                try
                                {
                                    using (var response = request.EndGetResponse(r3))
                                    {
                                        using (var reader = new StreamReader(response.GetResponseStream()))
                                        {
                                            var result = reader.ReadToEnd();
                                        }
                                    }
                                }
                                catch { }
                            }, null);
                        }
                        catch { }
                    }, null);
                }
                catch { }
            }, null);
        }
    }

}
