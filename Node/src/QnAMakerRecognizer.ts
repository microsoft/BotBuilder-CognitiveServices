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

import * as builder from 'botbuilder';
import * as request from 'request';
import * as entities from 'html-entities';

var qnaMakerServiceEndpoint = 'https://westus.api.cognitive.microsoft.com/qnamaker/v1.0/knowledgebases/';
var qnaApi = 'generateanswer';
var htmlentities = new entities.AllHtmlEntities();

export interface IQnAMakerResult extends builder.IIntentRecognizerResult {
    answer: string;
}

export interface IQnAMakerOptions extends builder.IIntentRecognizerSetOptions {
    knowledgeBaseId: string;
    subscriptionKey: string;
    qnaThreshold?: number;
    defaultMessage?: string;
}

export class QnAMakerRecognizer implements builder.IIntentRecognizer {
    private kbUri: string;
    private ocpApimSubscriptionKey: string;
    
    constructor(private options: IQnAMakerOptions){
        this.kbUri = qnaMakerServiceEndpoint + options.knowledgeBaseId + '/' + qnaApi;
        this.ocpApimSubscriptionKey = options.subscriptionKey;
    }

    public recognize(context: builder.IRecognizeContext, cb: (error: Error, result: IQnAMakerResult) => void): void {
        var result: IQnAMakerResult = { score: 0.0, answer: null, intent: null };
        if (context && context.message && context.message.text) {
            var utterance = context.message.text;
            QnAMakerRecognizer.recognize(utterance, this.kbUri, this.ocpApimSubscriptionKey, (error, result) => {
                    if (!error) {
                        cb(null, result);
                    } else {
                        cb(error, null);
                    }
                }
            );
        }
    }

    static recognize(utterance: string, kbUrl: string, ocpApimSubscriptionKey: string, callback: (error: Error, result?: IQnAMakerResult) => void): void {
        try {
            var postBody = '{"question":"' + utterance + '"}';
            request({
                url: kbUrl,
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Ocp-Apim-Subscription-Key': ocpApimSubscriptionKey
                },
                body: postBody
            },
                function (error: Error, response: any, body: string) {
                    var result: IQnAMakerResult;
                    try {
                        console.log(body);
                        if (!error) {
                            result = JSON.parse(body);
                            result.score = result.score / 100;
                            result.answer = htmlentities.decode(result.answer);
                        }
                    } catch (e) {
                        error = e;
                    }

                    // Return result
                    try {
                        if (!error) {
                            callback(null, result);
                        } else {
                            var m = error.toString();
                            callback(error instanceof Error ? error : new Error(m));
                        }

                    } catch (e) {
                        console.error(e.toString());
                    }
                }
            );
        } catch (e) {
            callback(e instanceof Error ? e : new Error(e.toString()));
        }
    }
}