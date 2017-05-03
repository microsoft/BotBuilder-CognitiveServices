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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Bot.Builder.Dialogs;
using Moq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Autofac;
using Microsoft.Bot.Builder.Dialogs.Internals;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Bot.Builder.CognitiveServices.QnAMaker.Tests
{
    [TestClass]
    public sealed class QnAMakerTests : DialogTestBase
    {
        public sealed class QnADialog : QnAMakerDialog
        {
            public QnADialog(params IQnAService[] services)
                : base(services)
            { }
        }

       [TestMethod]
        public async Task QnATests()
        {

            string text = "asd";
            Uri uri = new Uri("http://test");
            QnAMakerRequestBody postBody = new QnAMakerRequestBody();
            string subscriptionKey;

            QnAMakerResults qnaResults1 =
                JsonConvert.DeserializeObject<QnAMakerResults>(
                    "{\"answers\":[{\"answer\":\"Test answer 1.1\",\"questions\":[\"Test question 1.1\",\"Test alternate phrase 1.1\"],\"score\":88.5},{\"answer\":\"Test answer 1.2\",\"questions\":[\"Test question 1.2\",\"Test alternate phrase 1.2\"],\"score\":58.3},{\"answer\":\"Test answer 1.3\",\"questions\":[\"Test question 1.3\",\"Test alternate phrase 1.3\"],\"score\":42.7}]}");
            string expectedResponse1 =
                "Test answer 1.1";
            Mock<IQnAService> qnaServiceMock1 = new Mock<IQnAService>();
            qnaServiceMock1.Setup(x => x.QueryServiceAsync(It.IsAny<Uri>(), It.IsAny<QnAMakerRequestBody>(), It.IsAny<string>())).ReturnsAsync(qnaResults1);
            
            QnAMakerResults qnaResults2 =
                JsonConvert.DeserializeObject<QnAMakerResults>(
                    "{\"answers\":[{\"answer\":\"Test answer 2.1\",\"questions\":[\"Test question 2.1\",\"Test alternate phrase 2.1\"],\"score\":98.5},{\"answer\":\"Test answer 2.2\",\"questions\":[\"Test question 2.2\",\"Test alternate phrase 2.2\"],\"score\":78.3},{\"answer\":\"Test answer 2.3\",\"questions\":[\"Test question 2.3\",\"Test alternate phrase 2.3\"],\"score\":40.5}]}");
            string expectedResponse2 =
                "Test answer 2.1";
            Mock<IQnAService> qnaServiceMock2 = new Mock<IQnAService>();
            qnaServiceMock2.Setup(x => x.QueryServiceAsync(It.IsAny<Uri>(), It.IsAny<QnAMakerRequestBody>(), It.IsAny<string>())).ReturnsAsync(qnaResults2);
            
            var qnaDialog1 = new QnADialog(qnaServiceMock1.Object);
            await this.TestQnADialogResponse(qnaDialog1, expectedResponse1);

            var qnaDialog2 = new QnADialog(qnaServiceMock1.Object, qnaServiceMock2.Object);
            await this.TestQnADialogResponse(qnaDialog2, expectedResponse2);

            return;
        }

        private async Task TestQnADialogResponse(IDialog<object> qnaDialog, string expectedResponse)
        {
            // arrange
            var toBot = DialogTestBase.MakeTestMessage();
            toBot.From.Id = Guid.NewGuid().ToString();
            toBot.Text = "any query text";

            Func<IDialog<object>> MakeRoot = () => qnaDialog;

            using (new FiberTestBase.ResolveMoqAssembly(qnaDialog))
            using (var container = Build(Options.MockConnectorFactory | Options.ScopedQueue, qnaDialog))
            {
                // act: sending the message
                IMessageActivity toUser = null;
                toUser = await GetResponse(container, MakeRoot, toBot);
            
                // assert: check if the dialog returned the right response
                Assert.IsTrue(toUser.Text.Equals(expectedResponse));
            }
        }
        
        private async Task<IMessageActivity> GetResponse(
            IContainer container,
            Func<IDialog<object>> makeRoot,
            IMessageActivity toBot)
        {
            using (var scope = DialogModule.BeginLifetimeScope(container, toBot))
            {
                DialogModule_MakeRoot.Register(scope, makeRoot);

                // act: sending the message
                var task = scope.Resolve<IPostToBot>();
                CancellationToken token = default(CancellationToken);
                await task.PostAsync(toBot, token);
                return scope.Resolve<Queue<IMessageActivity>>().Dequeue();
            }
        }
    }

}
