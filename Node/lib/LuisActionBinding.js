var _ = require('lodash');
var util = require('util');
var builder = require('botbuilder');
var inspector = require('schema-inspector');
var Promise = require('bluebird');
var BuiltInTypes = require('./LuisBuiltInTypes');
var Status = {
    NoActionRecognized: 'NoActionRecognized',
    Fulfilled: 'Fulfilled',
    MissingParameters: 'MissingParameters',
    ContextSwitch: 'ContextSwitch'
};
module.exports = {
    Status: Status,
    BuiltInTypes: BuiltInTypes,
    evaluate: evaluate,
    bindToBotDialog: bindToBotDialog
};
var EmptyActionModel = {
    status: Status.NoActionRecognized,
    intentName: null,
    result: null,
    userInput: null,
    currentParameter: null,
    parameters: {},
    parameterErrors: []
};
function evaluate(modelUrl, actions, currentActionModel, userInput, onContextCreationHandler) {
    if (!modelUrl) {
        throw new Error('modelUrl not set');
    }
    actions.forEach(validateAction);
    onContextCreationHandler = validateContextCreationHandler(onContextCreationHandler);
    return new Promise(function (resolve, reject) {
        var actionModel = _.merge({}, EmptyActionModel, currentActionModel);
        if (actionModel.status === Status.ContextSwitch) {
            if (actionModel.confirmSwitch) {
                actionModel.intentName = actionModel.contextSwitchData.intentName;
                actionModel.parameters = actionModel.contextSwitchData.parameters;
            }
            actionModel.contextSwitchData = null;
            actionModel.currentParameter = null;
            actionModel.userInput = null;
        }
        actionModel.userInput = userInput ? userInput.trim() : null;
        delete actionModel.subcontextResult;
        switch (actionModel.status) {
            case Status.NoActionRecognized:
                builder.LuisRecognizer.recognize(actionModel.userInput, modelUrl, function (err, intents, entities) {
                    if (err) {
                        return reject(err);
                    }
                    var action = chooseBestIntentAction(intents, actions);
                    if (action) {
                        actionModel.intentName = action.intentName;
                        if (action.parentAction && !actionModel.contextModel) {
                            popupateContextParent(actionModel, action, actions);
                        }
                        actionModel.parameters = extractParametersFromEntities(action.schema, entities);
                        var next = function () {
                            tryExecute(action, actionModel)
                                .then(resolve)
                                .catch(reject);
                        };
                        if (action.parentAction) {
                            onContextCreationHandler(action.parentAction, actionModel.contextModel, next);
                        }
                        else {
                            next();
                        }
                    }
                    else {
                        actionModel.status = Status.NoActionRecognized;
                        resolve(actionModel);
                    }
                });
                break;
            case Status.MissingParameters:
            case Status.ContextSwitch:
                var action = _.find(actions, function (action) { return actionModel.intentName === action.intentName; });
                if (actionModel.userInput) {
                    builder.LuisRecognizer.recognize(actionModel.userInput, modelUrl, function (err, intents, entities) {
                        if (err) {
                            return reject(err);
                        }
                        var newAction = chooseBestIntentAction(intents, actions, action);
                        if (newAction && newAction.intentName !== action.intentName) {
                            if (newAction.parentAction === action) {
                                actionModel = _.merge({}, EmptyActionModel, {
                                    contextModel: actionModel,
                                    intentName: newAction.intentName
                                });
                                actionModel.parameters = [];
                                action = newAction;
                            }
                            else if (equalsTrue(action.confirmOnContextSwitch, true)) {
                                actionModel.status = Status.ContextSwitch;
                                actionModel.contextSwitchData = {
                                    intentName: newAction.intentName,
                                    parameters: extractParametersFromEntities(newAction.schema, entities)
                                };
                                var currentActionName = action.friendlyName || action.intentName;
                                var newActionName = newAction.friendlyName || newAction.intentName;
                                actionModel.contextSwitchPrompt = util.format('Do you want to discard the current action \'%s\' and start the with \'%s\' action?', currentActionName, newActionName);
                                return resolve(actionModel);
                            }
                            else {
                                action = newAction;
                                actionModel.intentName = newAction.intentName;
                                actionModel.currentParameter = null;
                            }
                        }
                        var parameters = extractParametersFromEntities(action.schema, entities, actionModel);
                        actionModel.parameters = _.merge({}, actionModel.parameters, parameters);
                        tryExecute(action, actionModel)
                            .then(resolve)
                            .catch(reject);
                    });
                }
                else {
                    tryExecute(action, actionModel)
                        .then(resolve)
                        .catch(reject);
                }
                break;
            default:
                reject('Unknown action.status "' + actionModel.status + '"');
        }
    });
}
function bindToBotDialog(bot, intentDialog, modelUrl, actions, options) {
    if (!bot) {
        throw new Error('bot is required');
    }
    if (!intentDialog) {
        throw new Error('intentDialog is required');
    }
    if (!modelUrl) {
        throw new Error('ModelUrl is required');
    }
    options = options || {};
    bot.set('persistConversationData', true);
    bot.library(createBotLibrary(modelUrl, actions, options));
    _.forEach(actions, function (action) {
        intentDialog.matches(action.intentName, createBotAction(action, modelUrl));
    });
}
function createBotLibrary(modelUrl, actions, options) {
    var defaultReplyHandler = typeof options.defaultReply === 'function' ? options.defaultReply : function (session) { session.endDialog('Sorry, I didn\'t understand that.'); };
    var fulfillReplyHandler = typeof options.fulfillReply === 'function' ? options.fulfillReply : function (session, actionModel) { session.endDialog(actionModel.result.toString()); };
    var onContextCreationHandler = validateContextCreationHandler(options.onContextCreation);
    var lib = new builder.Library('LuisActions');
    lib.dialog('Evaluate', new builder.SimpleDialog(function (session, args) {
        var actionModel = null;
        var action = null;
        if (args && args.intents) {
            action = chooseBestIntentAction(args.intents, actions);
            if (!action) {
                return defaultReplyHandler(session);
            }
            actionModel = _.merge({}, EmptyActionModel, {
                intentName: action.intentName
            });
            if (action.parentAction && !actionModel.contextModel) {
                popupateContextParent(actionModel, action, actions);
            }
            actionModel.parameters = extractParametersFromEntities(action.schema, args.entities);
            if (action.parentAction) {
                return onContextCreationHandler(action.parentAction, actionModel.contextModel, next, session);
            }
        }
        else {
            actionModel = session.privateConversationData['luisaction.model'];
        }
        next();
        function next() {
            if (!actionModel) {
                return defaultReplyHandler(session);
            }
            action = actions.find(function (a) { return a.intentName === actionModel.intentName; });
            if (!action) {
                return defaultReplyHandler(session);
            }
            var operation = null;
            if (actionModel.status === Status.ContextSwitch && args.response === true) {
                actionModel.confirmSwitch = true;
                operation = evaluate(modelUrl, actions, actionModel);
            }
            else if (args && args.response && actionModel.currentParameter) {
                operation = evaluate(modelUrl, actions, actionModel, args.response);
            }
            else {
                operation = tryExecute(action, actionModel);
            }
            operation.then(function (actionModel) {
                session.privateConversationData['luisaction.model'] = actionModel;
                if (actionModel.subcontextResult) {
                    session.send(actionModel.subcontextResult.toString());
                }
                switch (actionModel.status) {
                    case Status.MissingParameters:
                        var errors = actionModel.parameterErrors;
                        var firstError = _.first(errors);
                        actionModel.currentParameter = firstError.parameterName;
                        session.privateConversationData['luisaction.model'] = actionModel;
                        builder.Prompts.text(session, firstError.message);
                        break;
                    case Status.ContextSwitch:
                        var prompt = actionModel.contextSwitchPrompt;
                        session.privateConversationData['luisaction.model'] = actionModel;
                        builder.Prompts.confirm(session, prompt, { listStyle: builder.ListStyle.button });
                        break;
                    case Status.Fulfilled:
                        delete session.privateConversationData['luisaction.model'];
                        fulfillReplyHandler(session, actionModel);
                        break;
                }
            }).catch(function (err) {
                session.endDialog('Error: %s', err);
            });
        }
    }));
    return lib;
}
function createBotAction(action, modelUrl) {
    validateAction(action);
    return function (session, dialogArgs) {
        session.beginDialog('LuisActions:Evaluate', dialogArgs);
    };
}
function chooseBestIntentAction(intents, actions, currentAction) {
    var intent = _.maxBy(intents, function (intent) { return intent.score; });
    var action = _.find(actions, function (action) { return intent && intent.intent === action.intentName; });
    if (action && currentAction && action.parentAction && action.parentAction !== currentAction) {
        return null;
    }
    if (action && action.parentAction && (!equalsTrue(action.canExecuteWithoutContext, true) && action.parentAction !== currentAction)) {
        return null;
    }
    return action;
}
function extractParametersFromEntities(schema, entities, actionModel) {
    if (actionModel && actionModel.currentParameter && schema[actionModel.currentParameter]) {
        var currentParameterSchema = schema[actionModel.currentParameter];
        var entity = null;
        if (currentParameterSchema.customType) {
            entity = entities.find(function (e) { return e.type === currentParameterSchema.customType; });
        }
        if (!entity) {
            entity = entities.find(function (e) { return e.type === actionModel.currentParameter; });
        }
        if (!entity && currentParameterSchema.builtInType) {
            entity = entities.find(function (e) { return e.type === currentParameterSchema.builtInType; });
        }
        if (!entity) {
            entity = { entity: actionModel.userInput };
        }
        entity = _.merge({}, entity, { type: actionModel.currentParameter });
        entities = entities.concat([entity]);
    }
    entities = crossMatchEntities(entities);
    var parameters = _.reduce(entities, function (merged, entity) {
        merged[entity.type] = entity.entity;
        return merged;
    }, {});
    var schemaObject = wrap(schema);
    inspector.sanitize(schemaObject, parameters);
    var result = inspector.validate(schemaObject, parameters);
    if (!result.valid) {
        var invalidParameterNames = result.error.map(getParameterName);
        parameters = _.omit(parameters, invalidParameterNames);
    }
    return parameters;
}
function tryExecute(action, actionModel) {
    return new Promise(function (resolve, reject) {
        try {
            validate(action.schema, actionModel.parameters, function (parameters, errors) {
                actionModel.status = Status.MissingParameters;
                actionModel.parameters = parameters;
                actionModel.parameterErrors = errors;
                resolve(actionModel);
            }, function (completeParameters) {
                var parentContext = actionModel.contextModel;
                action.fulfill(completeParameters, function (fulfillResult) {
                    actionModel.status = Status.Fulfilled;
                    actionModel.result = fulfillResult;
                    actionModel.parameters = completeParameters;
                    actionModel.parameterErrors = [];
                    if (actionModel.contextModel) {
                        actionModel.contextModel.subcontextResult = actionModel.result;
                        actionModel = actionModel.contextModel;
                        tryExecute(action.parentAction, actionModel)
                            .then(resolve)
                            .catch(reject);
                    }
                    else {
                        resolve(actionModel);
                    }
                }, parentContext ? parentContext.parameters : {});
            });
        }
        catch (err) {
            reject(err);
        }
    });
}
function validate(schema, parameters, onValidationErrors, onValidationPass) {
    var schemaObject = wrap(schema);
    inspector.sanitize(schemaObject, parameters);
    var result = inspector.validate(schemaObject, parameters);
    if (result.valid) {
        onValidationPass(parameters);
    }
    else {
        var errors = result.error.map(function (fieldError) {
            var parameterName = getParameterName(fieldError);
            var errorMessage = schema[parameterName].message;
            return {
                parameterName: parameterName,
                message: errorMessage
            };
        });
        onValidationErrors(parameters, errors);
    }
}
function popupateContextParent(actionModel, currentAction) {
    if (!currentAction.parentAction) {
        return actionModel;
    }
    actionModel.contextModel = _.merge({}, EmptyActionModel, {
        intentName: currentAction.parentAction.intentName,
        status: Status.MissingParameters
    });
    actionModel.parameters = {};
    actionModel.parameterErrors = [];
    actionModel.result = null;
    return actionModel;
}
function crossMatchEntities(entities) {
    var groups = _.groupBy(entities, function (entity) {
        return entity.entity;
    });
    var result = [];
    _.forOwn(groups, function (matches, entity) {
        if (matches.length > 1) {
            var entityTarget = matches.find(function (e) { return e.type.indexOf('builtin.') === -1; });
            var entityWithValue = matches.find(function (e) { return e.resolution; });
            if (entityWithValue) {
                var resolution = entityWithValue.resolution;
                entityTarget.entity = resolution[_.keys(resolution)[0]];
            }
            if (entityTarget && entityTarget.entity && entityTarget.entity instanceof Array) {
                var first = entityTarget.entity[0];
                entityTarget.entity = first.value || first;
            }
            if (entityTarget) {
                result.push(entityTarget);
            }
        }
        else {
            result.push(matches[0]);
        }
    });
    return result;
}
function getParameterName(fieldError) {
    return _.last(fieldError.property.split('.'));
}
function wrap(propertiesSchema) {
    return {
        type: 'object',
        properties: propertiesSchema
    };
}
function equalsTrue(value, valueForUndefined) {
    if (value === undefined || value === null) {
        return valueForUndefined === true;
    }
    return value === true;
}
function validateContextCreationHandler(callback) {
    return typeof callback === 'function'
        ? callback
        : function (action, actionModel, next) { next(); };
}
function validateAction(action) {
    if (typeof action.intentName !== 'string') {
        throw new Error('actionModel.intentName requires a string');
    }
    if (typeof action.friendlyName !== 'string') {
        throw new Error('actionModel.friendlyName requires a string');
    }
    if (typeof action.schema !== 'object') {
        throw new Error('actionModel.schema requires a schema of properties');
    }
    if (typeof action.fulfill !== 'function') {
        throw new Error('actionModel.fulfill should be a function');
    }
}
