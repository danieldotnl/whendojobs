using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Commands;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;
using WhenDoJobs.Core.Services;
using WhenDoJobs.Core.Exceptions;
using WhenDoJobs.Core.Tests.Helpers;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;

namespace WhenDoJobs.Core.Tests
{
    [TestClass]
    public class WhenDoJobExecutorTest
    {
        [TestMethod]
        public async Task ExecuteDelayedCommandInJob()
        {
            var command = new Command()
            {
                Type = "Logging",
                MethodName = "LogError",
                Parameters = new Dictionary<string, object>() { { "text", "this is a delayed command" } },
                ExecutionStrategy = new ExecutionStrategy() { Mode = ExecutionMode.Delayed, Time = TimeSpan.FromSeconds(10) }
            };
            var job = new Job<TestMessage>();
            job.Commands = new List<IWhenDoCommand>() { command };

            var joblogger = MockHelper.CreateLogger<WhenDoJobExecutor>();
            var handlerlogger = MockHelper.CreateLogger<LoggingCommandHandler>();
            var registryMock = new Mock<IWhenDoRegistry>();
            registryMock.Setup(x => x.GetCommandHandler(It.IsAny<string>())).Returns(new LoggingCommandHandler(handlerlogger));
            var hangfireMock = new Mock<IBackgroundJobClient>();

            var jobExecutor = new WhenDoJobExecutor(new Mock<IServiceProvider>().Object, hangfireMock.Object, joblogger);
            await jobExecutor.ExecuteAsync(job);

            Assert.AreEqual(0, handlerlogger.Count);
            hangfireMock.Verify(x => x.Create(It.IsAny<Job>(), It.IsAny<ScheduledState>()));
        }

        [TestMethod]
        public async Task AllCommandsInJobShouldBeRun()
        {
            var job = CreateJob();
            Assert.IsTrue(job.Evaluate(new TestMessage()));

            var joblogger = MockHelper.CreateLogger<WhenDoJobExecutor>();

            var hangfireMock = new Mock<IBackgroundJobClient>();

            var handlerMock = new Mock<ILoggingCommandHandler>();
            handlerMock.Setup(x => x.LogError(It.IsAny<string>()));
            handlerMock.Setup(x => x.LogWarning(It.IsAny<string>()));

            var commandExecutorMock = new Mock<IWhenDoCommandExecutor>();
            commandExecutorMock.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>())).Returns(Task.CompletedTask);

            //var cmdexecutor = new WhenDoCommandExecutor(MockHelper.CreateRegistryMock(handlerMock.Object).Object, MockHelper.CreateLogger<WhenDoCommandExecutor>());
            IWhenDoJobExecutor executor = new WhenDoJobExecutor(MockHelper.CreateServiceProviderMock(commandExecutorMock.Object).Object, hangfireMock.Object, joblogger);

            await executor.ExecuteAsync(job);

            commandExecutorMock.Verify(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Exactly(2));
        }        

        private IWhenDoJob CreateJob()
        {
            var job = new Job<TestMessage>();
            job.Condition = WhenDoHelpers.ParseExpression("true", "Test", typeof(TestMessage));

            var command1 = new Command()
            {
                Type = "Logging",
                MethodName = "LogError",
                Parameters = new Dictionary<string, object>() { { "text", "unit test error command 1" } }
            };

            var command2 = new Command()
            {
                Type = "Logging",
                MethodName = "LogWarning",
                Parameters = new Dictionary<string, object>() { { "text", "unit test warning command 2" } }
            };

            job.Commands = new List<IWhenDoCommand>() { command1, command2 };

            return job;
        }

        private IWhenDoJob CreateJobWithInvalidCommand()
        {
            var job = new Job<TestMessage>();
            job.Condition = WhenDoHelpers.ParseExpression("true", "Test", typeof(TestMessage));

            var command1 = new Command()
            {
                Type = "Logging",
                MethodName = "LogError"
            };
            command1.Parameters.Add("texxt", "unit test command 1"); //texxt is invalid

            job.Commands = new List<IWhenDoCommand>() { command1};

            return job;
        }
    }
}