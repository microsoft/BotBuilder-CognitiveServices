import * as builder from 'botbuilder';

/** Options used to configure an IQnAMakerOptions. */
export interface IQnAMakerOptions {
    /** (Optional) minimum score needed to trigger the response. The default value is 30.0. */
    qnaThreshold?: number;
    
    /** The QnA Maker Knowledge Base ID to query for getting the response. */
    knowledgeBaseId: string;
    
    /** User's QnA Maker Subscription Key for authorization. */
    subscriptionKey: string;
    
    /** (Optional) message that is returned when there are no responses above the threshold. The default is 'No match found!'. */
    defaultMessage?: string;
    
}

/** Results returned by an QnA Maker recognizer. */
export interface IQnAMakerResult extends builder.IIntentRecognizerResult {
    /** Top answer that was matched with score greater than the specified threshold. */
    answer: string;
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
    * @param ocpApimSubscriptionKey The subscription key of the user to access knowledge bases on https://qnamaker.ai.
    * @param callback Callback to invoke with the results of the QnA Maker recognition step.
    * @param callback.err Error that occured during the recognition step.
    * @param callback.result the qna maker result that were recognized.
    */
    static recognize(utterance: string, kbUrl: string, ocpApimSubscriptionKey: string, callback: (error: Error, result?: IQnAMakerResult) => void): void;
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
}

