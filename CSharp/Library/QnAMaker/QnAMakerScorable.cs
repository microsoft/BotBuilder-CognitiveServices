// 
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

using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Scorables;
using System.Threading;
using System;

namespace Microsoft.Bot.Builder.CognitiveServices.QnAMaker
{
    /// <summary>
    /// A scorable specialized to handle QnA response from QnA Maker.
    /// </summary>
    public class QnAMakerScorable : IScorable<IActivity, double>
    {
        protected readonly IQnAService service;
        protected readonly IBotToUser botToUser;
        protected readonly ITraits<double> traits;

        /// <summary>
        /// Construct the QnA Scorable.
        /// </summary>
        public QnAMakerScorable(IQnAService service, IBotToUser botToUser, ITraits<double> traits)
        {
            SetField.NotNull(out this.service, nameof(service), service);
            SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
            SetField.NotNull(out this.traits, nameof(traits), traits);
        }
       
        async Task<object> IScorable<IActivity, double>.PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item as IMessageActivity;
            if (message != null && message.Text != null)
            {
                var response = await service.QueryServiceAsync(message.Text);

                if (!string.IsNullOrEmpty(response.Answer) && response.Score >= 0.0)
                {
                    return response.Answer;
                }
            }
            return false;
        }

        public bool HasScore(IActivity item, object state)
        {
            return state is QnAMakerResult;
        }

        public double GetScore(IActivity item, object state)
        {
            double score = double.NaN;
            if (state is QnAMakerResult)
            {
                score = this.traits.Minimum + ((this.traits.Maximum - this.traits.Minimum) * ((QnAMakerResult)state).Score);
            }
            return score;
        }

        async Task IScorable<IActivity, double>.PostAsync(IActivity item, object state, CancellationToken token)
        {
            var qnaResult = (QnAMakerResult)state;
            await botToUser.PostAsync(qnaResult.Answer);
        }

        public Task DoneAsync(IActivity item, object state, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
