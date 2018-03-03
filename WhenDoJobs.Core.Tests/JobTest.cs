using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;
using WhenDoJobs.Core.Tests.Helpers;

namespace WhenDoJobs.Core.Tests
{
    [TestClass]
    public class JobTest
    {
        [TestMethod]
        public void IsNotRunnableWhenDisabled()
        {
            var message = new Mock<IWhenDoMessage>().Object;
            var dtp = MockHelper.CreateDateTimeProviderMock().Object;

            var job = new WhenDoJob();

            job.Disabled = true;
            Assert.IsFalse(job.IsRunnable(dtp));

            job.Disabled = false;
            Assert.IsTrue(job.IsRunnable(dtp));
        }

        [TestMethod]
        public void IsNotRunnableWithinDisabledTimeFrame()
        {
            var message = new Mock<IWhenDoMessage>().Object;
            var dtpMock = MockHelper.CreateDateTimeProviderMock();
            dtpMock.Setup(x => x.CurrentTime).Returns(new TimeSpan(11, 55, 0));
            var dtp = dtpMock.Object;

            var job = new WhenDoJob();
            job.DisabledFrom = new TimeSpan(10, 0, 0);
            job.DisabledTill = new TimeSpan(14, 0, 0);

            Assert.IsFalse(job.IsRunnable(dtp));
            dtpMock.Verify(x => x.CurrentTime, Times.Once);
        }

        [TestMethod]
        public void IsRunnableOutsideDisabledTimeFrame()
        {
            var message = new Mock<IWhenDoMessage>().Object;
            var dtpMock = MockHelper.CreateDateTimeProviderMock();
            dtpMock.Setup(x => x.CurrentTime).Returns(new TimeSpan(15, 55, 0));
            var dtp = dtpMock.Object;

            var job = new WhenDoJob();
            job.DisabledFrom = new TimeSpan(10, 0, 0);
            job.DisabledTill = new TimeSpan(14, 0, 0);

            Assert.IsTrue(job.IsRunnable(dtp));
            dtpMock.Verify(x => x.CurrentTime, Times.Once);
        }

        [TestMethod]
        public void TestConditionEvaluation()
        {
            var condition = "TestMessage.DoubleValue > 15.3 AND TestMessage.StringValue = \"Livingroom\"";
            
            var job = new WhenDoJob();
            job.Condition = WhenDoHelpers.ParseExpression(condition, "TestMessage", typeof(TestMessage));

            var message = new TestMessage() { DoubleValue = 15.0D, IntValue = 20, StringValue = "Livingroom" };
            Assert.IsFalse(job.Evaluate(message));

            message = new TestMessage() { DoubleValue = 15.50D, IntValue = 20, StringValue = "Livingroom" };
            Assert.IsTrue(job.Evaluate(message));

            message = new TestMessage() { DoubleValue = 15.50D, IntValue = 20, StringValue = "Living room" };
            Assert.IsFalse(job.Evaluate(message));
        }
    }
}
