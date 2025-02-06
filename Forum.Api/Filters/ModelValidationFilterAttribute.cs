using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Forum.Api.Filters
{
    public class ModelValidationFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger<ModelValidationFilterAttribute> _logger;

        public ModelValidationFilterAttribute(ILogger<ModelValidationFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                _logger.LogWarning("Model validation failed: {Errors}", errors);

                context.Result = new BadRequestObjectResult(new
                {
                    Message = "Model validation error!",
                    Errors = errors
                });
            }
        }
    }
}