// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Bot Framework: http://botframework.com
// 
// Bot Builder SDK Github:
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

import botbuilder = require('botbuilder');
import request = require('request');

var qnaMakerServiceEndpoint = 'https://westus.api.cognitive.microsoft.com/qnamaker/v1.0/knowledgebases/';
var qnaApi = 'generateanswer';

export interface IQnADialogOptions {
	qnaThreshold?: number;
	knowledgeBaseId?: string;
	subscriptionKey?:string;
}

export class QnAMakerDialog extends botbuilder.Dialog {
	private kbUri: string;
	private answerThreshold: number;
	private ocpApimSubscriptionKey: string;

	constructor(private options: IQnADialogOptions = {}){
		super();
		if(typeof this.options.qnaThreshold !== 'number'){
			this.answerThreshold = 30.0;
		}
		else
		{
			this.answerThreshold = this.options.qnaThreshold;
		}

		this.kbUri = qnaMakerServiceEndpoint + this.options.knowledgeBaseId + '/' + qnaApi;
		this.ocpApimSubscriptionKey = this.options.subscriptionKey
	}

	public replyReceived(session: botbuilder.Session): void {
        var threshold = this.answerThreshold;
        var postBody = '{"question":"' + session.message.text + '"}';
        request({
				url: this.kbUri,
				method: 'POST',
				headers: {
					'Content-Type': 'application/json',
					'Ocp-Apim-Subscription-Key': this.ocpApimSubscriptionKey
				},
				body: postBody
			}, 
		
			function(error, response, body){
                try {
                    console.log(body);
                    if(!error){
                        var result = JSON.parse(body);
                        if(parseFloat(result.score) >= threshold)
                        {
                            session.send(result.answer);
                        }
                        else
                        {
                            session.send("No good match found!");
                        }
                    }
                } catch (e) {
                    console.log(e);
                    session.send("Not able to fetch response from server.");
                }
            }
        );
    }
}