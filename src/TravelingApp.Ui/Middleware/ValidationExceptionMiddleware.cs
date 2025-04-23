using System.ComponentModel.DataAnnotations;
using TravelingApp.Application.Models;

namespace TravelingApp.Ui.Middleware
{
    public class ValidationExceptionMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (FluentValidation.ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var response = new FrameworkResponse<ResponseDto>
                {
                    Data = null,
                    Count = 0,
                    Errors = ex.Errors.Select(e => new ValidationResult(e.ErrorMessage, [e.PropertyName]))
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

}
