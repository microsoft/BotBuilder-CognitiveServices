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
    /// LUIS prebuilt datetime types.
    /// https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-reference-prebuilt-entities
    /// </summary>
    /// <see cref="BuiltInGeographyTypes"/>
    [Obsolete("DateTime is deprecated. It is replaced by DateTimeV2 (BuiltInDatetimeV2Types).")]
    public class BuiltInDatetimeTypes
    {
        public const string Date = "builtin.datetime.date";

        public const string Time = "builtin.datetime.time";

        public const string Duration = "builtin.datetime.duration";

        public const string Set = "builtin.datetime.set";

        public string DateType
        {
            get { return Date; }
        }

        public string DurationType
        {
            get { return Duration; }
        }

        public string SetType
        {
            get { return Set; }
        }

        public string TimeType
        {
            get { return Time; }
        }
    }
}
