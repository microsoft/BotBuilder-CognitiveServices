"use strict";
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var builder = require('botbuilder');
var QnAMakerRecognizer_1 = require('./QnAMakerRecognizer');
var QnAMakerDialog = (function (_super) {
    __extends(QnAMakerDialog, _super);
    function QnAMakerDialog(options) {
        _super.call(this);
        this.options = options;
        if (typeof this.options.qnaThreshold !== 'number') {
            this.answerThreshold = 30.0;
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
        this.kbId = this.options.knowledgeBaseId;
        this.ocpApimSubscriptionKey = this.options.subscriptionKey;
        this.recognizers = new QnAMakerRecognizer_1.QnAMakerRecognizer(this.kbId, this.ocpApimSubscriptionKey);
    }
    QnAMakerDialog.prototype.replyReceived = function (session, recognizeResult) {
        var _this = this;
        var threshold = this.answerThreshold;
        var noMatchMessage = this.defaultNoMatchMessage;
        if (!recognizeResult) {
            var locale = session.preferredLocale();
            this.recognize({ message: session.message, locale: locale, dialogData: session.dialogData, activeDialog: true }, function (error, result) {
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
    QnAMakerDialog.prototype.invokeAnswer = function (session, recognizeResult, threshold, noMatchMessage) {
        if (recognizeResult.score >= threshold) {
            session.send(recognizeResult.answer);
        }
        else {
            session.send(noMatchMessage);
        }
    };
    QnAMakerDialog.prototype.emitError = function (session, err) {
        var m = err.toString();
        err = err instanceof Error ? err : new Error(m);
        session.error(err);
    };
    return QnAMakerDialog;
}(builder.Dialog));
exports.QnAMakerDialog = QnAMakerDialog;
