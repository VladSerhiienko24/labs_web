using Autofac.Integration.WebApi;
using BlackJack.BusinessLogic.Configurations;
using System.Reflection;
using System.Web.Http;

namespace BlackJack.Web
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new AutofacServiceConfig().ConfigureContainer();

            var config = GlobalConfiguration.Configuration;

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterWebApiFilterProvider(config);

            builder.RegisterWebApiModelBinderProvider();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}