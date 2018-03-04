using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Exceptions;
using System.Text;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;
using WhenDoJobs.Core.Providers;
using WhenDoJobs.Core.Services;
using WhenDoJobs.Core.Tests.Helpers;

namespace WhenDoJobs.Core.Tests
{
    [TestClass]
    public class JobTest
    {
        [TestMethod]
        public void IsNotRunnableWhenDisabled()
        {
            var dtpMock = MockHelper.CreateDateTimeProviderMock();
            dtpMock.Setup(x => x.CurrentTime).Returns(new TimeSpan(11, 55, 0));

            var jobExecutor = new Mock<IWhenDoJobExecutor>();
            var registry = new Mock<IWhenDoRegistry>();
            var manager = new WhenDoJobManager(dtpMock.Object, registry.Object, MockHelper.CreateLogger<WhenDoJobManager>(), jobExecutor.Object);

            var job = new WhenDoJob();
            job.Condition = WhenDoHelpers.ParseExpression("true", null);

            job.Disabled = true;
            Assert.IsFalse(manager.IsRunnable(job, null));

            job.Disabled = false;
            Assert.IsTrue(manager.IsRunnable(job, null));
        }

        [TestMethod]
        public void IsNotRunnableWithinDisabledTimeFrame()
        {
            var dtpMock = MockHelper.CreateDateTimeProviderMock();
            dtpMock.Setup(x => x.CurrentTime).Returns(new TimeSpan(11, 55, 0));

            var jobExecutor = new Mock<IWhenDoJobExecutor>();
            var registry = new Mock<IWhenDoRegistry>();
            var manager = new WhenDoJobManager(dtpMock.Object, registry.Object, MockHelper.CreateLogger<WhenDoJobManager>(), jobExecutor.Object);

            var job = new WhenDoJob();
            job.Condition = WhenDoHelpers.ParseExpression("true", null);
            job.DisabledFrom = new TimeSpan(10, 0, 0);
            job.DisabledTill = new TimeSpan(14, 0, 0);

            Assert.IsFalse(manager.IsRunnable(job, null));
            dtpMock.Verify(x => x.CurrentTime, Times.Once);
        }

        [TestMethod]
        public void IsRunnableOutsideDisabledTimeFrame()
        {
            var dtpMock = MockHelper.CreateDateTimeProviderMock();
            dtpMock.Setup(x => x.CurrentTime).Returns(new TimeSpan(15, 55, 0));

            var jobExecutor = new Mock<IWhenDoJobExecutor>();
            var registry = new Mock<IWhenDoRegistry>();
            var manager = new WhenDoJobManager(dtpMock.Object, registry.Object, MockHelper.CreateLogger<WhenDoJobManager>(), jobExecutor.Object);

            var job = new WhenDoJob();
            job.Condition = WhenDoHelpers.ParseExpression("true", null);
            job.DisabledFrom = new TimeSpan(10, 0, 0);
            job.DisabledTill = new TimeSpan(14, 0, 0);

            Assert.IsTrue(manager.IsRunnable(job, null));
            dtpMock.Verify(x => x.CurrentTime, Times.Once);
        }

        [TestMethod]
        public void ParseMethodWithoutProvider()
        {
            var condition = "TestMessage.DoubleValue > 15.3 AND TestMessage.StringValue = \"Livingroom\" AND dtp.CurrentTime > \"10:00\"";

            Assert.ThrowsException<ParseException>(() => WhenDoHelpers.ParseExpression(condition, new Dictionary<string, Type>() { { "TestMessage", typeof(TestMessage) } }));
        }

        [TestMethod]
        public void TestConditionEvaluation()
        {
            var dtpMock = new Mock<IDateTimeProvider>();
            dtpMock.Setup(x => x.CurrentTime).Returns(new TimeSpan(15, 55, 0));

            var jobExecutor = new Mock<IWhenDoJobExecutor>();
            var registry = new Mock<IWhenDoRegistry>();
            var manager = new WhenDoJobManager(dtpMock.Object, registry.Object, MockHelper.CreateLogger<WhenDoJobManager>(), jobExecutor.Object);

            var condition = "TestMessage.DoubleValue > 15.3 AND TestMessage.StringValue = \"Livingroom\"";
            
            var job = new WhenDoJob();
            job.ConditionProviders = new List<string>() { "TestMessage" };
            job.Condition = WhenDoHelpers.ParseExpression(condition, new Dictionary<string, Type>() {
                { "TestMessage", typeof(TestMessage) }
            });

            var message = new TestMessage() { DoubleValue = 15.0D, IntValue = 20, StringValue = "Livingroom" };
            Assert.IsFalse(manager.IsRunnable(job, message));

            message = new TestMessage() { DoubleValue = 15.50D, IntValue = 20, StringValue = "Livingroom" };
            Assert.IsTrue(manager.IsRunnable(job, message));
            
            message = new TestMessage() { DoubleValue = 15.50D, IntValue = 20, StringValue = "Living room" };
            Assert.IsFalse(manager.IsRunnable(job, message));
        }
    }
}
