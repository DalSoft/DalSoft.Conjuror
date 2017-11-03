using System.Threading.Tasks;
using DalSoft.Conjuror.Extensions;
using DalSoft.Conjuror.HttpPipeline;
using Microsoft.AspNetCore.Http;

namespace DalSoft.Conjuror.Middleware.ManageVirtualHttpCommands
{
    internal class DeleteCommand : HttpCommand
    {
        protected override async Task<bool> IsCommandFor(HttpContext httpContext)
        {
            await Task.CompletedTask;
            return httpContext.Request.Method.ToUpper() == "DELETE";
        }

        protected override async Task<bool> Validate(HttpContext httpContext)
        {
            await Task.CompletedTask;

            var vr = httpContext.GetVirtualRequestOr404();
            return vr != null;
        }

        protected override async Task Handle(HttpContext httpContext)
        {
            await Task.CompletedTask;
            HttpServiceVirtualizationMiddleware.HttpServiceVirtualizations.TryRemove(httpContext.GetVirtualPath(), out _);
        }
    }
}
