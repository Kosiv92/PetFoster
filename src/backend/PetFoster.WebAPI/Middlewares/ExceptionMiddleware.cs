﻿using PetFoster.Domain.Shared;
using PetFoster.WebAPI.DTO.Responses;

namespace PetFoster.WebAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
            => (_next, _logger) = (next, logger);         
        

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                var error = Error.Failure("server.internal.error", ex.Message);
                var envelope = Envelope.Error(error.ToErrorList());

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }

    }
}
