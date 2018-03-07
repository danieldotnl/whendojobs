using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;
using WhenDoJobs.Core.Services;
using WhenDoJobs.Core.Exceptions;
using WhenDoJobs.Core.Tests.Helpers;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using WhenDoJobs.Core.Persistence;

namespace WhenDoJobs.Core.Tests
{
    [TestClass]
    public class WhenDoJobExecutorTest
    {
        [TestMethod]
        public void ExecuteDelayedCommandInJob()
        {
            var command = new WhenDoCommand()
            {
                Type = "Logging",
                MethodName = "LogError",
                Parameters = new Dictionary<string, object>() { { "text", "this is a delayed command" } },
                ExecutionStrategy = new ExecutionStrategy() { Mode = ExecutionMode.Delayed, Time = TimeSpan.FromSeconds(10) }
            };
            var job = new WhenDoJob();
            job.Commands = new List<IWhenDoCommand>() { command };

            var joblogger = MockHelper.CreateLogger<WhenDoJobExecutor>();
            var handlerlogger = MockHelper.CreateLogger<LoggingCommandHandler>();
            var registryMock = new Mock<IWhenDoRegistry>();
            registryMock.Setup(x => x.GetCommandHandler(It.IsAny<string>())).Returns(new LoggingCommandHandler(handlerlogger));
            var hangfireMock = new Mock<IBackgroundJobClient>();

            var dtpMock = MockHelper.CreateDateTimeProviderMock();
            dtpMock.Setup(x => x.CurrentTime).Returns(new TimeSpan(11, 55, 0));
            var repository = new MemoryJobRepository();
            var manager = new WhenDoJobManager(dtpMock.Object, registryMock.Object, MockHelper.CreateLogger<WhenDoJobManager>(), repository, hangfireMock.Object);
            manager.ExecuteJob(job, new TestMessage());

            Assert.AreEqual(0, handlerlogger.Count);
            hangfireMock.Verify(x => x.Create(It.IsAny<Job>(), It.IsAny<ScheduledState>()));
        }

        [TestMethod]
        public void AllCommandsInJobShouldBeRun()
        {
            var job = CreateJob();
            var joblogger = MockHelper.CreateLogger<WhenDoJobExecutor>();
            var hangfireMock = new Mock<IBackgroundJobClient>();

            var handlerMock = new Mock<ILoggingCommandHandler>();
            handlerMock.Setup(x => x.LogErrorAsync(It.IsAny<string>()));
            handlerMock.Setup(x => x.LogWarningAsync(It.IsAny<string>()));

            var commandExecutorMock = new Mock<IWhenDoCommandExecutor>();
            commandExecutorMock.Setup(x => x.ExecuteAsync(It.IsAny<IWhenDoMessage>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var dtpMock = MockHelper.CreateDateTimeProviderMock();
            dtpMock.Setup(x => x.CurrentTime).Returns(new TimeSpan(11, 55, 0));
            var registry = new Mock<IWhenDoRegistry>();
            var hangfire = new Mock<IBackgroundJobClient>();
            var repository = new MemoryJobRepository();
            var manager = new WhenDoJobManager(dtpMock.Object, registry.Object, MockHelper.CreateLogger<WhenDoJobManager>(), repository, hangfire.Object);
            
            manager.ExecuteJob(job, new TestMessage());

            commandExecutorMock.Verify(x => x.ExecuteAsync(It.IsAny<IWhenDoMessage>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        } 
        
        [TestMethod]
        public void ExecutionShouldContinueAfterInvalidCommands()
        {
            var job = CreateJobWithInvalidCommands();
            var joblogger = MockHelper.CreateLogger<WhenDoJobExecutor>();
            var hangfireMock = new Mock<IBackgroundJobClient>();

            var commandExecutorMock = new Mock<IWhenDoCommandExecutor>();
            commandExecutorMock.Setup(x => x.ExecuteAsync(It.IsAny<IWhenDoMessage>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            var dtpMock = MockHelper.CreateDateTimeProviderMock();
            dtpMock.Setup(x => x.CurrentTime).Returns(new TimeSpan(11, 55, 0));
            var registry = new Mock<IWhenDoRegistry>();
            var hangfire = new Mock<IBackgroundJobClient>();
            var repository = new MemoryJobRepository();
            var manager = new WhenDoJobManager(dtpMock.Object, registry.Object, MockHelper.CreateLogger<WhenDoJobManager>(), repository, hangfire.Object);

            manager.ExecuteJob(job, new TestMessage());

            commandExecutorMock.Verify(x => x.ExecuteAsync(It.IsAny<IWhenDoMessage>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(4));
        }

        private IWhenDoJob CreateJob()
        {
            var job = new WhenDoJob();
            job.Condition = WhenDoHelpers.ParseExpression("true", new Dictionary<string, Type>() { { "Test", typeof(TestMessage) } });

            var command1 = new WhenDoCommand()
            {
                Type = "Logging",
                MethodName = "LogError",
                Parameters = new Dictionary<string, object>() { { "text", "unit test error command 1" } }
            };

            var command2 = new WhenDoCommand()
            {
                Type = "Logging",
                MethodName = "LogWarning",
                Parameters = new Dictionary<string, object>() { { "text", "unit test warning command 2" } }
            };

            job.Commands = new List<IWhenDoCommand>() { command1, command2 };

            return job;
        }

        private IWhenDoJob CreateJobWithInvalidCommands()
        {
            var job = new WhenDoJob();
            job.Condition = WhenDoHelpers.ParseExpression("true", new Dictionary<string, Type>() { { "Test", typeof(TestMessage) } });

            var command1 = new WhenDoCommand()
            {
                Type = "BlaBla", //handler doesn't exist
                MethodName = "LogError", 
                Parameters = new Dictionary<string, object>() { { "text", "unit test command 1" } }
            };

            var command2 = new WhenDoCommand()
            {
                Type = "Logging",
                MethodName = "LogErrors", //method doesn't exist
                Parameters = new Dictionary<string, object>() { { "text", "unit test command 1" } }
            };

            var command3 = new WhenDoCommand()
            {
                Type = "Logging",
                MethodName = "LogError", 
                Parameters = new Dictionary<string, object>() { { "blabla", "unit test command 1" } } //parameter doesn't exist
            };

            var command4 = new WhenDoCommand()
            {
                Type = "Logging",
                MethodName = "LogError",
                Parameters = new Dictionary<string, object>() { { "text", "This should be normally logged" } } //parameter doesn't exist
            };

            job.Commands = new List<IWhenDoCommand>() { command1, command2, command3, command4};

            return job;
        }
    }
}