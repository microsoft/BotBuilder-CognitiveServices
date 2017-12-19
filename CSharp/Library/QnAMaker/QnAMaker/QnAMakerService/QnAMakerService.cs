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
using Microsoft.Bot.Builder.Internals.Fibers;
using Newtonsoft.Json;
using System.Text;
using System.Web;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Bot.Builder.CognitiveServices.QnAMaker
{
    public interface IQnAService
    {
        /// <summary>
        /// Build the query uri for the query text.
        /// </summary>
        /// <param name="queryText">The query text.</param>
        /// <returns>The query uri</returns>
        Uri BuildRequest(string queryText, out QnAMakerRequestBody postBody, out string subscriptionKey);

        /// <summary>
        /// Build the feedback request uri for the query text and the selected QnA.
        /// </summary>
        /// <param name="userId">An unique ID for every user.</param>
        /// <param name="userQuery">The original user query.</param>
        /// <param name="kbQuestion">The question from the knowledgebase which corresponds to the selected answer.</param>
        /// <param name="kbAnswer">The selected answer.</param>
        /// <returns>The query uri</returns>
        Uri BuildFeedbackRequest(string userId, string userQuery, string kbQuestion, string kbAnswer, out QnAMakerTrainingRequestBody postBody, out string subscriptionKey, out string knowledgebaseId);

        /// <summary>
        /// Query the QnA service using this uri.
        /// </summary>
        /// <param name="uri">The query uri</param>
        /// <returns>The QnA service results.</returns>
        Task<QnAMakerResults> QueryServiceAsync(Uri uri, QnAMakerRequestBody postBody, string subscriptionKey);

        /// <summary>
        /// Sends the feedback entry the QnA service.
        /// </summary>
        /// <param name="uri">The train uri to record the feedback entry</param>
        /// <returns>A boolean indicating success/failure.</returns>
        Task<bool> ActiveLearnAsync(Uri uri, QnAMakerTrainingRequestBody postBody, string subscriptionKey);
    }

    /// <summary>
    /// Standard implementation of IQnAService
    /// </summary>
    [Serializable]
    public sealed class QnAMakerService : IQnAService
    {
        private readonly QnAMakerAttribute qnaInfo;

        /// <summary>
        /// The base URI for accessing QnA Service.
        /// </summary>
        public static readonly Uri UriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases");

        /// <summary>
        /// Construct the QnA service using the qnaInfo information.
        /// </summary>
        /// <param name="qnaInfo">The QnA knowledgebase information.</param>
        public QnAMakerService(QnAMakerAttribute qnaInfo)
        {
            SetField.NotNull(out this.qnaInfo, nameof(qnaInfo), qnaInfo);
        }

        Uri IQnAService.BuildRequest(string queryText, out QnAMakerRequestBody postBody, out string subscriptionKey)
        {
            var knowledgebaseId = this.qnaInfo.KnowledgebaseId;
            var builder = new UriBuilder($"{UriBase}/{knowledgebaseId}/generateanswer");
            postBody = new QnAMakerRequestBody { question = queryText, top = this.qnaInfo.Top };
            subscriptionKey = this.qnaInfo.SubscriptionKey;
            return builder.Uri;
        }

        Uri IQnAService.BuildFeedbackRequest(string userId, string userQuery, string kbQuestion, string kbAnswer, out QnAMakerTrainingRequestBody postBody, out string subscriptionKey, out string knowledgebaseId)
        {
            knowledgebaseId = this.qnaInfo.KnowledgebaseId;
            subscriptionKey = this.qnaInfo.SubscriptionKey;
            var builder = new UriBuilder($"{UriBase}/{knowledgebaseId}/train");
            var feedbackRecord = new FeedbackRecord { UserId = userId, UserQuestion = userQuery, KbQuestion = kbQuestion, KbAnswer = kbAnswer };
            var feedbackRecords = new List<FeedbackRecord>();
            feedbackRecords.Add(feedbackRecord);
            postBody = new QnAMakerTrainingRequestBody { KnowledgeBaseId = knowledgebaseId, FeedbackRecords = feedbackRecords };
            return builder.Uri;
        }

        async Task<QnAMakerResults> IQnAService.QueryServiceAsync(Uri uri, QnAMakerRequestBody postBody, string subscriptionKey)
        {
            string json = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                //Add the subscription key header
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                var response = await client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(postBody), Encoding.UTF8, "application/json"));
                if (response != null && response.Content != null)
                {
                    json = await response.Content.ReadAsStringAsync();
                }
            }

            try
            {
                var qnaMakerResults = JsonConvert.DeserializeObject<QnAMakerResults>(json);

                //Adding internal service cfg reference [used when checking configured threshold to provide an answer]
                qnaMakerResults.ServiceCfg = this.qnaInfo;

                var filteredQnAResults = new List<QnAMakerResult>();
                foreach (var qnaMakerResult in qnaMakerResults.Answers)
                {
                    qnaMakerResult.Score /= 100;
                    if (qnaMakerResult.Score >= this.qnaInfo.ScoreThreshold)
                    {
                        qnaMakerResult.Answer = HttpUtility.HtmlDecode(qnaMakerResult.Answer);
                        qnaMakerResult.Questions = qnaMakerResult.Questions.Select(x => HttpUtility.HtmlDecode(x)).ToList();
                        filteredQnAResults.Add(qnaMakerResult);
                    }
                }

                qnaMakerResults.Answers = filteredQnAResults;

                return qnaMakerResults;
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("Unable to deserialize the QnA service response.", ex);
            }
        }

        async Task<bool> IQnAService.ActiveLearnAsync(Uri uri, QnAMakerTrainingRequestBody postBody, string subscriptionKey)
        {
            try
            {
                string json = string.Empty;

                using (HttpClient client = new HttpClient())
                {
                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), uri)
                                      {
                                          Content =
                                              new StringContent(
                                              JsonConvert
                                              .SerializeObject(
                                                  postBody),
                                              Encoding.UTF8,
                                              "application/json")
                                      };

                    //Add the subscription key header
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                    var response = await client.SendAsync(request);
                    if (response != null && response.Content != null)
                    {
                        json = await response.Content.ReadAsStringAsync();
                    }
                }
            
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class QnAMakerRequestBody
    {
        [JsonProperty("question")]
        public string question { get; set; }

        [JsonProperty("top")]
        public int top { get; set; }
    }

    public class QnAMakerTrainingRequestBody
    {
        [JsonProperty("knowledgeBaseId")]
        public string KnowledgeBaseId { get; set; }

        [JsonProperty("feedbackRecords")]
        public List<FeedbackRecord> FeedbackRecords { get; set; }
    }

    [Serializable]
    public class FeedbackRecord
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("userQuestion")]
        public string UserQuestion { get; set; }

        [JsonProperty("kbQuestion")]
        public string KbQuestion { get; set; }

        [JsonProperty("kbAnswer")]
        public string KbAnswer { get; set; }
    }

    /// <summary>
    /// QnA Service extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Query the QnA service using this text.
        /// </summary>
        /// <param name="service">QnA service.</param>
        /// <param name="text">The query text.</param>
        /// <returns>The QnA result.</returns>
        public static async Task<QnAMakerResults> QueryServiceAsync(this IQnAService service, string text)
        {
            QnAMakerRequestBody postBody;
            string subscriptionKey;
            var uri = service.BuildRequest(text, out postBody, out subscriptionKey);
            return await service.QueryServiceAsync(uri, postBody, subscriptionKey);
        }

        public static async Task<bool> ActiveLearnAsync(
            this IQnAService service,
            string userId,
            string userQuestion,
            string kbQuestion,
            string kbAnswer,
            string knowledgebaseId)
        {
            QnAMakerTrainingRequestBody postBody;
            string subscriptionKey;
            string serviceKnowledgebaseId;
            var uri = service.BuildFeedbackRequest(
                userId,
                userQuestion,
                kbQuestion,
                kbAnswer,
                out postBody,
                out subscriptionKey,
                out serviceKnowledgebaseId);

            if (serviceKnowledgebaseId.Equals(knowledgebaseId))
            {
                return await service.ActiveLearnAsync(uri, postBody, subscriptionKey);
            }

            return false;
        }
    }
}
