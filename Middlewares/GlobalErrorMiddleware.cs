using EconomicNews_DAL.Repositories;
using System.Net;

namespace EconomicNews.Middlewares
{
    public class GlobalErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public GlobalErrorMiddleware(RequestDelegate next, ILogger<GlobalErrorMiddleware> logger, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                var path = context.Request.Path;

                // 👇 استخدم ServiceScope عشان تجيب ErrorLogData
                using (var scope = _scopeFactory.CreateScope())
                {
                    var errorLogData = scope.ServiceProvider.GetRequiredService<ErrorLogData>();
                    await errorLogData.LogAsync(path, ex.Message, ex.StackTrace ?? "");
                }

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = "Something went wrong",
                    error = ex.Message
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
