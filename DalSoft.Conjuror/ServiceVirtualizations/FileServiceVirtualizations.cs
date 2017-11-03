using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace DalSoft.Conjuror.ServiceVirtualizations
{
    public static class FileServiceVirtualization
    {
        public static void UseFileServiceVirtualization(this HttpServiceVirtualizations httpServiceVirtualizations, string contentType, string binPath, string ext)
        {
            var binFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            httpServiceVirtualizations["*"] = new HttpServiceVirtualization
            (
                async context =>
                {
                    var httpContext = context; //Access to modified closure
                    var staticVirtualResponse = $"{binFolder}\\{binPath.Trim("\\".ToCharArray()).Trim("/".ToCharArray())}\\" +
                                                $"{httpContext.Request.Path}/{httpContext.Request.Method}.{ext}";

                    if (!File.Exists(staticVirtualResponse))
                        return false;
                    
                    var json = File.ReadAllText(staticVirtualResponse); //todocahce
                    httpContext.Response.Headers["Content-Type"] = contentType;
                    await httpContext.Response.WriteAsync(json);

                    return true;
                }
            );
        }
    }
}
