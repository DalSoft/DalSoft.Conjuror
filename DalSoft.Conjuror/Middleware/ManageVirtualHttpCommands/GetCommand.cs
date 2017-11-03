using System.Threading.Tasks;
using DalSoft.Conjuror.Extensions;
using DalSoft.Conjuror.HttpPipeline;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DalSoft.Conjuror.Middleware.ManageVirtualHttpCommands
{
    internal class GetCommand : HttpCommand
    {
        private HttpServiceVirtualization _virtualRequest;

        protected override async Task<bool> IsCommandFor(HttpContext httpContext)
        {
            await Task.CompletedTask;
            return httpContext.Request.Method.ToUpper() == "GET";
        }

        protected override async Task<bool> Validate(HttpContext httpContext)
        {
            await Task.CompletedTask;
            _virtualRequest = httpContext.GetVirtualRequestOr404();
            return _virtualRequest != null;
        }

        protected override async Task Handle(HttpContext context)
        {
            await context.Response.WriteAsync(JsonConvert.SerializeObject(_virtualRequest.LastRequest ?? new VirtualRequest()));
        }
    }
}
