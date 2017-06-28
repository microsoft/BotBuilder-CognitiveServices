"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var builder = require("botbuilder");
var request = require("request");
var QnAMakerDialog = (function (_super) {
    __extends(QnAMakerDialog, _super);
    function QnAMakerDialog(options) {
        var _this = _super.call(this) || this;
        _this.options = options;
        _this.recognizers = new builder.IntentRecognizerSet(options);
        var qnaRecognizer = _this.options.recognizers[0];
        _this.ocpApimSubscriptionKey = qnaRecognizer.ocpApimSubscriptionKey;
        _this.kbUriForTraining = qnaRecognizer.kbUriForTraining;
        _this.qnaMakerTools = _this.options.feedbackLib;
        if (typeof _this.options.qnaThreshold !== 'number') {
            _this.answerThreshold = 0.3;
        }
        else {
            _this.answerThreshold = _this.options.qnaThreshold;
        }
        if (_this.options.defaultMessage && _this.options.defaultMessage !== "") {
            _this.defaultNoMatchMessage = _this.options.defaultMessage;
        }
        else {
            _this.defaultNoMatchMessage = "No match found!";
        }
        return _this;
    }
    QnAMakerDialog.prototype.replyReceived = function (session, recognizeResult) {
        var _this = this;
        var threshold = this.answerThreshold;
        var noMatchMessage = this.defaultNoMatchMessage;
        if (!recognizeResult) {
            var locale = session.preferredLocale();
            var context = session.toRecognizeContext();
            context.dialogData = session.dialogData;
            context.activeDialog = true;
            this.recognize(context, function (error, result) {
                try {
                    if (!error) {
                        _this.invokeAnswer(session, result, threshold, noMatchMessage);
                    }
                }
                catch (e) {
                    _this.emitError(session, e);
                }
            });
        }
        else {
            this.invokeAnswer(session, recognizeResult, threshold, noMatchMessage);
        }
    };
    QnAMakerDialog.prototype.recognize = function (context, cb) {
        this.recognizers.recognize(context, cb);
    };
    QnAMakerDialog.prototype.recognizer = function (plugin) {
        this.recognizers.recognizer(plugin);
        return this;
    };
    QnAMakerDialog.prototype.invokeAnswer = function (session, recognizeResult, threshold, noMatchMessage) {
        var qnaMakerResult = recognizeResult;
        session.privateConversationData.qnaFeedbackUserQuestion = session.message.text;
        if (qnaMakerResult.score >= threshold && qnaMakerResult.answers.length > 0) {
            if (this.isConfidentAnswer(qnaMakerResult) || this.qnaMakerTools == null) {
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
    };
    QnAMakerDialog.prototype.qnaFeedbackStep = function (session, qnaMakerResult) {
        this.qnaMakerTools.answerSelector(session, qnaMakerResult);
    };
    QnAMakerDialog.prototype.respondFromQnAMakerResult = function (session, qnaMakerResult) {
        session.send(qnaMakerResult.answers[0].answer);
    };
    QnAMakerDialog.prototype.defaultWaitNextMessage = function (session, qnaMakerResult) {
        session.endDialog();
    };
    QnAMakerDialog.prototype.isConfidentAnswer = function (qnaMakerResult) {
        if (qnaMakerResult.answers.length <= 1
            || qnaMakerResult.answers[0].score >= 0.99
            || (qnaMakerResult.answers[0].score - qnaMakerResult.answers[1].score > 0.2)) {
            return true;
        }
        return false;
    };
    QnAMakerDialog.prototype.dialogResumed = function (session, result) {
        var selectedResponse = result;
        if (selectedResponse && selectedResponse.answer && selectedResponse.questions && selectedResponse.questions.length > 0) {
            var feedbackPostBody = '{"feedbackRecords": [{"userId": "' + session.message.user.id + '","userQuestion": "' + session.privateConversationData.qnaFeedbackUserQuestion
                + '","kbQuestion": "' + selectedResponse.questions[0] + '","kbAnswer": "' + selectedResponse.answer + '"}]}';
            this.recordQnAFeedback(feedbackPostBody);
        }
        this.defaultWaitNextMessage(session, { answers: [selectedResponse] });
    };
    QnAMakerDialog.prototype.recordQnAFeedback = function (body) {
        console.log(body);
        request({
            url: this.kbUriForTraining,
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'Ocp-Apim-Subscription-Key': this.ocpApimSubscriptionKey
            },
            body: body
        }, function (error, response, body) {
            if (response.statusCode == 204) {
                console.log('Feedback sent successfully.');
            }
            else {
                console.log('error: ' + response.statusCode);
                console.log(body);
            }
        });
    };
    QnAMakerDialog.prototype.emitError = function (session, err) {
        var m = err.toString();
        err = err instanceof Error ? err : new Error(m);
        session.error(err);
    };
    return QnAMakerDialog;
}(builder.Dialog));
exports.QnAMakerDialog = QnAMakerDialog;
