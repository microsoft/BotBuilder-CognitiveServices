"use strict";
var request = require('request');
var entities = require('html-entities');
var qnaMakerServiceEndpoint = 'https://westus.api.cognitive.microsoft.com/qnamaker/v1.0/knowledgebases/';
var qnaApi = 'generateanswer';
var htmlentities = new entities.AllHtmlEntities();
var QnAMakerRecognizer = (function () {
    function QnAMakerRecognizer(options) {
        this.options = options;
        this.kbUri = qnaMakerServiceEndpoint + options.knowledgeBaseId + '/' + qnaApi;
        this.ocpApimSubscriptionKey = options.subscriptionKey;
    }
    QnAMakerRecognizer.prototype.recognize = function (context, cb) {
        var result = { score: 0.0, answer: null, intent: null };
        if (context && context.message && context.message.text) {
            var utterance = context.message.text;
            QnAMakerRecognizer.recognize(utterance, this.kbUri, this.ocpApimSubscriptionKey, function (error, result) {
                if (!error) {
                    cb(null, result);
                }
                else {
                    cb(error, null);
                }
            });
        }
    };
    QnAMakerRecognizer.recognize = function (utterance, kbUrl, ocpApimSubscriptionKey, callback) {
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
            }, function (error, response, body) {
                var result;
                try {
                    console.log(body);
                    if (!error) {
                        result = JSON.parse(body);
                        result.score = result.score / 100;
                        result.answer = htmlentities.decode(result.answer);
                    }
                }
                catch (e) {
                    error = e;
                }
                try {
                    if (!error) {
                        callback(null, result);
                    }
                    else {
                        var m = error.toString();
                        callback(error instanceof Error ? error : new Error(m));
                    }
                }
                catch (e) {
                    console.error(e.toString());
                }
            });
        }
        catch (e) {
            callback(e instanceof Error ? e : new Error(e.toString()));
        }
    };
    return QnAMakerRecognizer;
}());
exports.QnAMakerRecognizer = QnAMakerRecognizer;
