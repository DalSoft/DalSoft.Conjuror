using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace DalSoft.Conjuror
{
    public class VirtualRequest
    {
        public VirtualRequest()
        {
            Headers = new Dictionary<string, StringValues>();
        }

        public IDictionary<string, StringValues> Headers { get; set; }
        public object RequestBody { get; set; }
        public HttpMethodEnum? HttpMethod { get; set; }
    }
}

