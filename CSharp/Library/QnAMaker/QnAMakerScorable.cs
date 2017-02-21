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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Internals.Scorables;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Builder.CognitiveServices.QnAMaker
{
    /// <summary>
    /// A scorable specialized to handle QnA response from QnA Maker.
    /// </summary>
    public class QnAMakerScorable : IScorable<IActivity, double>
    {
        protected readonly QnAMakerServiceScorable inner;
        protected readonly ITraits<double> traits;
        protected readonly QnAMakerAttribute qnaInfo;

        /// <summary>
        /// Construct the QnA Scorable.
        /// </summary>
        public QnAMakerScorable(QnAMakerServiceScorable inner, ITraits<double> traits, QnAMakerAttribute qnaInfo)
        {
            SetField.NotNull(out this.inner, nameof(inner), inner);
            SetField.NotNull(out this.traits, nameof(traits), traits);
            SetField.NotNull(out this.qnaInfo, nameof(qnaInfo), qnaInfo);
        }

        async Task<object> IScorable<IActivity, double>.PrepareAsync(IActivity item, CancellationToken token)
        {
            var result = await this.inner.PrepareAsync(item, token);

            if (result is QnAMakerResult)
            {
                var qnaMakerResult = (QnAMakerResult)result;

                qnaMakerResult.Score /= 100;
                
                if (qnaMakerResult.Score == 0 & qnaInfo.ScoreThreshold == 0)
                {
                    qnaMakerResult.Answer = qnaInfo.DefaultMessage;
                }
                
                return qnaMakerResult.Score >= qnaInfo.ScoreThreshold ? qnaMakerResult : null;
            }

            return result;
        }

        public bool HasScore(IActivity item, object state)
        {
            return this.inner.HasScore(item, state);
        }

        public double GetScore(IActivity item, object state)
        {
            return this.traits.Minimum + ((this.traits.Maximum - this.traits.Minimum) * this.inner.GetScore(item, state).Score);
        }

        async Task IScorable<IActivity, double>.PostAsync(IActivity item, object state, CancellationToken token)
        {
            await this.inner.PostAsync(item, state, token);
        }

        public Task DoneAsync(IActivity item, object state, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
