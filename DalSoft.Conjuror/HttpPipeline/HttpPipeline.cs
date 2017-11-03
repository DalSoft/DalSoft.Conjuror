using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DalSoft.Conjuror.HttpPipeline
{
    internal class HttpPipeline
    {
        private HttpCommand _root;

        internal HttpPipeline Register(HttpCommand httpCommand)
        {
            if (_root == null)
                _root = httpCommand;
            else
                _root.Register(httpCommand);
            
            return this;
        }

        internal Task Execute(HttpContext context)
        {
            return _root.Execute(context);
        }
    }
}