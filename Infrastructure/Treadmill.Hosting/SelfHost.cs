using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Treadmill.Domain;
using Treadmill.Domain.Services;
using Treadmill.Hosting.Exceptions;
using Treadmill.Hosting.Extensions;
using Treadmill.Infrastructure;

namespace Treadmill.Hosting
{
    public class SelfHost
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly ILogService _log;
        private Dictionary<string, IController> _controllers = new Dictionary<string, IController>();

        public SelfHost(ICompositionRoot root, ILogService log)
        {
            RegisterControllers(root.GetAll<IController>());
            _log = log;
        }

        public void RegisterControllers(IEnumerable<IController> controllers) 
            => _controllers = controllers.ToDictionary(c => c.RoutePrefix, c => c);

        public void Run(string uri)
        {
            _log.Add("Starting...");
            _listener.Prefixes.Add(uri);
            _listener.Start();
            _log.Add("Started");

            while (true)
            {
                var context = _listener.GetContext();

                Task.Run(async () =>
                {
                    await RunOnceSafe(context).ConfigureAwait(false);
                });
            }
        }

        private async Task RunOnceSafe(HttpListenerContext context)
        {
            await Async.SafeExec
            (
                async () =>
                {
                    _log.Add($"{context.Request.HttpMethod} {context.Request.Url}");

                    HttpReponse response;

                    try
                    {
                        response = await RunOnce(context);
                    }
                    catch (NotFoundException e)
                    {
                        response = e.Reponse;
                    }

                    _log.Add($"Response: {response.Data}");
                    await context.Response.Write(response.Code, response.Data);
                },
                async (e) => await context.Response.Write(HttpStatusCode.BadRequest, "BadRequest")
            ).ConfigureAwait(false);
        }

        private async Task<HttpReponse> RunOnce(HttpListenerContext context)
        {
            string httpMethod = context.Request.HttpMethod.ToUpper();

            if (!TryGetAction
            (
                context, 
                httpMethod, 
                out IController controller, 
                out MethodInfo action
            ))
            {
                throw new NotFoundException();
            }

            {
                object[] objects = await context.GetObjects(action);
                object obj;

                switch (httpMethod)
                {
                    case "GET":
                        obj = await action.InvokeAsync(controller, objects);
                        break;

                    case "POST":
                        await action.InvokeAsync2(controller, objects);
                        obj = "ok";
                        break;

                    default:
                        throw new NotFoundException();
                }

                return new HttpReponse { Code = HttpStatusCode.OK, Data = obj.ToString() };
            }
        }

        private bool TryGetAction
        (
            HttpListenerContext context, 
            string httpMethod, 
            out IController controller, 
            out MethodInfo action
        )
        {
            string[] segments = context.Request.Url.Segments;

            var location = segments[1].Replace("/", "");
            var resource = segments.Length > 2
                ? segments[2].Replace("/", "")
                : "";

            if (!_controllers.TryGetValue(location, out controller))
            {
                action = default;
                return false;
            }

            if (!controller.GetActions(httpMethod).TryGetValue(resource, out action))
            {
                return false;
            }

            return true;
        }
    }

}
