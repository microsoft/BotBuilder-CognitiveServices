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
import { IQnAMakerResults, IQnAMakerResult } from './QnAMakerRecognizer';

export interface IQnAMakerTools{
    createLibrary(): builder.Library;
    answerSelector(session: builder.Session, options: IQnAMakerResults): void;
}

export class QnAMakerTools implements IQnAMakerTools{
    private lib: builder.Library;
    
    constructor(){
        this.lib = new builder.Library('qnaMakerTools');
        this.lib.dialog('answerSelection', [
                function (session, args) {
                    var qnaMakerResult = args as IQnAMakerResults;
                    session.dialogData.qnaMakerResult = qnaMakerResult;
                    var questionOptions: string[] = [];
                    qnaMakerResult.answers.forEach(function (qna: IQnAMakerResult) { questionOptions.push(qna.questions[0]); });
                    questionOptions.push("None of the above.");
                    var promptOptions: builder.IPromptOptions = {listStyle: builder.ListStyle.button, maxRetries: 0};
                    builder.Prompts.choice(session,  "Did you mean:", questionOptions, promptOptions);
                },
                function (session, results) {
                    if(results && results.response && results.response.entity){
                        var qnaMakerResult = session.dialogData.qnaMakerResult as IQnAMakerResults;
                        var filteredResult = qnaMakerResult.answers.filter(qna => qna.questions[0] === results.response.entity);
                        if(filteredResult !== null && filteredResult.length > 0){
                            var selectedQnA = filteredResult[0];
                            session.send(selectedQnA.answer);
                            session.endDialogWithResult({ response: selectedQnA });
                        }
                    } else {
                        session.send("Sorry! Not able to match any of the options.");
                    }
                    session.endDialog();
                },
            ]);
    }
    
    public createLibrary(): builder.Library{
        return this.lib;
    }

    public answerSelector(session: builder.Session, options: IQnAMakerResults): void{
        session.beginDialog('qnaMakerTools:answerSelection', options || {});
    }

}