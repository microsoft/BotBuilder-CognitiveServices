namespace Microsoft.Bot.Builder.CognitiveServices.QnAMaker
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Connector;
    using Dialogs;
    using Dialogs.Internals;
    using Internals.Fibers;
    using Internals.Scorables;

    /// <summary>
    /// A scorable specialized to handle QnA response from QnA Maker service.
    /// </summary>
    public class QnAMakerServiceScorable : IScorable<IActivity, QnAMakerResult>
    {
        protected readonly IQnAService service;
        protected readonly IBotToUser botToUser;

        /// <summary>
        /// Construct the QnA Scorable.
        /// </summary>
        public QnAMakerServiceScorable(IQnAService service, IBotToUser botToUser)
        {
            SetField.NotNull(out this.service, nameof(service), service);
            SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
        }

        public async Task<object> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item as IMessageActivity;
            if (message != null && message.Text != null)
            {
                return await service.QueryServiceAsync(message.Text);
            }

            return null;
        }

        public bool HasScore(IActivity item, object state)
        {
            return state is QnAMakerResult;
        }

        public QnAMakerResult GetScore(IActivity item, object state)
        {
            return (QnAMakerResult)state;
        }

        public async Task PostAsync(IActivity item, object state, CancellationToken token)
        {
            await botToUser.PostAsync(HttpUtility.HtmlDecode(((QnAMakerResult)state).Answer));
        }

        public Task DoneAsync(IActivity item, object state, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
