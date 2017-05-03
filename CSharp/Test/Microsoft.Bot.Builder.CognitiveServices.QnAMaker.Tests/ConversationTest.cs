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
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Autofac;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Microsoft.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Bot.Builder.CognitiveServices.QnAMaker.Tests
{
    public class MockConnectorFactory : IConnectorClientFactory
    {
        protected readonly IBotDataStore<BotData> memoryDataStore = new InMemoryDataStore();
        protected readonly string botId;
        public StateClient StateClient;

        public MockConnectorFactory(string botId)
        {
            SetField.NotNull(out this.botId, nameof(botId), botId);
        }

        public IConnectorClient MakeConnectorClient()
        {
            var client = new Mock<ConnectorClient>();
            client.CallBase = true;
            return client.Object;
        }

        public IStateClient MakeStateClient()
        {
            if (this.StateClient == null)
            {
                this.StateClient = MockIBots(this).Object;
            }
            return this.StateClient;
        }

        protected IAddress AddressFrom(string channelId, string userId, string conversationId)
        {
            var address = new Address
            (
                this.botId,
                channelId,
                userId ?? "AllUsers",
                conversationId ?? "AllConversations",
                "InvalidServiceUrl"
            );
            return address;
        }
        protected async Task<HttpOperationResponse<object>> UpsertData(string channelId, string userId, string conversationId, BotStoreType storeType, BotData data)
        {
            var _result = new HttpOperationResponse<object>();
            _result.Request = new HttpRequestMessage();
            try
            {
                var address = AddressFrom(channelId, userId, conversationId);
                await memoryDataStore.SaveAsync(address, storeType, data, CancellationToken.None);
            }
            catch (HttpException e)
            {
                _result.Body = e.Data;
                _result.Response = new HttpResponseMessage { StatusCode = HttpStatusCode.PreconditionFailed };
                return _result;
            }
            catch (Exception)
            {
                _result.Response = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
                return _result;
            }

            _result.Body = data;
            _result.Response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            return _result;
        }

        protected async Task<HttpOperationResponse<object>> GetData(string channelId, string userId, string conversationId, BotStoreType storeType)
        {
            var _result = new HttpOperationResponse<object>();
            _result.Request = new HttpRequestMessage();
            BotData data;
            var address = AddressFrom(channelId, userId, conversationId);
            data = await memoryDataStore.LoadAsync(address, storeType, CancellationToken.None);
            _result.Body = data;
            _result.Response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            return _result;
        }

        public Mock<StateClient> MockIBots(MockConnectorFactory mockConnectorFactory)
        {
            var botsClient = new Moq.Mock<StateClient>(MockBehavior.Loose);

            botsClient.Setup(d => d.BotState.SetConversationDataWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BotData>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, BotData, Dictionary<string, List<string>>, CancellationToken>(async (channelId, conversationId, data, headers, token) =>
                {
                    return await mockConnectorFactory.UpsertData(channelId, null, conversationId, BotStoreType.BotConversationData, data);
                });

            botsClient.Setup(d => d.BotState.GetConversationDataWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, Dictionary<string, List<string>>, CancellationToken>(async (channelId, conversationId, headers, token) =>
                {
                    return await mockConnectorFactory.GetData(channelId, null, conversationId, BotStoreType.BotConversationData);
                });


            botsClient.Setup(d => d.BotState.SetUserDataWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BotData>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
              .Returns<string, string, BotData, Dictionary<string, List<string>>, CancellationToken>(async (channelId, userId, data, headers, token) =>
              {
                  return await mockConnectorFactory.UpsertData(channelId, userId, null, BotStoreType.BotUserData, data);
              });

            botsClient.Setup(d => d.BotState.GetUserDataWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, Dictionary<string, List<string>>, CancellationToken>(async (channelId, userId, headers, token) =>
                {
                    return await mockConnectorFactory.GetData(channelId, userId, null, BotStoreType.BotUserData);
                });

            botsClient.Setup(d => d.BotState.SetPrivateConversationDataWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BotData>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
             .Returns<string, string, string, BotData, Dictionary<string, List<string>>, CancellationToken>(async (channelId, conversationId, userId, data, headers, token) =>
             {
                 return await mockConnectorFactory.UpsertData(channelId, userId, conversationId, BotStoreType.BotPrivateConversationData, data);
             });

            botsClient.Setup(d => d.BotState.GetPrivateConversationDataWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
             .Returns<string, string, string, Dictionary<string, List<string>>, CancellationToken>(async (channelId, conversationId, userId, headers, token) =>
             {
                 return await mockConnectorFactory.GetData(channelId, userId, conversationId, BotStoreType.BotPrivateConversationData);
             });

            return botsClient;
        }
    }

    public abstract class ConversationTestBase
    {
        [Flags]
        public enum Options { None, InMemoryBotDataStore };

        public static IContainer Build(Options options, params object[] singletons)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DialogModule_MakeRoot());

            // make a "singleton" MockConnectorFactory per unit test execution
            IConnectorClientFactory factory = null;
            builder
                .Register((c, p) => factory ?? (factory = new MockConnectorFactory(c.Resolve<IAddress>().BotId)))
                .As<IConnectorClientFactory>()
                .InstancePerLifetimeScope();

            var r =
              builder
              .Register<Queue<IMessageActivity>>(c => new Queue<IMessageActivity>())
              .AsSelf()
              .InstancePerLifetimeScope();

            builder
                .RegisterType<BotToUserQueue>()
                .AsSelf()
                .As<IBotToUser>()
                .InstancePerLifetimeScope();

            if (options.HasFlag(Options.InMemoryBotDataStore))
            {
                //Note: memory store will be single instance for the bot
                builder.RegisterType<InMemoryDataStore>()
                    .AsSelf()
                    .SingleInstance();

                builder.Register(c => new CachingBotDataStore(c.Resolve<InMemoryDataStore>(), CachingBotDataStoreConsistencyPolicy.ETagBasedConsistency))
                    .As<IBotDataStore<BotData>>()
                    .AsSelf()
                    .InstancePerLifetimeScope();
            }

            foreach (var singleton in singletons)
            {
                builder
                    .Register(c => singleton)
                    .Keyed(FiberModule.Key_DoNotSerialize, singleton.GetType());
            }

            return builder.Build();
        }
    }
}
