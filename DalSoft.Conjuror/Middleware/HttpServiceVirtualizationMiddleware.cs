using System;
using System.Linq;
using System.Threading.Tasks;
using DalSoft.Conjuror.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using DalSoft.Conjuror.Middleware.ManageVirtualHttpCommands;

namespace DalSoft.Conjuror.Middleware
{
    /// <summary>
    /// If you want to run using muiliple test clients use virtualhttps/guid/
    /// </summary>
    public class HttpServiceVirtualizationMiddleware
    {
        internal const string VirtualHttp = "/virtualhttp";
        internal static readonly HttpServiceVirtualizations HttpServiceVirtualizations = new HttpServiceVirtualizations();
        private static Action<HttpServiceVirtualizations> _initialVirtualization;
        private readonly RequestDelegate _next;
        
        public HttpServiceVirtualizationMiddleware(RequestDelegate next, Action<HttpServiceVirtualizations> initialVirtualization)
        {
            _next = next;
            _initialVirtualization = initialVirtualization;

            SetupInitialServiceVirtualization();
        }
        
        public async Task Invoke(HttpContext context)
        {
            var requestBody = await context.GetRequestBody();
            
            if (context.Request.Path.ToString().StartsWith(VirtualHttp))
            {
                await new HttpPipeline.HttpPipeline()
                        .Register(new GetAllCommand())
                        .Register(new DeleteAllHttpCommand())
                        .Register(new RequestForAllButNotGetOrDeleteCommand())
                        .Register(new PutCommand())
                        .Register(new GetCommand())
                        .Register(new DeleteCommand())
                    .Execute(context);

                return;
            }
            
            foreach (var httpServiceVirtualization in HttpServiceVirtualizations)
            {
                if (httpServiceVirtualization.Key=="*" || httpServiceVirtualization.Key.ToLower() == context.GetVirtualPath())
                {
                    var hasRequestBeenVirtualizated = await httpServiceVirtualization.Value.VirtualHttpContextDelegate(context);

                    if (hasRequestBeenVirtualizated)
                    {
                        httpServiceVirtualization.Value.LastRequest = new VirtualRequest
                        {
                            Headers = context.Request.Headers,
                            RequestBody = context.Request.Headers.ContainsKey("Content-Type") && context.Request.Headers["Content-Type"].Any(_ => _.Contains("json")) ? 
                                                JsonConvert.DeserializeObject(requestBody) : requestBody
                        };

                        return;
                    }
                }
            }

            await _next(context);
        }

        internal static void SetupInitialServiceVirtualization()
        {
            HttpServiceVirtualizations.Clear();

            _initialVirtualization(HttpServiceVirtualizations);
        }
    }
}