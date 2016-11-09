using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using System;
using System.Reflection;
using System.Web.Http;
using QnAMakerSampleBot.Dialogs;
using Microsoft.Bot.Builder.CognitiveServices.QnAMakerDialog;

namespace QnAMakerSampleBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            {
                // http://docs.autofac.org/en/latest/integration/webapi.html#quick-start
                var builder = new ContainerBuilder();
                builder.RegisterModule(new DialogModule());
                builder.RegisterType<DummyEchoDialog>().As<IDialog<object>>().InstancePerDependency();
                var config = System.Web.Http.GlobalConfiguration.Configuration;
                builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
                builder.RegisterWebApiFilterProvider(config);
                var container = builder.Build();
                config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            }
     
            // WebApiConfig stuff
            GlobalConfiguration.Configure(config =>
            {
                config.MapHttpAttributeRoutes();
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );
            });
        }

        public static ILifetimeScope FindContainer()
        {
            var config = GlobalConfiguration.Configuration;
            var resolver = (AutofacWebApiDependencyResolver)config.DependencyResolver;
            return resolver.Container;
        }
    }
}
