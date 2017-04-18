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
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Internals.Fibers;
using System.Reflection;
using System.Linq;
using System.Web;

namespace Microsoft.Bot.Builder.CognitiveServices.QnAMaker
{
    /// <summary>
    /// A dialog specialized to handle QnA response from QnA Maker.
    /// </summary>
    [Serializable]
    public class QnAMakerDialog : IDialog<IMessageActivity>
    {
        protected readonly IQnAService[] services;
        
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
            var qnaMakerResults = default(QnAMakerResults);

            if (message != null && !string.IsNullOrEmpty(message.Text))
            {
                var tasks = this.services.Select(s => s.QueryServiceAsync(message.Text)).ToArray();

                if (tasks.Any())
                {
                    var maxValue = tasks.Max(x => x.Result.Answers[0].Score);
                    qnaMakerResults = tasks.First(x => x.Result.Answers[0].Score == maxValue).Result;

                    var filteredQnAResults = new List<QnAMakerResult>();
                    foreach (var qnaMakerResult in qnaMakerResults.Answers)
                    {
                        qnaMakerResult.Score /= 100;
                        if (qnaMakerResult.Score >= qnaMakerResults.ServiceCfg.ScoreThreshold)
                        {
                            qnaMakerResult.Answer = HttpUtility.HtmlDecode(qnaMakerResult.Answer);
                            filteredQnAResults.Add((qnaMakerResult));
                        }
                    }

                    qnaMakerResults.Answers = filteredQnAResults;

                    if (qnaMakerResults != null && qnaMakerResults.Answers != null && qnaMakerResults.Answers.Count > 0)
                    {
                        if (this.IsConfidentAnswer(qnaMakerResults))
                        {
                            await this.RespondFromQnAMakerResultAsync(context, message, qnaMakerResults);
                            await this.DefaultWaitNextMessageAsync(context, message, qnaMakerResults);
                        }
                        else
                        {
                            var feedbackRecord = new FeedbackRecord { UserId = message.From.Id, UserQuestion = message.Text };
                            context.PrivateConversationData.SetValue("qnaMakerResults", qnaMakerResults);
                            context.PrivateConversationData.SetValue("feedbackRecord", feedbackRecord);
                            await this.QnAFeedbackStepAsync(context, qnaMakerResults);
                        }
                    }
                    else
                    {
                        await context.PostAsync(qnaMakerResults.ServiceCfg.DefaultMessage);
                        await this.DefaultWaitNextMessageAsync(context, message, qnaMakerResults);
                    }
                }
                else
                {
                    await context.PostAsync(qnaMakerResults.ServiceCfg.DefaultMessage);
                    await this.DefaultWaitNextMessageAsync(context, message, qnaMakerResults);
                }
            }
            else
            {
                await context.PostAsync(qnaMakerResults.ServiceCfg.DefaultMessage);
                await this.DefaultWaitNextMessageAsync(context, message, qnaMakerResults);
            }
        }

        protected virtual bool IsConfidentAnswer(QnAMakerResults qnaMakerResults)
        {
            if (qnaMakerResults.Answers.Count < 2 || qnaMakerResults.Answers.FirstOrDefault().Score >= 99)
            {
                return true;
            }

            if (qnaMakerResults.Answers[0].Score - qnaMakerResults.Answers[1].Score > 20.0)
            {
                return true;
            }

            return false;
        }

        private async Task ResumeAndPostAnswer(IDialogContext context, IAwaitable<string> argument)
        {
            var selection = await argument;

            var qnaMakerResults = default(QnAMakerResults);
            context.PrivateConversationData.TryGetValue("qnaMakerResults", out qnaMakerResults);

            if (qnaMakerResults != null)
            {
                bool match = false;
                foreach (var qnaMakerResult in qnaMakerResults.Answers)
                {
                    if (qnaMakerResult.Questions[0].Equals(selection, StringComparison.OrdinalIgnoreCase))
                    {
                        context.PostAsync(qnaMakerResult.Answer);
                        match = true;

                        var feedbackRecord = default(FeedbackRecord);
                        context.PrivateConversationData.TryGetValue("feedbackRecord", out feedbackRecord);

                        if (feedbackRecord != null)
                        {
                            feedbackRecord.KbQuestion = qnaMakerResult.Questions.FirstOrDefault();
                            feedbackRecord.KbAnswer = qnaMakerResult.Answer;

                            var tasks =
                                this.services.Select(
                                    s =>
                                    s.ActiveLearnAsync(
                                        feedbackRecord.UserId,
                                        feedbackRecord.UserQuestion,
                                        feedbackRecord.KbQuestion,
                                        feedbackRecord.KbAnswer)).ToArray();
                            break;
                        }
                    }
                }
                if (!match)
                {
                    context.PostAsync(
                        "Not able to match. Please click on the options or type in the exact text from the options.");
                }
            }
            await this.DefaultWaitNextMessageAsync(context, context.Activity.AsMessageActivity(), qnaMakerResults);
        }

        protected virtual async Task QnAFeedbackStepAsync(IDialogContext context, QnAMakerResults qnaMakerResults)
        {
            var qnaList = qnaMakerResults.Answers;
            var questions = qnaList.Select(x => HttpUtility.HtmlDecode(x.Questions[0])).ToArray();

            PromptDialog.Choice(
                                context: context,
                                resume: ResumeAndPostAnswer,
                                options: questions,
                                prompt: "I've found multiple responses matching your query. Please select from the following:",
                                retry: "Please retry!! Click on the options or type in the exact text from the options.");
        }

        protected virtual async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            await context.PostAsync(result.Answers.FirstOrDefault().Answer);
        }

        protected virtual async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            context.Done(true);
        }
    }
}
