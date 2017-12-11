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
    /// LUIS prebuilt-in types.
    /// https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-reference-prebuilt-entities
    /// </summary>
    public static class BuiltInTypes
    {
        public const string Age = "builtin.age";

        public const string Dimension = "builtin.dimension";

        public const string Email = "builtin.email";

        public const string Money = "builtin.money";

        public const string Number = "builtin.number";

        public const string Ordinal = "builtin.ordinal";

        public const string Percentage = "builtin.percentage";

        public const string Phonenumber = "builtin.phonenumber";

        public const string Temperature = "builtin.temperature";

        public const string Url = "builtin.url";

        private const string SharedPrefix = "builtin.";

        [Obsolete("DateTime is deprecated. It is replaced by DateTimeV2.")]
        public static BuiltInDatetimeTypes Datetime { get; } = new BuiltInDatetimeTypes();

        public static BuiltInDatetimeV2Types DatetimeV2 { get; } = new BuiltInDatetimeV2Types();

        public static BuiltInEncyclopediaTypes Encyclopedia { get; } = new BuiltInEncyclopediaTypes();

        public static BuiltInGeographyTypes Geography { get; } = new BuiltInGeographyTypes();

        public static bool IsBuiltInType(string type)
        {
            return type.StartsWith(SharedPrefix, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
