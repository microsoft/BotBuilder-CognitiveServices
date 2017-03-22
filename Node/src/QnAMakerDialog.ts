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
import { QnAMakerRecognizer, IQnAMakerResult, IQnAMakerOptions } from './QnAMakerRecognizer';

export class QnAMakerDialog extends builder.Dialog {
    private answerThreshold: number;
    private defaultNoMatchMessage: string;
    private recognizers: builder.IntentRecognizerSet;

    constructor(private options: IQnAMakerOptions) {
        super();
        this.recognizers = new builder.IntentRecognizerSet(options);
        if (typeof this.options.qnaThreshold !== 'number') {
            this.answerThreshold = 0.3;
        }
        else {
            this.answerThreshold = this.options.qnaThreshold;
        }
        if (this.options.defaultMessage && this.options.defaultMessage !== "") {
            this.defaultNoMatchMessage = this.options.defaultMessage;
        }
        else {
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
                    if (!error) {
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

    public recognize(context: builder.IRecognizeContext, cb: (error: Error, result: IQnAMakerResult) => void): void {
        this.recognizers.recognize(context, cb);
    }

    public recognizer(plugin: builder.IIntentRecognizer): this {
        // Append recognizer
        this.recognizers.recognizer(plugin);
        return this;

    }

    private invokeAnswer(session: builder.Session, recognizeResult: builder.IIntentRecognizerResult, threshold: number, noMatchMessage: string): void {
        var qnaMakerResult = recognizeResult as IQnAMakerResult;
        if (qnaMakerResult.score >= threshold) {
            session.send(qnaMakerResult.answer);
        }
        else {
            session.send(noMatchMessage);
        }
    }

    private emitError(session: builder.Session, err: Error): void {
        var m = err.toString();
        err = err instanceof Error ? err : new Error(m);
        session.error(err);
    }
}