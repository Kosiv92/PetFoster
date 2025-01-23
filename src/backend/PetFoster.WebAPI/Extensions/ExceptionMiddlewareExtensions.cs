using PetFoster.WebAPI.Middlewares;

namespace PetFoster.WebAPI.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app) 
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
