using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace DalSoft.Conjuror
{
    public class VirtualResponse
    {
        public VirtualResponse()
        {
            Headers = new Dictionary<string, StringValues>();
        }

        public IDictionary<string, StringValues> Headers { get; set; }
        public HttpMethodEnum? HttpMethod { get; set; }
        public object ResponseBody { get; set; }
        public int? StatusCode { get; set; }
    }
}
