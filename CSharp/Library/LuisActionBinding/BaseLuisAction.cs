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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for implementing LUIS Action Bindings.
    /// </summary>
    [Serializable]
    public abstract class BaseLuisAction : ILuisAction
    {
        /// <summary>
        /// The method to be executed when all required parameters are filled. This method should fulfill or call any external services and return the result.
        /// </summary>
        /// <returns>The result of the action execution.</returns>
        public abstract Task<object> FulfillAsync();

        /// <summary>
        /// Validates if the current action's context is valid, based on the Action properties set so far and their validation attributes.
        /// </summary>
        /// <param name="validationResults">List of validation errors.</param>
        /// <returns>True if context is valid, False otherwise.</returns>
        public virtual bool IsValid(out ICollection<ValidationResult> validationResults)
        {
            var context = new ValidationContext(this, null, null);

            validationResults = new List<ValidationResult>();
            var result = Validator.TryValidateObject(this, context, validationResults, true);

            // do order properties
            validationResults = validationResults
                .OrderBy(r =>
                {
                    var paramAttrib = LuisActionResolver.GetParameterDefinition(this, r.MemberNames.First());

                    return paramAttrib != null ? paramAttrib.Order : int.MaxValue;
                })
                .ThenBy(r => r.MemberNames.First())
                .ToList();

            return result;
        }
    }
}
