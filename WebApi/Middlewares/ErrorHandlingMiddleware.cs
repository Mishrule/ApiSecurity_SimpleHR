using System.Net;
using System.Text.Json;
using Application.Exceptions;
using Common.Responses;
using Common.Responses.Wrappers;

namespace WebApi.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                
              var responseWrapper = await ResponseWrapper<string>.FailAsync( ex.Message );
                switch (ex)
                {
                    case CustomValidationException vex:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(responseWrapper);
                await response.WriteAsync(result);
            }
            
        }
    }
}
