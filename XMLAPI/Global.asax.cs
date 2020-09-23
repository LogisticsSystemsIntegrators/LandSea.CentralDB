using System.Web.Http;
using System.Web.Mvc;

namespace XMLAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);

            GlobalConfiguration.Configuration.Filters.Add(new UnhandledExceptionFilter());
            GlobalConfiguration.Configuration.Filters.Add(new TokenValidationFilter());

        }
    }
}
