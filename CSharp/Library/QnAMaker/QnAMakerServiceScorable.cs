﻿// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Bot Framework: http://botframework.com
// 
// Cognitive Services based Dialogs for Bot Builder:
// https://github.com/Microsoft/BotBuilder-CognitiveServices 
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Microsoft.Bot.Builder.CognitiveServices.QnAMaker
{
    using Connector;
    using Dialogs;
    using Dialogs.Internals;
    using Internals.Fibers;
    using Scorables;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// A scorable specialized to handle QnA response from QnA Maker service.
    /// </summary>
    public class QnAMakerServiceScorable : IScorable<IActivity, QnAMakerResult>
    {
        /// <summary>The QnA Maker service</summary>
        protected readonly IQnAService service;
        /// <summary>The bot to user object used to communicate back to the user</summary>
        protected readonly IBotToUser botToUser;

        /// <summary>
        /// Construct the QnA Scorable.
        /// </summary>
        public QnAMakerServiceScorable(IQnAService service, IBotToUser botToUser)
        {
            SetField.NotNull(out this.service, nameof(service), service);
            SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
        }

        /// <summary>
        /// Perform some asynchronous work to analyze the item and produce some opaque state.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<object> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item as IMessageActivity;
            if (message != null && message.Text != null)
            {
                return await service.QueryServiceAsync(message.Text);
            }

            return null;
        }

        /// <summary>
        /// Returns whether this scorable wants to participate in scoring this item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool HasScore(IActivity item, object state)
        {
            return state is QnAMakerResult;
        }

        /// <summary>
        /// Gets the score for this item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public QnAMakerResult GetScore(IActivity item, object state)
        {
            return (QnAMakerResult)state;
        }

        /// <summary>
        /// If this scorable wins, this method is called.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="state"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task PostAsync(IActivity item, object state, CancellationToken token)
        {
            await botToUser.PostAsync(((QnAMakerResult)state).Answer);
        }

        /// <summary>
        /// The scoring process has completed - dispose of any scoped resources.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="state"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task DoneAsync(IActivity item, object state, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
