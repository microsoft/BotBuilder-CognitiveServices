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
    /// LUIS prebuilt datetimeV2 types.
    /// https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-reference-prebuilt-entities#builtindatetimev2
    /// </summary>
    public class BuiltInDatetimeV2Types
    {
        public const string Date = "builtin.datetimeV2.date";

        public const string Time = "builtin.datetimeV2.time";

        public const string DateRange = "builtin.datetimeV2.daterange";

        public const string TimeRange = "builtin.datetimeV2.timerange";

        public const string DateTimeRange = "builtin.datetimeV2.datetimerange";

        public const string Duration = "builtin.datetimeV2.duration";

        public const string Set = "builtin.datetimeV2.se";

        public static string DateType
        {
            get { return Date; }
        }

        public static string TimeType
        {
            get { return Time; }
        }

        public static string DateRangeType
        {
            get { return DateRange; }
        }

        public static string TimeRangeType
        {
            get { return TimeRange; }
        }

        public static string DateTimeRangeType
        {
            get { return DateTimeRange; }
        }

        public static string DurationType
        {
            get { return Duration; }
        }

        public static string SetType
        {
            get { return Set; }
        }
    }
}
