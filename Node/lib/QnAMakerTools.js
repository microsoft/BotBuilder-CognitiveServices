"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var builder = require("botbuilder");
var lib = new builder.Library('qnaMakerTools');
exports.createLibrary = function () {
    return lib;
};
exports.answerSelector = function (session, options) {
    session.beginDialog('qnaMakerTools:answerSelection', options || {});
};
lib.dialog('answerSelection', [
    function (session, args) {
        var qnaMakerResult = args;
        session.dialogData.qnaMakerResult = qnaMakerResult;
        var questionOptions = [];
        qnaMakerResult.answers.forEach(function (qna) { questionOptions.push(qna.questions[0]); });
        var promptOptions = { listStyle: builder.ListStyle.button };
        builder.Prompts.choice(session, "I've found multiple responses matching your query. Please select from the following:", questionOptions, promptOptions);
    },
    function (session, results) {
        var qnaMakerResult = session.dialogData.qnaMakerResult;
        var filteredResult = qnaMakerResult.answers.filter(function (qna) { return qna.questions[0] === results.response.entity; });
        var selectedQnA = filteredResult[0];
        session.send(selectedQnA.answer);
        session.endDialogWithResult(selectedQnA);
    },
]);
