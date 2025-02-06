using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Forum.Api.Filters
{
    public class ExceptionHandlingFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionHandlingFilterAttribute> _logger;

        public ExceptionHandlingFilterAttribute(ILogger<ExceptionHandlingFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = "An unexpected error occurred. Please try again later.";

            switch (context.Exception)
            {
                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = context.Exception.Message;
                    break;
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "Unauthorized access.";
                    break;
            }

            _logger.LogError(context.Exception, "An error occurred: {Message}", context.Exception.Message);

            var response = new
            {
                Message = message,
                Error = context.Exception.Message
            };

            context.Result = new JsonResult(response)
            {
                StatusCode = (int)statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}