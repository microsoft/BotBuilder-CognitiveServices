namespace LuisActions.Samples.Web.Controllers
{
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.CognitiveServices.LuisActionBinding;
    using Models;
    using Samples;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var emptyModel = new QueryViewModel();
            return this.View(emptyModel);
        }

        [HttpPost]
        public async Task<ActionResult> Index(QueryViewModel model)
        {
            if (!model.HasIntent)
            {
                var luisService = new LuisService(new LuisModelAttribute(ConfigurationManager.AppSettings["LuisApplicationId"], ConfigurationManager.AppSettings["LuisSubscriptionKey"]));
                var luisResult = await luisService.QueryAsync(model.Query, CancellationToken.None);
                var resolver = new LuisActionResolver(typeof(GetTimeInPlaceAction).Assembly);
                var action = resolver.ResolveActionFromLuisIntent(luisResult);

                // Triggering Contextual Action from scratch is not supported on this Web Sample
                if (action != null && !LuisActionResolver.IsContextualAction(action))
                {
                    model.LuisAction = action;

                    // Store the Action name, encrypted
                    model.LuisActionType = StringCrypto.Encrypt(action.GetType().AssemblyQualifiedName);
                }
                else
                {
                    // no action recognized
                    return this.View(new QueryViewModel());
                }
            }

            ModelState.Clear();
            var isValid = TryValidateModel(model.LuisAction);
            if (isValid)
            {
                // fulfill
                var actionResult = await model.LuisAction.FulfillAsync();
                if (actionResult == null)
                {
                    actionResult = "Cannot resolve your query";
                }

                return this.View("ActionFulfill", actionResult);
            }
            else
            {
                // not valid, continue to present form with missing/invalid parameters
                return this.View(model);
            }
        }

        public PartialViewResult ScaffoldAction(QueryViewModel query)
        {
            if (query == null || query.LuisAction == null)
            {
                return new EmptyPartialViewResult();
            }

            var modelProperties = query.LuisAction.GetType().GetProperties()
                .Where(p => p.SetMethod != null);

            var model = new ActionScaffoldViewModel()
            {
                Fields = modelProperties.Select(o => string.Format("LuisAction." + o.Name)),
                LuisAction = query.LuisAction
            };

            return this.PartialView(model);
        }
    }
}