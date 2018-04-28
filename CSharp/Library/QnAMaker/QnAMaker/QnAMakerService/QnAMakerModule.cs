// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Bot Framework: http://botframework.com
// 
// Cognitive Services based Dialogs for Bot Builder:
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

using Autofac;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace Microsoft.Bot.Builder.CognitiveServices.QnAMaker
{
    public sealed class QnAMakerModule : Module
    {
        string authKey;
        string kbid;
        string defaultMessage;
        string endpointHostName;
        double threshold;
        int top;

        public QnAMakerModule(string authKey, string kbid, string defaultMessage, double threshold, int top = 1, string endpointHostName = null)
        {
            this.authKey = authKey;
            this.kbid = kbid;
            this.endpointHostName = endpointHostName;
            this.defaultMessage = defaultMessage;
            this.threshold = threshold;
            this.top = top;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .Register(c => new QnAMakerAttribute(authKey, kbid, defaultMessage, threshold, top, endpointHostName))
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterType<QnAMakerService>()
                .Keyed<IQnAService>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .Register(c => new QnAMakerScorable(
                    new QnAMakerServiceScorable(c.Resolve<IQnAService>(), c.Resolve<IBotToUser>()),
                    c.Resolve<ITraits<double>>()))
                .AsImplementedInterfaces()
                .InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
        }
    }
}
