using System;
using System.Configuration;
using System.Web.Http;
using Autofac;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace IntegralSampleBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Conversation.UpdateContainer(builder =>
            {
                builder.RegisterModule(new ReflectionSurrogateModule());
                builder.RegisterModule(new QnAMakerModule(
                    ConfigurationManager.AppSettings["QnAMaker_SubscriptionId"],
                    ConfigurationManager.AppSettings["QnAMaker_KnowledgeBaseId"],
                    "I don't understand this right now! Try another query!",
                    0.50));
            });

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
    }
}
