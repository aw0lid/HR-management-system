using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;


        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var argument = context.ActionArguments.Values.FirstOrDefault();
            if (argument == null)
            {
                await next();
                return;
            }

           
            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            var validator = _serviceProvider.GetService(validatorType) as IValidator;

            if (validator != null)
            {
               
                var validationContext = new ValidationContext<object>(argument);
                var validationResult = await validator.ValidateAsync(validationContext);

                if (!validationResult.IsValid)
                {
                    var firstError = validationResult.Errors.First();
                    
                    var fieldName = firstError.PropertyName;
                    var camelCaseField = JsonNamingPolicy.CamelCase.ConvertName(fieldName);
                    
                    var errorResponse = new
                    {
                        IsSuccess = false,
                        Error = new
                        {
                            Key = firstError.ErrorCode,
                            Type = 1,
                            Args = new[] { camelCaseField }
                        }
                    };

                    context.Result = new BadRequestObjectResult(errorResponse);
                    return;
                }
            }

            await next();
        }
    }
}