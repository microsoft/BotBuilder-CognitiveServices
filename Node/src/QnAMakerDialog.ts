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
import { QnAMakerRecognizer, IQnAMakerResults, IQnAMakerOptions, IQnAMakerResult } from './QnAMakerRecognizer'; 
import { QnAMakerTools } from './QnAMakerTools';

export class QnAMakerDialog extends builder.Dialog {
    private answerThreshold: number;
    private defaultNoMatchMessage: string;
    private recognizers: builder.IntentRecognizerSet;
    private ocpApimSubscriptionKey: string;
    private kbUriForTraining: string;
    private qnaMakerTools: QnAMakerTools;
   
    constructor(private options: IQnAMakerOptions){
        super();
        this.recognizers = new builder.IntentRecognizerSet(options);
        var qnaRecognizer = this.options.recognizers[0] as QnAMakerRecognizer;
        this.ocpApimSubscriptionKey = qnaRecognizer.ocpApimSubscriptionKey;
        this.kbUriForTraining = qnaRecognizer.kbUriForTraining;
        this.qnaMakerTools = this.options.feedbackLib;
        if(typeof this.options.qnaThreshold !== 'number'){
            this.answerThreshold = 0.3;
        }
        else
        {
            this.answerThreshold = this.options.qnaThreshold;
        }
        if(this.options.defaultMessage && this.options.defaultMessage !== "")
        {
            this.defaultNoMatchMessage = this.options.defaultMessage;
        }
        else
        {
            this.defaultNoMatchMessage = "No match found!";
        }
    }

    public replyReceived(session: builder.Session, recognizeResult?: builder.IIntentRecognizerResult): void {
        var threshold = this.answerThreshold;
        var noMatchMessage = this.defaultNoMatchMessage;
        if (!recognizeResult) {
            var locale = session.preferredLocale();
            var context = <builder.IRecognizeDialogContext>session.toRecognizeContext();
            context.dialogData = session.dialogData;
            context.activeDialog = true;
            this.recognize(context, (error, result) => {
                    try {
                        if(!error){
                            this.invokeAnswer(session, result, threshold, noMatchMessage);
                        }
                    } catch (e) {
                        this.emitError(session, e);
                    }
                }
            );
        } else {
            this.invokeAnswer(session, recognizeResult, threshold, noMatchMessage);
        }
    }

    public recognize(context: builder.IRecognizeContext, cb: (error: Error, result: IQnAMakerResults) => void): void {
        this.recognizers.recognize(context, cb);
    }

    public recognizer(plugin: builder.IIntentRecognizer): this {
        // Append recognizer
        this.recognizers.recognizer(plugin);
        return this;
    }
    
    public invokeAnswer(session: builder.Session, recognizeResult: builder.IIntentRecognizerResult, threshold: number, noMatchMessage: string): void {
        var qnaMakerResult = recognizeResult as IQnAMakerResults;
        session.privateConversationData.qnaFeedbackUserQuestion = session.message.text;
        if (qnaMakerResult.score >= threshold && qnaMakerResult.answers.length > 0) {
            if(this.isConfidentAnswer(qnaMakerResult) || this.qnaMakerTools == null){
                this.respondFromQnAMakerResult(session, qnaMakerResult);
                this.defaultWaitNextMessage(session, qnaMakerResult);
            }
            else {
                this.qnaFeedbackStep(session, qnaMakerResult);
            }
        }
        else {
            session.send(noMatchMessage);
            this.defaultWaitNextMessage(session, qnaMakerResult);
        }
    }

    public qnaFeedbackStep(session: builder.Session, qnaMakerResult: IQnAMakerResults) : void {
        this.qnaMakerTools.answerSelector(session, qnaMakerResult);
    }

    public respondFromQnAMakerResult(session: builder.Session, qnaMakerResult: IQnAMakerResults): void {
        session.send(qnaMakerResult.answers[0].answer);
    }

    public defaultWaitNextMessage(session: builder.Session, qnaMakerResult: IQnAMakerResults): void {
        session.endDialog();
    }

    public isConfidentAnswer(qnaMakerResult: IQnAMakerResults): boolean{
        if(qnaMakerResult.answers.length <= 1 
            || qnaMakerResult.answers[0].score >= 0.99
            || (qnaMakerResult.answers[0].score - qnaMakerResult.answers[1].score > 0.2)
            ){
                return true;
            }
        return false;
    }

    public dialogResumed(session: builder.Session, result: builder.IDialogResult<IQnAMakerResult>): void {
        var selectedResponse = result as IQnAMakerResult;
        if(selectedResponse && selectedResponse.answer && selectedResponse.questions && selectedResponse.questions.length > 0){
            var feedbackPostBody: string =
                '{"feedbackRecords": [{"userId": "' + session.message.user.id + '","userQuestion": "' + session.privateConversationData.qnaFeedbackUserQuestion
                + '","kbQuestion": "' + selectedResponse.questions[0] + '","kbAnswer": "' + selectedResponse.answer + '"}]}';
            this.recordQnAFeedback(feedbackPostBody);
        }
        this.defaultWaitNextMessage(session, {answers: [ selectedResponse ]} as IQnAMakerResults);
    }

    private recordQnAFeedback(body: string) : void {
        console.log(body);
        request({
            url: this.kbUriForTraining,
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'Ocp-Apim-Subscription-Key': this.ocpApimSubscriptionKey
            },
            body: body
        },
            function (error: Error, response: any, body: string) {
                if(response.statusCode == 204){
                    console.log('Feedback sent successfully.')
                } else {
                    console.log('error: '+ response.statusCode)
                    console.log(body)
                }
            }
        );
    }

    private emitError(session: builder.Session, err: Error): void {
        var m = err.toString();
        err = err instanceof Error ? err : new Error(m);
        session.error(err);
    }
}