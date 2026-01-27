using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace FanQuest.API.Middleware
{

        public class CustomRateLimitMiddleware
        {
            private readonly RequestDelegate _next;

            public CustomRateLimitMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                await _next(context);

                // Customize 429 response
                if (context.Response.StatusCode == (int)HttpStatusCode.TooManyRequests)
                {
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        error = "Rate limit exceeded",
                        message = "Too many requests. Please try again later.",
                        retryAfter = context.Response.Headers["Retry-After"].ToString()
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            }
        }
}
