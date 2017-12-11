// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Bot Framework: http://botframework.com
// 
// Bot Builder Cognitive Services Github:
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

var BuiltInTypes = {
    Age:                    'builtin.age',
    Dimension:              'builtin.dimension',
    Email:                  'builtin.email',
    Money:                  'builtin.money',
    Number:                 'builtin.number',
    Ordinal:                'builtin.ordinal',
    Percentage:             'builtin.percentage',
    PhoneNumber:            'builtin.phonenumber',
    Temperature:            'builtin.temperature',
    Url:                    'builtin.url',

    // DateTime is deprecated. It is replaced by DateTimeV2
    DateTime: {
        Date:               'builtin.datetime.date',
        Time:               'builtin.datetime.time',
        Duration:           'builtin.datetime.duration',
        Set:                'builtin.datetime.set'
    },
    DateTimeV2: {
        Date:               'builtin.datetimeV2.date',
        Time:               'builtin.datetimeV2.time',
        DateRange:          'builtin.datetimeV2.daterange',
        TimeRange:          'builtin.datetimeV2.timerange',
        DateTimeRange:      'builtin.datetimeV2.datetimerange',
        Duration:           'builtin.datetimeV2.duration',
        Set:                'builtin.datetimeV2.set'
    },
    Encyclopedia: {
        Person:             'builtin.encyclopedia.people.person',
        Organization:       'builtin.encyclopedia.organization.organization',
        Event:              'builtin.encyclopedia.time.event'
    },
    Geography: {
        City:               'builtin.geography.city',
        Country:            'builtin.geography.country',
        PointOfInterest:    'builtin.geography.pointOfInterest'
    }
};

module.exports = BuiltInTypes;