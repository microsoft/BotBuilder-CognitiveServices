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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Dialogs;

using Moq;
using Autofac;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Bot.Builder.CognitiveServices.QnAMaker.Tests
{
    [TestClass]
    public abstract class FiberTestBase
    {
        public struct C
        {
        }

        public static readonly C Context = default(C);
        public static readonly CancellationToken Token = new CancellationTokenSource().Token;

        public interface IMethod
        {
            Task<IWait<C>> CodeAsync<T>(IFiber<C> fiber, C context, IAwaitable<T> item, CancellationToken token);
        }

        public static Moq.Mock<IMethod> MockMethod()
        {
            var method = new Moq.Mock<IMethod>(Moq.MockBehavior.Loose);
            return method;
        }

        public static Expression<Func<IAwaitable<T>, bool>> Item<T>(T value)
        {
            return item => value.Equals(item.GetAwaiter().GetResult());
        }

        protected sealed class CodeException : Exception
        {
        }

        public static bool ExceptionOfType<T, E>(IAwaitable<T> item) where E : Exception
        {
            try
            {
                item.GetAwaiter().GetResult();
                return false;
            }
            catch (E)
            {
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Expression<Func<IAwaitable<T>, bool>> ExceptionOfType<T, E>() where E : Exception
        {
            return item => ExceptionOfType<T, E>(item);
        }

        public static async Task PollAsync(IFiberLoop<C> fiber)
        {
            IWait wait;
            do
            {
                wait = await fiber.PollAsync(Context, Token);
            }
            while (wait.Need != Need.None && wait.Need != Need.Done);
        }

        public static IContainer Build()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new FiberModule<C>());
            return builder.Build();
        }

        public sealed class ResolveMoqAssembly : IDisposable
        {
            private readonly object[] instances;
            public ResolveMoqAssembly(params object[] instances)
            {
                SetField.NotNull(out this.instances, nameof(instances), instances);

                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            }
            void IDisposable.Dispose()
            {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            }
            private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs arguments)
            {
                foreach (var instance in instances)
                {
                    var type = instance.GetType();
                    if (arguments.Name == type.Assembly.FullName)
                    {
                        return type.Assembly;
                    }
                }

                return null;
            }
        }

        public static void AssertSerializable<T>(ILifetimeScope scope, ref T item) where T : class
        {
            var formatter = scope.Resolve<IFormatter>();

            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, item);
                stream.Position = 0;
                item = (T)formatter.Deserialize(stream);
            }
        }
    }
}
