"use strict";

var builder = require("botbuilder");
var lib = new builder.Library('customQnAMakerTools');
exports.createLibrary = function () {
    return lib;
};
exports.answerSelector = function (session, options) {
    session.beginDialog('customQnAMakerTools:answerSelection', options || {});
};
lib.dialog('answerSelection', [
    function (session, args) {
        var qnaMakerResult = args;
        session.dialogData.qnaMakerResult = qnaMakerResult;
        var questionOptions = [];
        qnaMakerResult.answers.forEach(function (qna) { questionOptions.push(qna.questions[0]); });
        builder.Prompts.choice(session, "I've found many responses matching your query. Please select from the following:", questionOptions);
    },
    function (session, results) {
        var qnaMakerResult = session.dialogData.qnaMakerResult;
        var filteredResult = qnaMakerResult.answers.filter(function (qna) { return qna.questions[0] === results.response.entity; });
        var selectedQnA = filteredResult[0];
        session.send(selectedQnA.answer);
        session.endDialogWithResult(selectedQnA);
    },
]);
