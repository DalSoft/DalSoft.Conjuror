using System.Threading.Tasks;
using DalSoft.Conjuror.Extensions;
using DalSoft.Conjuror.HttpPipeline;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DalSoft.Conjuror.Middleware.ManageVirtualHttpCommands
{
    internal class GetAllCommand : HttpCommand
    {
        protected override async Task<bool> IsCommandFor(HttpContext httpContext)
        {
            await Task.CompletedTask;
            return httpContext.Request.Method.ToUpper() == "GET" && httpContext.IsRequestForAll();
        }

        protected override async Task<bool> Validate(HttpContext httpContext)
        {
            await Task.CompletedTask;
            return true;
        }

        protected override Task Handle(HttpContext httpContext)
        {
            return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { values = "TODO:" }));
        }
    }
}
