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

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Internals.Fibers;
using System.Reflection;
using System.Linq;
using System.Web;
using System.Collections.Generic;

namespace Microsoft.Bot.Builder.CognitiveServices.QnAMaker
{
    /// <summary>
    /// A dialog specialized to handle QnA response from QnA Maker.
    /// </summary>
    [Serializable]
    public class QnAMakerDialog : IDialog<IMessageActivity>
    {
        protected readonly IQnAService[] services;

        [NonSerialized]
        protected Dictionary<QnAMakerResponseHandlerAttribute, QnAMakerResponseHandler> HandlerByMaximumScore;

        public IQnAService[] MakeServicesFromAttributes()
        {
            var type = this.GetType();
            var qnaModels = type.GetCustomAttributes<QnAMakerAttribute>(inherit: true);
            return qnaModels.Select(m => new QnAMakerService(m)).Cast<IQnAService>().ToArray();
        }

        /// <summary>
        /// Construct the QnA Service dialog.
        /// </summary>
        /// <param name="services">The QnA service.</param>
        public QnAMakerDialog(params IQnAService[] services)
        {
            if (services.Length == 0)
            {
                services = MakeServicesFromAttributes();
            }

            SetField.NotNull(out this.services, nameof(services), services);
        }

        async Task IDialog<IMessageActivity>.StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            var qnaMakerResult = default(QnAMakerResult);
            if (message != null && !string.IsNullOrEmpty(message.Text))
            {
                var tasks = this.services.Select(s => s.QueryServiceAsync(message.Text)).ToArray();

                var maxValue = tasks.Max(x => x.Result.Score);
                qnaMakerResult = await tasks.First(x => x.Result.Score == maxValue);

                if (HandlerByMaximumScore == null)
                {
                    HandlerByMaximumScore =
                        new Dictionary<QnAMakerResponseHandlerAttribute, QnAMakerResponseHandler>(GetHandlersByMaximumScore());
                }

                var applicableHandlers = HandlerByMaximumScore.OrderBy(h => h.Key.MaximumScore).Where(h => h.Key.MaximumScore > (qnaMakerResult.Score / 100));
                var handler = applicableHandlers.Any() ? applicableHandlers.First().Value : null;

                if (handler != null)
                {
                    await handler.Invoke(context, message, qnaMakerResult);
                }
                else
                {
                    await this.RespondFromQnAMakerResultAsync(context, message, qnaMakerResult);
                }
            }

            await this.DefaultWaitNextMessageAsync(context, message, qnaMakerResult);
        }

        protected virtual async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResult result)
        {
            result.Score /= 100;
            var answer = result.Score >= result.ServiceCfg.ScoreThreshold ? result.Answer : result.ServiceCfg.DefaultMessage;

            await context.PostAsync(HttpUtility.HtmlDecode(answer));
        }

        protected virtual async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResult result)
        {
            context.Wait(MessageReceivedAsync);
        }

        protected virtual IDictionary<QnAMakerResponseHandlerAttribute, QnAMakerResponseHandler> GetHandlersByMaximumScore()
        {
            return EnumerateHandlers(this).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        internal static IEnumerable<KeyValuePair<QnAMakerResponseHandlerAttribute, QnAMakerResponseHandler>> EnumerateHandlers(object dialog)
        {
            var type = dialog.GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var method in methods)
            {
                var qNaResponseHandlerAttributes = method.GetCustomAttributes<QnAMakerResponseHandlerAttribute>(inherit: true).ToArray();
                Delegate created = null;
                try
                {
                    created = Delegate.CreateDelegate(typeof(QnAMakerResponseHandler), dialog, method, throwOnBindFailure: false);
                }
                catch (ArgumentException)
                {
                    // "Cannot bind to the target method because its signature or security transparency is not compatible with that of the delegate type."
                    // https://github.com/Microsoft/BotBuilder/issues/634
                    // https://github.com/Microsoft/BotBuilder/issues/435
                }

                var qNaResponseHanlder = (QnAMakerResponseHandler)created;
                if (qNaResponseHanlder != null)
                {
                    foreach (var qNaResponseAttribute in qNaResponseHandlerAttributes)
                    {
                        if (qNaResponseAttribute != null && qNaResponseHandlerAttributes.Any())
                            yield return new KeyValuePair<QnAMakerResponseHandlerAttribute, QnAMakerResponseHandler>(qNaResponseAttribute, qNaResponseHanlder);
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class QnAMakerResponseHandlerAttribute : Attribute
    {
        public readonly double MaximumScore;

        public QnAMakerResponseHandlerAttribute(double maximumScore)
        {
            MaximumScore = maximumScore;
        }
    }

    public delegate Task QnAMakerResponseHandler(IDialogContext context, IMessageActivity message, QnAMakerResult result);
}
