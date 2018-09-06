import * as builder from 'botbuilder';

/** Options used to configure an IQnAMakerOptions. */
export interface IQnAMakerOptions {
    /** (Optional) minimum score needed to trigger the response. The default value is 0.3 */
    qnaThreshold?: number;

    /** The QnA Maker Knowledge Base ID to query for getting the response. */
    knowledgeBaseId: string;

    /** User's QnA Maker Subscription Key for authorization. */
    authKey: string;

    /** (Optional) message that is returned when there are no responses above the threshold. The default is 'No match found!'. */
    defaultMessage?: string;

    /** (Optional) maximum number of answers from the QnA Maker service. */
    Top?: number;

    /** And instance of dialog library which includes the feedback flow. This should be set if Top > 1. */
    feedbackLib?: QnAMakerTools;

    /** The endpoint of the service */
    endpointHostName?: string;
}

/** Result returned by an QnA Maker recognizer. */
export interface IQnAMakerResult {
    /** Answer text of the matched QnA. */
    answer: string;

    /** Array of questions and alternate phrases of the matched QnA. */
    questions: string[];

    /**  The confidence score from QnA Maker service for the matched QnA. */
    score: number;
}

/** Response returned by QnA Maker recognizer. */
export interface IQnAMakerResult extends builder.IIntentRecognizerResult {
    /** List of top matched QnA with score greater than the specified threshold. */
    answers: IQnAMakerResult[];
}

/**
* QnA Maker recognizer plugin fetches answers for users utterances using [QnA Maker](https://qnamaker.ai)
* @param options used to initialize the recognizer.
*/
export class QnAMakerRecognizer implements builder.IIntentRecognizer {
    /**
    * Constructs a new instance of the recognizer.
    * @param options used to initialize the recognizer.
    */
    constructor(options: IQnAMakerOptions);

    /** Attempts to match a users text utterance and retrieves the best matching answer. */
    public recognize(context: builder.IRecognizeContext, cb: (error: Error, result: IQnAMakerResult) => void): void;

    /**
    * Calls QnA Maker to retrieve answers for users utterances.
    * @param utterance The text to pass to QnA Maker for recognition.
    * @param kbUrl URI for QnA Maker knowledge base hosted on https://qnamaker.ai.
    * @param authKey The subscription key of the user to access knowledge bases on https://qnamaker.ai.
    * @param authHeader The subscription key of the user to access knowledge bases on https://qnamaker.ai.
    * @param callback Callback to invoke with the results of the QnA Maker recognition step.
    * @param callback.err Error that occured during the recognition step.
    * @param callback.result the qna maker result that were recognized.
    */
    static recognize(utterance: string, kbUrl: string, authKey: string, authHeader: string, callback: (error: Error, result?: IQnAMakerResult) => void): void;
}

/** Fetches the best matching answer response from QnA Maker's Knowledge Base. */
export class QnAMakerDialog extends builder.Dialog {
    /**  
    * Constructs a new instance of an QnAMakerDialog.
    * @param options used to initialize the dialog.
    */
    constructor(options: IQnAMakerOptions);

    /**
    * Processes messages received from the user. Called by the dialog system. 
    * @param session Session object for the current conversation.
    * @param recognizeResult (Optional) recognition results returned from a prior call to the dialogs [recognize()](#recognize) method. 
    */
    replyReceived(session: builder.Session, recognizeResult?: builder.IIntentRecognizerResult): void;

    /** Attempts to find an answer to users text utterance from QnA Maker knowledge base. */
    recognize(context: builder.IRecognizeContext, cb: (error: Error, result: IQnAMakerResult) => void): void;

    /**
     * Adds a new recognizer plugin to the QnA Maker dialog.
     * @param plugin The recognizer to add. 
     */
    recognizer(plugin: builder.IIntentRecognizer): QnAMakerDialog;

    /**
     * 
     * @param session Session object for the current conversation.
     * @param qnaMakerResult QnA Maker response from the service.
     */
    qnaFeedbackStep(session: builder.Session, qnaMakerResult: IQnAMakerResult): void;

    /**
     * Processes the QnA Maker result and returns true when the top answer is very confident
     * @param qnaMakerResult QnA Maker response from the service.
     */
    isConfidentAnswer(qnaMakerResult: IQnAMakerResult): boolean;

    /**
     * Default wait method the service calls after responding to the received text.
     * @param session Session object for the current conversation.
     * @param qnaMakerResult QnA Maker response from the service.
     */
    defaultWaitNextMessage(session: builder.Session, qnaMakerResult: IQnAMakerResult): void;

    /**
     * Sends the message from the bot based on the result from QnA Maker.
     * @param session Session object for the current conversation.
     * @param qnaMakerResult QnA Maker response from the service.
     */
    respondFromQnAMakerResult(session: builder.Session, qnaMakerResult: IQnAMakerResult): void;
}

/**
 * Dialog library which includes the feedback flow when there are more than 1 good match
 * This should be set if Top > 1.
 */
export class QnAMakerTools {
    /**
     * Constructs a new instance of QnAMakerTools
     */
    constructor();

    /**
     * Returns the dialog library including the feedback dialog.
     */
    public createLibrary(): builder.Library;

    /**
     * Starts the new dialog to select the best match from top N QnA Maker responses
     * @param session Session object for the current conversation.
     * @param qnaMakerResult QnA Maker response from the service. 
     */
    public answerSelector(session: builder.Session, qnaMakerResult: IQnAMakerResult): void;
}