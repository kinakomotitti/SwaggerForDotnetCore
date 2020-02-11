using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication
{
    public class MyCustomMiddleware1
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public MyCustomMiddleware1(RequestDelegate next, ILoggerFactory logFactory)
        {
            _next = next;
            _logger = logFactory.CreateLogger(typeof(MyCustomMiddleware1));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            _logger.LogInformation($"Request Method = {httpContext.Request.Method}");
            _logger.LogInformation($"Request Path = {httpContext.Request.Path}");
            _logger.LogInformation($"Request User = {httpContext.User.Identity.Name}");
            //次のミドルウェア呼び出し
            await _next(httpContext);
            _logger.LogInformation($"Process End");
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MyCustomMiddleware1Extensions
    {
        public static IApplicationBuilder UseMyCustomMiddleware1(this IApplicationBuilder app)
        {
            return app.UseMiddleware<MyCustomMiddleware1>();
        }
    }
}
