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

namespace WhenDoJobs.Core.Tests
{
    [TestClass]
    public class JobExecutorTest
    {
        [TestMethod]
        public async Task AllCommandsShouldBeRun()
        {
            var logger = MockHelper.CreateLogger<LoggingCommandHandler>();
            var registryMock = new Mock<IWhenDoRegistry>();
            registryMock.Setup(x => x.GetCommandHandler(It.IsAny<string>())).Returns(new LoggingCommandHandler(logger));

            var job = CreateJob();
            Assert.IsTrue(job.Evaluate(new TestMessage()));

            IWhenDoExecutor executor = new WhenDoExecutor(registryMock.Object);
            await executor.ExecuteJobAsync(job);

            Assert.AreEqual(2, logger.Count);
        }

        [TestMethod]
        public async Task InvalidCommandShouldRaiseNiceException()
        {
            var logger = MockHelper.CreateLogger<LoggingCommandHandler>();
            var registryMock = new Mock<IWhenDoRegistry>();
            registryMock.Setup(x => x.GetCommandHandler(It.IsAny<string>())).Returns(new LoggingCommandHandler(logger));

            var job = CreateJobWithInvalidCommand();
            Assert.IsTrue(job.Evaluate(new TestMessage()));

            IWhenDoExecutor executor = new WhenDoExecutor(registryMock.Object);

            await Assert.ThrowsExceptionAsync<InvalidCommandException>(async () => await executor.ExecuteJobAsync(job));
        }

        [TestMethod]
        public async Task ExecuteDelayedCommand()
        {
            var command = new Command()
            {
                Type = "Logging",
                MethodName = "LogError",
                Parameters = new Dictionary<string, object>() { { "text", "this is a delayed command"} },
                ExecutionStrategy = new ExecutionStrategy() { Mode = ExecutionMode.Delayed, Time = TimeSpan.FromSeconds(10) }
            };

            var logger = MockHelper.CreateLogger<LoggingCommandHandler>();
            var registryMock = new Mock<IWhenDoRegistry>();
            registryMock.Setup(x => x.GetCommandHandler(It.IsAny<string>())).Returns(new LoggingCommandHandler(logger));
            IWhenDoExecutor executor = new WhenDoExecutor(registryMock.Object);

            await executor.ExecuteCommandAsync(command);
            Assert.AreEqual(0, logger.Count);

            await Task.Delay(TimeSpan.FromSeconds(13));
            Assert.AreEqual(1, logger.Count);
        }

        private IJob CreateJob()
        {
            var job = new Job<TestMessage>();
            job.Condition = WhenDoHelpers.ParseExpression("true", "Test", typeof(TestMessage));

            var command1 = new Command()
            {
                Type = "Logging",
                MethodName = "LogError"
            };
            command1.Parameters.Add("text", "unit test command 1");

            var command2 = new Command()
            {
                Type = "Logging",
                MethodName = "LogError"
            };
            command2.Parameters.Add("text", "unit test command 2");

            job.Commands = new List<ICommand>() { command1, command2 };

            return job;
        }

        private IJob CreateJobWithInvalidCommand()
        {
            var job = new Job<TestMessage>();
            job.Condition = WhenDoHelpers.ParseExpression("true", "Test", typeof(TestMessage));

            var command1 = new Command()
            {
                Type = "Logging",
                MethodName = "LogError"
            };
            command1.Parameters.Add("texxt", "unit test command 1"); //texxt is invalid

            job.Commands = new List<ICommand>() { command1};

            return job;
        }

    }
}
