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
import { QnAMakerTools } from './QnAMakerTools';

var qnaMakerServiceEndpoint = 'https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/';
var qnaApi = 'generateanswer';
var qnaTrainApi = 'train';
var htmlentities = new entities.AllHtmlEntities();

export interface IQnAMakerResults extends builder.IIntentRecognizerResult {
    answers: IQnAMakerResult[];
}

export interface IQnAMakerResult{
    answer: string;
    questions: string[];
    score: number;
}

export interface IQnAMakerOptions extends builder.IIntentRecognizerSetOptions {
    knowledgeBaseId: string;
    subscriptionKey: string;
    top?: number;
    qnaThreshold?: number;
    defaultMessage?: string;
    intentName?: string;
    feedbackLib?: QnAMakerTools;
}

export class QnAMakerRecognizer implements builder.IIntentRecognizer {
    private kbUri: string;
    public kbUriForTraining: string;
    public ocpApimSubscriptionKey: string;
    private top: number;
    private intentName: string;
    
    constructor(private options: IQnAMakerOptions){
        this.kbUri = qnaMakerServiceEndpoint + this.options.knowledgeBaseId + '/' + qnaApi;
        this.kbUriForTraining = qnaMakerServiceEndpoint + this.options.knowledgeBaseId + '/' + qnaTrainApi;
        this.ocpApimSubscriptionKey = this.options.subscriptionKey;
        this.intentName = options.intentName || "qna";
        if(typeof this.options.top !== 'number'){
          this.top = 1;
        }
        else
        {
          this.top = this.options.top;
        }
    }

    public recognize(context: builder.IRecognizeContext, cb: (error: Error, result: IQnAMakerResults) => void): void {
        var result: IQnAMakerResults = { score: 0.0, answers: null, intent: null };
        if (context && context.message && context.message.text) {
            var utterance = context.message.text;
            QnAMakerRecognizer.recognize(utterance, this.kbUri, this.ocpApimSubscriptionKey, this.top, this.intentName, (error, result) => {
                    if (!error) {
                        cb(null, result);
                    } else {
                        cb(error, null);
                    }
                }
            );
        }
    }

    static recognize(utterance: string, kbUrl: string, ocpApimSubscriptionKey: string, top: number, intentName: string, callback: (error: Error, result?: IQnAMakerResults) => void): void {
        try {
            request({
                url: kbUrl,
                method: 'POST',
                headers: {
                    'Ocp-Apim-Subscription-Key': ocpApimSubscriptionKey
                },
                json: {
                    question: utterance,
                    top: top
                }
            },
                function (error: Error, response: any, result: IQnAMakerResults) {
                    var result: IQnAMakerResults;
                    try {
                        if (!error) {
                            if (response.statusCode === 200) {
                                var answerEntities: builder.IEntity[] = [];
                                if(result.answers && result.answers.length > 0){
                                    result.answers.forEach((ans) => {
                                        ans.score /= 100;
                                        ans.answer = htmlentities.decode(ans.answer);
                                        if (ans.questions && ans.questions.length > 0) {
                                            ans.questions = ans.questions.map((q: string) => htmlentities.decode(q));
                                        }
                                        var answerEntity = {
                                            score: ans.score,
                                            entity: ans.answer,
                                            type: 'answer'
                                        }
                                        answerEntities.push(answerEntity as builder.IEntity);
                                    });
                                    result.score = result.answers[0].score;
                                    result.entities = answerEntities;
                                    result.intent = intentName;
                                }
                            } else {
                                error = new Error(`QnA request returned a ${response.statusCode} code with body: ${result}`);
                            }
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