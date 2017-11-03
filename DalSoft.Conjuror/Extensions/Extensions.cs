using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DalSoft.Conjuror.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace DalSoft.Conjuror.Extensions
{
    public static class Extensions
    {
        /// <summary>Allows you to inject pipeline i.e. middleware as a service when started up from a different project e.g. a test project.</summary>
        public static void UseInjectedApplicationBuilder(this IApplicationBuilder app)
        {
            var testIsolationPipeline = app.ApplicationServices.GetService<Func<IApplicationBuilder, IApplicationBuilder>>();
            testIsolationPipeline?.Invoke(app); //Simple way of injecting a test pipeline your welcome by the way
        }

        internal static bool IsRequestForAll(this HttpContext context)
        {
            return context.Request.Path.ToString().ToLower() == HttpServiceVirtualizationMiddleware.VirtualHttp 
                || context.Request.Path.ToString().ToLower() == $"{HttpServiceVirtualizationMiddleware.VirtualHttp}/";
        }

        internal static async Task<string> GetRequestBody(this HttpContext context)
        {
            context.Request.EnableRewind();
            string result;

            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true)) result = await reader.ReadToEndAsync();

            context.Request.Body.Position = 0;
            return result;
        }

        internal static string GetVirtualPath(this HttpContext context)
        {
            return "/" + context.Request.Path.ToString().ToLower().Replace(HttpServiceVirtualizationMiddleware.VirtualHttp, string.Empty).Trim("/".ToCharArray());
        }

        internal static HttpServiceVirtualization GetVirtualRequestOr404(this HttpContext context)
        {
            if (HttpServiceVirtualizationMiddleware.HttpServiceVirtualizations.ContainsKey(GetVirtualPath(context)))
                return HttpServiceVirtualizationMiddleware.HttpServiceVirtualizations[GetVirtualPath(context)];

            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            return null;
        }
    }
}
