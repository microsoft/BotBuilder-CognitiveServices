"use strict";
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var builder = require('botbuilder');
var QnAMakerDialog = (function (_super) {
    __extends(QnAMakerDialog, _super);
    function QnAMakerDialog(options) {
        _super.call(this);
        this.options = options;
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
        if (qnaMakerResult.score >= threshold) {
            session.send(qnaMakerResult.answer);
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
