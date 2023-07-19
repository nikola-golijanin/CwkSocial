using CwkSocial.Api.Contracts.Common;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1;

public class BaseController : ControllerBase
{
    protected IActionResult HandleErroroResponse(List<Error> errors)
    {
        //TODO implement support for other types of Error Codes
        var apiError = new ErrorResponse();
        
        if (errors.Any(e => e.Code == ErrorCode.NotFound))
        {
            var error = errors.FirstOrDefault(e => e.Code == ErrorCode.NotFound);

            apiError.StatusCode = 404;
            apiError.StatusPhrase = "Not found";
            apiError.Timestamp = DateTime.Now;
            apiError.Errors.Add(error.Message);

            return NotFound(apiError);
        }

        apiError.StatusCode = 500;
        apiError.StatusPhrase = "Internal server error";
        apiError.Timestamp = DateTime.Now;
        errors.ForEach(e =>
        {
            apiError.Errors.Add(e.Message);
            
        });
        return StatusCode(500, apiError);
    }
}