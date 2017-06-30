// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Bot Framework: http://botframework.com
// 
// Bot Builder Cognitive Services Github:
// https://github.com/Microsoft/BotBuilder-CognitiveServices
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

import { Session, IntentDialog, UniversalBot } from "botbuilder";

// CONSTS
export namespace Status {
    export const NoActionRecognized: string;
    export const Fulfilled: string;
    export const MissingParameters: string;
    export const ContextSwitch: string;
}

// API
export function evaluate(
    modelUrl: string,
    actions: Array<IAction>,
    currentActionModel?: IActionModel,
    userInput?: string,
    onContextCreationHandler?: onContextCreationHandler): PromiseLike<IActionModel>;

declare type onContextCreationHandler = (action: IAction, actionModel: IActionModel, next: () => void) => void

export function bindToBotDialog(
    bot: UniversalBot,
    intentDialog: IntentDialog,
    modelUrl: string,
    actions: Array<IAction>,
    options: IBindToDialogOptions
): void

declare type onDialogContextCreationHandler = (action: IAction, actionModel: IActionModel, next: () => void, session: Session) => void
declare type replyHandler = (session: Session) => void
declare type fulfillHandler = (session: Session, actionModel: IActionModel) => void

export interface IBindToDialogOptions {
    defaultReply: replyHandler,
    fulfillReply: fulfillHandler,
    onContextCreation: onDialogContextCreationHandler    
}

// TYPES
export interface IAction {
    intentName: string;
    friendlyName: string;
    confirmOnContextSwitch?: boolean;
    canExecuteWithoutContext?: boolean;
    parentAction?: IAction,
    schema: { [key: string]: ISchemaParameter };
    fulfill: (parameters: any, callback: (result: any) => void) => void;
}

export interface ISchemaParameter {
    type: string;
    builtInType?: string;
    message: string;
}

export interface IActionModel {
    status: string;
    intentName: string;
    result?: any;
    userInput?: string;
    currentParameter?: string;
    parameters: { [key: string]: any };
    parameterErrors: Array<IParameterError>;
    contextSwitchPrompt?: string,
    confirmSwitch?: boolean;
    subcontextResult?: any;
}

export interface IParameterError {
    parameterName: string;
    message: string;
}