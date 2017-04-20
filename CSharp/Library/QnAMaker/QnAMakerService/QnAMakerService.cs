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
using System.Net.Http;

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
        /// Query the QnA service using this uri.
        /// </summary>
        /// <param name="uri">The query uri</param>
        /// <returns>The QnA service result.</returns>
        Task<QnAMakerResult> QueryServiceAsync(Uri uri, QnAMakerRequestBody postBody, string subscriptionKey);
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
        public static readonly Uri UriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0/knowledgebases");

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
            postBody = new QnAMakerRequestBody { question = queryText };
            subscriptionKey = this.qnaInfo.SubscriptionKey;
            return builder.Uri;
        }

        async Task<QnAMakerResult> IQnAService.QueryServiceAsync(Uri uri, QnAMakerRequestBody postBody, string subscriptionKey)
        {
            string json;
            
            using (HttpClient client = new HttpClient())
            {
                //Add the subscription key header
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                var response = await client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(postBody), Encoding.UTF8, "application/json"));
                json = await response?.Content?.ReadAsStringAsync();
            }

            try
            {
                var result = JsonConvert.DeserializeObject<QnAMakerResult>(json);

                //Adding internal service cfg reference [used when checking configured threshold to provide an answer]
                result.ServiceCfg = this.qnaInfo;

                return result;
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("Unable to deserialize the QnA service response.", ex);
            }
        }
    }

    public class QnAMakerRequestBody
    {
        [JsonProperty("question")]
        public string question { get; set; }
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
        public static async Task<QnAMakerResult> QueryServiceAsync(this IQnAService service, string text)
        {
            QnAMakerRequestBody postBody;
            string subscriptionKey;
            var uri = service.BuildRequest(text, out postBody, out subscriptionKey);
            return await service.QueryServiceAsync(uri, postBody, subscriptionKey);
        }
    }
}
