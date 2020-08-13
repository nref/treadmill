using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Treadmill.Hosting.Attributes;

namespace Treadmill.Hosting.Extensions
{
    public static class HttpListenerExtensions
    {
        public static async Task<object[]> GetObjects(this HttpListenerContext context, MethodInfo action)
        {
            return (await context.GetQueryData(action))
                .Concat(await context.GetRouteData(action))
                .Concat(await context.GetBodyData(action))
                .ToDictionary
                (
                    kvp => kvp.Key,
                    kvp => kvp.Key.ParameterType.IsPrimitive
                        ? kvp.Value.ChangeType(kvp.Key.ParameterType)
                        : JsonSerializer.Deserialize(kvp.Value, kvp.Key.ParameterType)
                )
                .OrderBy(kvp => kvp.Key.Position)
                .Select(kvp => kvp.Value)
                .ToArray();
        }

        /// <summary>
        /// Map query parameters to [FromQuery] parameters
        /// 
        /// <para/>
        /// The query parameters must map to the method parameters. 
        /// Extra parameters are allowed.
        /// 
        /// <para/>
        /// Example:
        /// 
        /// <code>
        /// someroute/test?a=1&amp;b=2&amp;c=3
        /// </code>
        /// 
        /// maps to
        /// 
        /// <code>
        /// SomeController.Test(a = 1, b = 2)
        /// </code>
        /// 
        /// (the query parameter "c" is ignored) while
        /// 
        /// <code>
        /// someroute/test?a=1
        /// </code>
        /// 
        /// fails for
        /// 
        /// <code>
        /// SomeController.Test(a = 1, b = 2)
        /// </code>
        /// 
        /// because b is not specified.
        /// </summary>
        public static async Task<Dictionary<ParameterInfo, string>> GetQueryData(this HttpListenerContext context, MethodInfo action)
        {
            var data = new Dictionary<ParameterInfo, string>();

            IEnumerable<ParameterInfo> parameters = action
                .GetParameters()
                .Where(p => p.GetCustomAttributes<FromQueryAttribute>().Any());


            data = parameters
                .Where(pi => context.Request.QueryString[pi.Name] != null)
                .ToDictionary
                (
                    pi => pi,
                    pi => context.Request.QueryString[pi.Name]
                );

            if (data.Count() < parameters.Count())
            {
                throw new ArgumentException("The query string does not contain data for each [FromQuery] parameter");
            }

            await Task.CompletedTask;
            return data;
        }

        /// <summary>
        /// Map the last segment != first to the [RouteData] parameter if there is one
        /// 
        /// <para/>
        /// Only 0 or 1 uses of [RouteData] per method are allowed.
        /// </summary>
        public static async Task<Dictionary<ParameterInfo, string>> GetRouteData(this HttpListenerContext context, MethodInfo action)
        {
            var data = new Dictionary<ParameterInfo, string>();

            IEnumerable<ParameterInfo> parameters = action
                .GetParameters()
                .Where(p => p.GetCustomAttributes<FromRouteAttribute>().Any());

            if (parameters.Count() > 1)
            {
                throw new ArgumentException("A controller action may have up to 1 use of [FromRoute]");
            }

            if (parameters.Count() == 1)
            {
                string lastSegment = context.Request.Url.Segments.Length > 1
                    ? context.Request.Url.Segments.Last()
                    : default;

                data[parameters.First()] = lastSegment
                    ?? throw new ArgumentException("No data found on the route for controller action with [FromRoute] parameter");
            }

            await Task.CompletedTask;
            return data;
        }

        /// <summary>
        /// Map the HTTP request body to the [FromBody] parameter if there is one.
        /// 
        /// <para/>
        /// Only 0 or 1 uses of [FromBody] per method are allowed.
        /// </summary>
        public static async Task<Dictionary<ParameterInfo, string>> GetBodyData(this HttpListenerContext context, MethodInfo action)
        {
            var data = new Dictionary<ParameterInfo, string>();

            IEnumerable<ParameterInfo> parameters = action
                .GetParameters()
                .Where(p => p.GetCustomAttributes<FromBodyAttribute>().Any());

            if (parameters.Count() > 1)
            {
                throw new ArgumentException("A controller action may have up to 1 use of [FromBody]");
            }

            if (parameters.Count() == 1)
            {
                var body = await context.Request.Read();

                if (string.IsNullOrWhiteSpace(body))
                {
                    throw new ArgumentException("No data from in the body for controller action with [FromBody] parameter");
                }

                data[parameters.First()] = body;
                Console.WriteLine($"Body: {body}");
            }

            return data;
        }

        public static async Task<string> Read(this HttpListenerRequest request)
        {
            var reader = new StreamReader(request.InputStream);
            var ret = await reader.ReadToEndAsync();
            reader.Close();
            return ret;
        }

        public static async Task Write(this HttpListenerResponse response, HttpStatusCode code, string responseString)
        {

            response.StatusCode = (int)code;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }

}
