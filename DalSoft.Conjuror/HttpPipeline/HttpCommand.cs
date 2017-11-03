using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DalSoft.Conjuror.HttpPipeline
{
    internal abstract class HttpCommand
    {
        private HttpCommand _next;
        protected abstract Task<bool> IsCommandFor(HttpContext httpContext);
        protected abstract Task<bool> Validate(HttpContext httpContext);
        protected abstract Task Handle(HttpContext httpContext);

        public void Register(HttpCommand httpCommand)
        {
            if (_next == null)
                _next = httpCommand;
            else
                _next.Register(httpCommand);
        }

        internal async Task Execute(HttpContext httpContext)
        {
            if (await IsCommandFor(httpContext) && await Validate(httpContext))
            {
                await Handle(httpContext);
                return;
            }

            await _next.Execute(httpContext);
        }
    }
}
