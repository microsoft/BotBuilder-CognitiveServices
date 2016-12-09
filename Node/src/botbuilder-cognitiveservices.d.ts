import * as builder from 'botbuilder';

/** Options used to configure an IQnADialogOptions. */
export interface IQnADialogOptions {
    /** (Optional) minimum score needed to trigger the response. The default value is 30.0. */
    qnaThreshold?: number;
    
    /** The QnA Maker Knowledge Base ID to query for getting the response. */
    knowledgeBaseId: string;
    
    /** User's QnA Maker Subscription Key for authorization. */
    subscriptionKey: string;
    
    /** (Optional) message that is returned when there are no responses above the threshold. The default is 'No match found!'. */
    defaultMessage?: string;
    
}

/** Fetches the best matching answer response from QnA Maker's Knowledge Base. */
export class QnAMakerDialog extends builder.Dialog {
    /**  
    * Constructs a new instance of an QnAMakerDialog.
    * @param options used to initialize the dialog.
    */
    constructor(options: IQnADialogOptions);

    /**
    * Processes messages received from the user. Called by the dialog system. 
    * @param session Session object for the current conversation.
    * @param recognizeResult (Optional) recognition results returned from a prior call to the dialogs [recognize()](#recognize) method. 
    */
    replyReceived(session: builder.Session, recognizeResult?: IQnAMakerResult): void;

    /** Attempts to find an answer to users text utterance from QnA Maker knowledge base. */
    recognize(context: builder.IRecognizeContext, cb: (error: Error, result: IQnAMakerResult) => void): void;
}

