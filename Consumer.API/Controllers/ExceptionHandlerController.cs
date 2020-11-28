using Consumer.Api.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Consumer.Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ExceptionHandlerController : ControllerBase
    {
        [Route("error")]
        public ErrorResponse Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;
            int errorCode = 500;
            if (exception is HttpStatusException httpException)
                errorCode = (int)httpException.Status;
            Response.StatusCode = errorCode;
            return new ErrorResponse(exception);
        }
    }
}