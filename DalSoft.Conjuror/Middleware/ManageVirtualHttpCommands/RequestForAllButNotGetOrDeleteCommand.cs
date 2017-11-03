using System.Threading.Tasks;
using DalSoft.Conjuror.Extensions;
using DalSoft.Conjuror.HttpPipeline;
using Microsoft.AspNetCore.Http;

namespace DalSoft.Conjuror.Middleware.ManageVirtualHttpCommands
{
    internal class RequestForAllButNotGetOrDeleteCommand : HttpCommand
    {
        protected override async Task<bool> IsCommandFor(HttpContext httpContext)
        {
            await Task.CompletedTask;
            return httpContext.IsRequestForAll();
        }

        protected override async Task<bool> Validate(HttpContext httpContext)
        {
            await Task.CompletedTask;
            return true;
        }

        protected override Task Handle(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 404;    
            return Task.CompletedTask;
        }
    }
}