using Newtonsoft.Json;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;

namespace Dogey.Scripting
{
    public class HttpFunctions : ScriptObject
    {
        public HttpFunctions(HttpClient http)
        {
            var httpFuncs = new ScriptObject();

            httpFuncs.Import("send", new Func<string, string, HttpResponse>((method, url) =>
            {
                var request = new HttpRequestMessage(new HttpMethod(method), url);
                var httpResponse = http.SendAsync(request).GetAwaiter().GetResult();
                var response = new HttpResponse(httpResponse);
                return response;
            }));
            httpFuncs.Import("readjson", new Func<string, dynamic>((json) =>
            {
                var obj = JsonConvert.DeserializeObject<ExpandoObject>(json);
                return obj;
            }));

            SetValue("http", httpFuncs, true);
        }
    }

    public class HttpResponse
    {
        public Dictionary<string, string> Headers { get; set; }
        public int StatusCode { get; set; }
        public string Error { get; set; }
        public string Body { get; set; }

        public HttpResponse(HttpResponseMessage msg)
        {
            Headers = msg.Headers.ToDictionary(x => x.Key, x => string.Join(";", x.Value));
            StatusCode = (int)msg.StatusCode;
            Error = msg.ReasonPhrase;
            Body = msg.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
    }
}
