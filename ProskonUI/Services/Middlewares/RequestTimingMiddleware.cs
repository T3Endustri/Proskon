using System.Diagnostics;
using Serilog;

namespace ProskonUI.Services.Middlewares;

public sealed class RequestTimingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await next(context);
        }
        finally
        {
            sw.Stop();
            var path = context.Request.Path.Value ?? "/";
            var status = context.Response?.StatusCode;
            // gürültüyü azaltmak için statik assetleri filtrelemek isterseniz:
            if (!path.StartsWith("/_framework") && !path.StartsWith("/_content") && !path.StartsWith("/css") && !path.StartsWith("/js"))
            {
                Log.Information("HTTP {Method} {Path} -> {Status} in {Elapsed:0.000} ms",
                    context.Request.Method, path, status, sw.Elapsed.TotalMilliseconds);
            }
        }
    }
}
