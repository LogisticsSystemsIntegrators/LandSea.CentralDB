using System;
using System.Web.Http.Filters;
using System.Net.Http;
using BSW.APIToken;

namespace XMLAPI
{
    public class TokenValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string token = string.Empty;

            try
            {
                //first we need to check if any token was passed in the header of the request
                var authorization = actionContext.Request.Headers.Authorization;

                if (authorization == null)
                {
                    actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                    {
                        Content = new StringContent("Missing Flow-Token")
                    };
                    return;
                }
                else
                {
                    token = authorization.Parameter.ToString();
                }
            }
            catch (Exception)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("Missing Flow-Token")
                };
                return;

            }
            //we got a token - so now lets validate the actual token itself
            try
            {
                //we need to get the caller address this will be compared in the token validation
                var referrer = actionContext.Request.Headers.Referrer;
                string callerUrl = string.Empty;
                if (referrer != null)
                {
                    callerUrl = referrer.Authority;
                }

                //we also need to check if this was a original call from an ang6 application - if so bypass this validation
                if (!token.StartsWith("Ng6App"))
                {
                    using (Token t = new Token())
                    {
                        using (TokenResponce result = t.ValidateUserToken(token, callerUrl))
                        {
                            if (!result.IsTokenValid)
                            {
                                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                                {
                                    Content = new StringContent(result.Reason)
                                };
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("Invalid Flow-Token - " + exp.Message)
                };
                return;
            }
                      
            base.OnActionExecuting(actionContext);
        }
    }
}