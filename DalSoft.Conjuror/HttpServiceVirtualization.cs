using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DalSoft.Conjuror
{
    public class HttpServiceVirtualization
    {
        public HttpServiceVirtualization(Func<HttpContext, Task<bool>>  virtualHttpContextDelegate)
        {
            VirtualHttpContextDelegate = virtualHttpContextDelegate;
        }

        public Func<HttpContext, Task<bool>> VirtualHttpContextDelegate { get; }
        public VirtualRequest LastRequest { get; set; }
    }

    public class HttpServiceVirtualizations : ConcurrentDictionary<string, HttpServiceVirtualization>
    {

    }
}