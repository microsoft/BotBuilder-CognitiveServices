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
using Microsoft.Bot.Builder.Internals.Fibers;

namespace Microsoft.Bot.Builder.CognitiveServices.QnAMaker
{
    /// <summary>
    /// The QnA Knowledgebase information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [Serializable]
    public class QnAMakerAttribute : Attribute
    {
        /// <summary>
        /// The Subscription Key to access the QnA Knowledge Base.
        /// </summary>
        public readonly string SubscriptionKey;
        
        /// <summary>
        /// The QnA Knowledgebase ID.
        /// </summary>
        public readonly string KnowledgebaseId;

        /// <summary>
        /// The default message returned when no match found.
        /// </summary>
        public readonly string DefaultMessage;

        /// <summary>
        /// The threshold for answer score.
        /// </summary>
        public readonly double ScoreThreshold;

        /// <summary>
        /// Maximum number of answers.
        /// </summary>
        public readonly int Top;

        /// <summary>
        /// Construct the QnA Knowledgebase information.
        /// </summary>
        /// <param name="knowledgebaseId">The QnA Knowledgebase ID.</param>
        /// <param name="defaultMessage">The default message returned when no match found.</param>
        /// <param name="scoreThreshold">The threshold for answer score.</param>
        /// <param name="top">The number of answers to return.</param>
        public QnAMakerAttribute(string subscriptionKey, string knowledgebaseId, string defaultMessage = null, double scoreThreshold = 0.3, int top = 1)
        {
            if (string.IsNullOrEmpty(defaultMessage))
            {
                defaultMessage = Resource.Resource.defaultMessage;
            }
            SetField.NotNull(out this.SubscriptionKey, nameof(subscriptionKey), subscriptionKey);
            SetField.NotNull(out this.KnowledgebaseId, nameof(knowledgebaseId), knowledgebaseId);
            SetField.NotNull(out this.DefaultMessage, nameof(defaultMessage), defaultMessage);
            this.ScoreThreshold = scoreThreshold;
            this.Top = top;
        }
    }
}
