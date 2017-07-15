using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;

namespace QnAMakerSampleBot.Dialogs
{
    [Serializable]
    public sealed class DummyEchoDialog : IDialog<IMessageActivity>
    {
        async Task IDialog<IMessageActivity>.StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceived);
        }

        public async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> message)
        {
            var toBot = await message;
            var value = toBot.Text;
            await context.PostAsync(value.ToString());
            context.Done(value);
        }
    }
}

