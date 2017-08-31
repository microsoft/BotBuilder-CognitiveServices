﻿// 
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
    /// <summary>An Autofac module for a QnA Maker definition</summary>
    /// <seealso cref="Autofac.Module" />
    public sealed class QnAMakerModule : Module
    {
        string subscriptionKey;
        string kbid;
        string defaultMessage;
        double threshold;
        int top;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnAMakerModule" /> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <param name="kbid">The kbid.</param>
        /// <param name="defaultMessage">The default message.</param>
        /// <param name="threshold">The threshold.</param>
        /// <param name="top">The top.</param>
        public QnAMakerModule(string subscriptionKey, string kbid, string defaultMessage, double threshold, int top = 1)
        {
            this.subscriptionKey = subscriptionKey;
            this.kbid = kbid;
            this.defaultMessage = defaultMessage;
            this.threshold = threshold;
            this.top = top;
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be
        /// registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .Register(c => new QnAMakerAttribute(subscriptionKey, kbid, defaultMessage, threshold, top))
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
