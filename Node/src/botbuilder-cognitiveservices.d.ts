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
export class QnAMakerDialog extends botbuilder.Dialog {
    /**  
    * Constructs a new instance of an QnAMakerDialog.
    * @param options used to initialize the dialog.
    */
    constructor(options: IQnADialogOptions);

    /**
    * Processes messages received from the user. Called by the dialog system. 
    * @param session Session object for the current conversation.
    */
    replyReceived(session: Session): void;
}
