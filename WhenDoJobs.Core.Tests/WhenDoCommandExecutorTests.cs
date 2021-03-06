﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Exceptions;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;
using WhenDoJobs.Core.Persistence;
using WhenDoJobs.Core.Services;
using WhenDoJobs.Core.Tests.Helpers;

namespace WhenDoJobs.Core.Tests
{
    [TestClass]
    public class WhenDoCommandExecutorTests
    {
        [TestMethod]
        public async Task InvalidCommandShouldRaiseNiceException()
        {
            var handlerMock = new Mock<ILoggingCommandHandler>();
            var repo = new MemoryJobRepository();
            //repo.Save()
            var executor = new WhenDoCommandExecutor(MockHelper.CreateRegistryMock(handlerMock.Object).Object, new MemoryJobRepository(), MockHelper.CreateLogger<WhenDoCommandExecutor>());


            //await Assert.ThrowsExceptionAsync<InvalidCommandException>(async () =>
            //            await executor.ExecuteAsync(new TestMessage(), "Logging", "LogError", new Dictionary<string, object>() { { "texxxt", "unit test command 1" } }));

            //await Assert.ThrowsExceptionAsync<InvalidCommandException>(async () =>
            //            await executor.ExecuteAsync(new TestMessage(), "Logging", "LogError",
            //                new Dictionary<string, object>()
            //                    {
            //                        { "text", "unit test command 1" },
            //                        {"bla", "another parameter" }
            //                    }
            //                ));
        }
    }
}
