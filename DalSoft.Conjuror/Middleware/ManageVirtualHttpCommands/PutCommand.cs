using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DalSoft.Conjuror.Extensions;
using DalSoft.Conjuror.HttpPipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace DalSoft.Conjuror.Middleware.ManageVirtualHttpCommands
{
    internal class PutCommand : HttpCommand
    {
        private VirtualResponse _virtualResponse;

        protected override async Task<bool> IsCommandFor(HttpContext httpContext)
        {
            await Task.CompletedTask;
            return httpContext.Request.Method.ToUpper() == "PUT";
        }

        protected override async Task<bool> Validate(HttpContext context)
        {
            try
            {
                var body = await context.GetRequestBody();
                _virtualResponse = JsonConvert.DeserializeObject<VirtualResponse>(body);
            }
            catch
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "VirtualResponse is invalid" }));
                return false;
            }

            if (string.IsNullOrEmpty(context.GetVirtualPath()))
            {
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "Path is required" }));
                _virtualResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                return false;
            }

            return true;
        }

        protected override async Task Handle(HttpContext context)
        {
            await Task.CompletedTask;
            HttpServiceVirtualizationMiddleware.HttpServiceVirtualizations[context.GetVirtualPath()] = new HttpServiceVirtualization
            (
                async httpContext =>
                {
                    context = httpContext;

                    if (_virtualResponse.HttpMethod != null && _virtualResponse.HttpMethod.ToString() != context.Request.Method.ToString())
                        return false;

                    context.Response.StatusCode = _virtualResponse.StatusCode ?? 200;
                    _virtualResponse.Headers["Content-Type"] = _virtualResponse.Headers.ContainsKey("Content-Type") ? _virtualResponse.Headers["Content-Type"] : new StringValues("application/json");

                    foreach (var header in _virtualResponse.Headers)
                    {
                        if (context.Response.Headers.ContainsKey(header.Key))
                            context.Response.Headers.Remove(header.Key);

                        context.Response.Headers.Add(header);
                    }

                    if (_virtualResponse.Headers.ContainsKey("Content-Type") && _virtualResponse
                            .Headers["Content-Type"]
                            .Any(_ => _.Contains("json") && _virtualResponse.ResponseBody != null))
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(_virtualResponse.ResponseBody));
                    else if (_virtualResponse.ResponseBody != null)
                        await context.Response.WriteAsync(_virtualResponse.ResponseBody.ToString());


                    return true;
                }
            );

            context.Response.StatusCode = 201;
        }
    }
}
