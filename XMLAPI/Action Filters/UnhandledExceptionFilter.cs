using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace XMLAPI
{
    public class UnhandledExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;

            var exType = actionExecutedContext.Exception.GetType();

            if (exType == typeof(UnauthorizedAccessException))
                status = HttpStatusCode.Unauthorized;
            else if (exType == typeof(ArgumentException))
                status = HttpStatusCode.NotFound;

            // create a new response and attach our ApiError object
            // which now gets returned on ANY exception result
            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(status, actionExecutedContext.Exception.Message);

            base.OnException(actionExecutedContext);
        }
    }
}