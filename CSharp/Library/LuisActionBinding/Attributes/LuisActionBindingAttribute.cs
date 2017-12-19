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
    using System;

    /// <summary>
    /// Attribute for defining a LUIS Action Binding.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class LuisActionBindingAttribute : Attribute
    {
        /// <summary>
        /// Construct the LUIS Action Binding.
        /// </summary>
        /// <param name="intentName">The LUIS Intent name that triggers this action.</param>
        public LuisActionBindingAttribute(string intentName)
        {
            if (string.IsNullOrWhiteSpace(intentName))
            {
                throw new ArgumentException(nameof(intentName));
            }

            this.IntentName = intentName;

            // setting defaults
            this.CanExecuteWithNoContext = true;
            this.ConfirmOnSwitchingContext = true;

            this.FriendlyName = this.IntentName;
        }

        /// <summary>
        /// Indicates if the action can be executed without previous context. This is recommended for sub-actions that can change some variable of the current context.
        /// Default value is True.
        /// </summary>
        public bool CanExecuteWithNoContext { get; set; }

        /// <summary>
        /// Should ask user before switching action and discarding current action's context.
        /// </summary>
        public bool ConfirmOnSwitchingContext { get; set; }

        /// <summary>
        /// Text used when asking to switch context.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// The LUIS Intent name that triggers this action.
        /// </summary>
        public string IntentName { get; private set; }
    }
}
