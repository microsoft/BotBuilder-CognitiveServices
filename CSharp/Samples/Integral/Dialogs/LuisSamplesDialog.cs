namespace IntegralSampleBot.Dialogs
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Builder.CognitiveServices.LuisActionBinding.Bot;
    using LuisActions.Samples;
    using LuisActions.Samples.Models;

    [Serializable]
    public class LuisSamplesDialog : LuisActionDialog<object>
    {
        public LuisSamplesDialog() : base(
            new Assembly[] { typeof(FindHotelsAction).Assembly },
            new LuisService(new LuisModelAttribute(ConfigurationManager.AppSettings["LuisApplicationId"], ConfigurationManager.AppSettings["LuisSubscriptionKey"])))
        {
        }

        [LuisIntent("FindHotels")]
        [LuisIntent("TimeInPlace")]
        public async Task IntentActionResultHandlerAsync(IDialogContext context, object actionResult)
        {
            // we know these actions return a string for their related intents,
            // although you could have individual handlers for each intent
            var message = context.MakeMessage();

            message.Text = actionResult != null ? actionResult.ToString() : "Cannot resolve your query";

            await context.PostAsync(message);
        }

        [LuisIntent("WeatherInPlace")]
        public async Task WeatherInPlaceActionHandlerAsync(IDialogContext context, object actionResult)
        {
            // we know the action for this intent returns a WeatherInfo, so we cast the result
            var weatherInfo = (WeatherInfo)actionResult;

            var message = context.MakeMessage();
            message.Text = weatherInfo != null
                ? $"The current weather in {weatherInfo.Location}, {weatherInfo.Country} is {weatherInfo.Condition} (humidity {weatherInfo.Humidity}%)"
                : "Weather information not available";

            await context.PostAsync(message);
        }

        [LuisIntent("FindAirportByCode")]
        public async Task FindAirportByCodeActionHandlerAsync(IDialogContext context, IAwaitable<IMessageActivity> message, object actionResult)
        {
            var messageText = (await message).Text;

            // we know the action for this intent returns an AirportInfo, so we cast the result
            var airportInfo = (AirportInfo)actionResult;

            var reply = context.MakeMessage();
            reply.Text = airportInfo != null
                ? $"{airportInfo.Code} corresponds to \"{airportInfo.Name}\" which is located in {airportInfo.City}, {airportInfo.Country} [{airportInfo.Location}]"
                : $"We could not find the airport for your \"{messageText}\" request, please try a different one!";

            await context.PostAsync(reply);
        }

        protected override async Task NoActionDetectedAsync(IDialogContext context, IMessageActivity message)
        {
            var reply = context.MakeMessage();
            reply.Text = $"Sorry, I did not understand \"{message.Text}\". Use sentences like \"What is the time in Miami?\", \"Search for 5 stars hotels in Barcelona\", \"Tell me the weather in Buenos Aires\", \"Location of SFO airport\".";

            await context.PostAsync(reply);
        }
    }
}