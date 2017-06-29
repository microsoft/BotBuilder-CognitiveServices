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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Bot.Builder.CognitiveServices.QnAMaker
{
    [Serializable]
    public class QnAMakerResults
    {
        public QnAMakerResults() { }

        public QnAMakerResults(List<QnAMakerResult> qnaMakerResults)
        {
            Answers = qnaMakerResults;
        }

        [JsonProperty(PropertyName = "answers")]
        public List<QnAMakerResult> Answers { get; set; }

        /// <summary>
        /// Internal member with instance used to configure the service that returned this result.
        /// </summary>
        internal QnAMakerAttribute ServiceCfg { get; set; }
    }

    [Serializable]
    public class QnAMakerResult
    {
        /// <summary>
        /// Initializes a new instance of the QnAResult class.
        /// </summary>
        public QnAMakerResult() { }

        /// <summary>
        /// Initializes a new instance of the QnAResult class.
        /// </summary>
        public QnAMakerResult(string answer, List<string> questions, double score)
        {
            Answer = answer;
            Questions = questions;
            Score = score;
        }

        /// <summary>
        /// The list of questions indexed in the QnA Service for the given answer.
        /// </summary>
        [JsonProperty(PropertyName = "questions")]
        public List<string> Questions { get; set; }

        /// <summary>
        /// The top answer found in the QnA Service.
        /// </summary>
        [JsonProperty(PropertyName = "answer")]
        public string Answer { get; set; }

        /// <summary>
        /// The score in range [0, 100] corresponding to the top answer found in the QnA Service.
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }
    }
}
