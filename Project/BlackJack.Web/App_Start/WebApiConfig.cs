using BlackJack.Exceptions.Interfaces;
using BlackJack.Web.Filters;
using Newtonsoft.Json.Serialization;
using System.Web.Http;
using System.Linq;

namespace BlackJack.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            object loggerObject = config.DependencyResolver.GetServices(typeof(IExceptionFileLogger)).FirstOrDefault();

            IExceptionFileLogger logger = loggerObject as IExceptionFileLogger;

            config.Filters.Add(new ExceptionLoggerFilter(logger));
        }
    }
}
