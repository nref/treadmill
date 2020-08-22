using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Treadmill.Domain.Services
{
    public interface IHttpService
    {
        DateTime LastMessage { get; }
        Task Serve(Dictionary<string, Action<string>> callbacks, CancellationToken ct = default);
    }

    public class HttpService : IHttpService
    {
        public DateTime LastMessage { get; private set; } = DateTime.UtcNow;
        private readonly HttpListener _httpListener = new HttpListener();
        private readonly string _url;

        public HttpService(string url)
        {
            _url = url;
        }

        public async Task Serve(Dictionary<string, Action<string>> callbacks, CancellationToken ct = default)
        {
            _httpListener.Prefixes.Add(_url);
            _httpListener.Start();

            while (!ct.IsCancellationRequested)
            {
                var context = await _httpListener.GetContextAsync();
                LastMessage = DateTime.UtcNow;
                var json = await Read(context.Request);

                var callback = callbacks.FirstOrDefault(kvp =>
                    context.Request.Url.AbsoluteUri.EndsWith(kvp.Key));

                if (!callback.Equals(default(KeyValuePair<string, Action<string>>)))
                {
                    callback.Value(json);
                }

                await Write(context.Response, HttpStatusCode.OK, "ok");
            }
        }

        private async Task<string> Read(HttpListenerRequest request)
        {
            var reader = new StreamReader(request.InputStream);
            var ret = await reader.ReadToEndAsync();
            reader.Close();
            return ret;
        }

        private async Task Write(HttpListenerResponse response, HttpStatusCode code, string responseString)
        {
            response.StatusCode = (int)code;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
