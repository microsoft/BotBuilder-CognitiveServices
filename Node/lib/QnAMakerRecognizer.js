"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var request = require("request");
var entities = require("html-entities");
var qnaMakerV2ServiceEndpoint = 'https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/';
var qnaMakerServiceEndpoint = null;
var endpointHostName = null;
var qnaApi = 'generateanswer';
var qnaTrainApi = 'train';
var htmlentities = new entities.AllHtmlEntities();
var QnAMakerRecognizer = (function () {
    function QnAMakerRecognizer(options) {
        this.options = options;
        if (this.options.endpointHostName != null) {
            var hostName = this.options.endpointHostName.toLowerCase();
            if (hostName.indexOf('https://') > -1)
                hostName = hostName.split('/')[2];
            if (hostName.indexOf("qnamaker") > -1) {
                hostName = hostName.split('/')[0];
            }
            hostName = hostName.replace("/", "");
            this.kbUri = 'https://' + hostName + '/qnamaker/knowledgebases/' + this.options.knowledgeBaseId + '/' + qnaApi;
            this.authHeader = 'Authorization';
            var re = /endpointkey/gi;
            if (this.options.authKey.search(re) > -1) {
                this.authorizationKey = this.options.authKey.trim();
            }
            else {
                this.authorizationKey = 'EndpointKey ' + this.options.authKey.trim();
            }
        }
        else {
            this.kbUri = qnaMakerV2ServiceEndpoint + this.options.knowledgeBaseId + '/' + qnaApi;
            this.kbUriForTraining = qnaMakerV2ServiceEndpoint + this.options.knowledgeBaseId + '/' + qnaTrainApi;
            this.authHeader = 'Ocp-Apim-Subscription-Key';
            this.authorizationKey = this.options.authKey;
        }
        this.intentName = options.intentName || "qna";
        if (typeof this.options.top !== 'number') {
            this.top = 1;
        }
        else {
            this.top = this.options.top;
        }
    }
    QnAMakerRecognizer.prototype.recognize = function (context, cb) {
        var result = { score: 0.0, answers: null, intent: null };
        if (context && context.message && context.message.text) {
            var utterance = context.message.text;
            QnAMakerRecognizer.recognize(utterance, this.kbUri, this.authorizationKey, this.authHeader, this.top, this.intentName, function (error, result) {
                if (!error) {
                    cb(null, result);
                }
                else {
                    cb(error, null);
                }
            });
        }
    };
    QnAMakerRecognizer.recognize = function (utterance, kbUrl, authkey, authHeader, top, intentName, callback) {
        try {
            request({
                url: kbUrl,
                method: 'POST',
                headers: (_a = {},
                    _a[authHeader] = authkey,
                    _a),
                json: {
                    question: utterance,
                    top: top
                }
            }, function (error, response, result) {
                var result;
                try {
                    if (!error) {
                        if (response.statusCode === 200) {
                            var answerEntities = [];
                            if (result.answers && result.answers.length > 0) {
                                result.answers.forEach(function (ans) {
                                    ans.score /= 100;
                                    ans.answer = htmlentities.decode(ans.answer);
                                    if (ans.questions && ans.questions.length > 0) {
                                        ans.questions = ans.questions.map(function (q) { return htmlentities.decode(q); });
                                    }
                                    var answerEntity = {
                                        score: ans.score,
                                        entity: ans.answer,
                                        type: 'answer'
                                    };
                                    answerEntities.push(answerEntity);
                                });
                                result.score = result.answers[0].score;
                                result.entities = answerEntities;
                                result.intent = intentName;
                            }
                        }
                        else {
                            error = new Error("QnA request returned a " + response.statusCode + " code with body: " + result);
                        }
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
        var _a;
    };
    return QnAMakerRecognizer;
}());
exports.QnAMakerRecognizer = QnAMakerRecognizer;
