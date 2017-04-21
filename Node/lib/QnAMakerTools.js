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
                var promptOptions = { listStyle: builder.ListStyle.button };
                builder.Prompts.choice(session, "Please select from the following:", questionOptions, promptOptions);
            },
            function (session, results) {
                var qnaMakerResult = session.dialogData.qnaMakerResult;
                var filteredResult = qnaMakerResult.answers.filter(function (qna) { return qna.questions[0] === results.response.entity; });
                var selectedQnA = filteredResult[0];
                session.send(selectedQnA.answer);
                session.endDialogWithResult(selectedQnA);
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
