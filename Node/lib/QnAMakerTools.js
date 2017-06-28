"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var builder = require("botbuilder");
var QnAMakerTools = (function () {
    function QnAMakerTools() {
        this.lib = new builder.Library('qnaMakerTools');
        this.lib.dialog('answerSelection', [
            function (session, args) {
                var qnaMakerResult = args;
                session.dialogData.qnaMakerResult = qnaMakerResult;
                var questionOptions = [];
                qnaMakerResult.answers.forEach(function (qna) { questionOptions.push(qna.questions[0]); });
                questionOptions.push("None of the above.");
                var promptOptions = { listStyle: builder.ListStyle.button, maxRetries: 0 };
                builder.Prompts.choice(session, "Did you mean:", questionOptions, promptOptions);
            },
            function (session, results) {
                if (results && results.response && results.response.entity) {
                    var qnaMakerResult = session.dialogData.qnaMakerResult;
                    var filteredResult = qnaMakerResult.answers.filter(function (qna) { return qna.questions[0] === results.response.entity; });
                    if (filteredResult !== null && filteredResult.length > 0) {
                        var selectedQnA = filteredResult[0];
                        session.send(selectedQnA.answer);
                        session.endDialogWithResult(selectedQnA);
                    }
                }
                else {
                    session.send("Sorry! Not able to match any of the options.");
                }
                session.endDialog();
            },
        ]);
    }
    QnAMakerTools.prototype.createLibrary = function () {
        return this.lib;
    };
    QnAMakerTools.prototype.answerSelector = function (session, options) {
        session.beginDialog('qnaMakerTools:answerSelection', options || {});
    };
    return QnAMakerTools;
}());
exports.QnAMakerTools = QnAMakerTools;
