using CwkSocial.Api.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CwkSocial.Api.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var apiError = new ErrorResponse();
    
            apiError.StatusCode = 422;
            apiError.StatusPhrase = "Unprocessable Entity, check your input data again";
            apiError.Timestamp = DateTime.Now;
            foreach (var modelState in context.ModelState.Values) {
                foreach (var error in modelState.Errors) {
                    apiError.Errors.Add(error.ErrorMessage);
                }
            }
            
            context.Result = new UnprocessableEntityObjectResult(apiError);           
        }
    }
}