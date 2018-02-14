using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Commands;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Tests
{
    public static class MockHelper
    {
        public static Mock<IDateTimeProvider> CreateDateTimeProviderMock()
        {
            var dtpMock = new Mock<IDateTimeProvider>();
            dtpMock.Setup(x => x.CurrentTime).Returns(DateTimeOffset.Now.TimeOfDay);
            return dtpMock;
        }

        public static Logger<T> CreateLogger<T>()
        {
            return new Logger<T>();
        }
    }    
}
