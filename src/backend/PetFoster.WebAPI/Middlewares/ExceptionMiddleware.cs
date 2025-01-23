using PetFoster.Domain.Shared;
using PetFoster.WebAPI.DTO.Responses;
using System.Net;

namespace PetFoster.WebAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var error = Error.Failure("server.internal.error", ex.Message);
                var envelope = Envelope.Error(error.ToErrorList());

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }

    }
}
