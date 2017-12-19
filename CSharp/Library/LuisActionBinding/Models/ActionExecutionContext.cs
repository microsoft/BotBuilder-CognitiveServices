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

namespace Microsoft.Bot.Builder.CognitiveServices.LuisActionBinding
{
    /// <summary>
    /// A LUIS Action Binding context.
    /// </summary>
    public class ActionExecutionContext
    {
        /// <summary>
        /// Construct the Action Context.
        /// </summary>
        /// <param name="intent">The LUIS Intent name that triggered the action.</param>
        /// <param name="action">The LUIS Action Binding model that was triggered.</param>
        public ActionExecutionContext(string intent, ILuisAction action)
        {
            this.Intent = intent;
            this.Action = action;
        }

        /// <summary>
        /// The current LUIS Action Binding model.
        /// </summary>
        public ILuisAction Action { get; private set; }


        /// <summary>
        /// The LUIS Intent name that triggers this action.
        /// </summary>
        public string Intent { get; private set; }

        /// <summary>
        /// Indicates if the action was triggered due to context switching.
        /// </summary>
        public bool ChangeRootSignaling { get; set; }
    }
}
