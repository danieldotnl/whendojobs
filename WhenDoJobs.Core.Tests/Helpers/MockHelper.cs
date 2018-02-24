using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Services;

namespace WhenDoJobs.Core.Tests.Helpers
{
    public static class MockHelper
    {
        public static Mock<IDateTimeProvider> CreateDateTimeProviderMock()
        {
            var dtpMock = new Mock<IDateTimeProvider>();
            dtpMock.Setup(x => x.CurrentTime).Returns(DateTimeOffset.Now.TimeOfDay);
            return dtpMock;
        }

        public static Mock<IServiceProvider> CreateServiceProviderMock(IWhenDoCommandExecutor executor)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(IWhenDoCommandExecutor)))
                .Returns(executor);

            //var serviceScope = new Mock<IServiceScope>();
            //serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

            //var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            //serviceScopeFactory
            //    .Setup(x => x.CreateScope())
            //    .Returns(serviceScope.Object);

            //serviceProvider
            //    .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            //    .Returns(serviceScopeFactory.Object);

            return serviceProvider;
        }

        public static Mock<IWhenDoRegistry> CreateRegistryMock(ILoggingCommandHandler handler)
        {
            var mock = new Mock<IWhenDoRegistry>();
            mock.Setup(x => x.GetCommandHandler(It.Is<string>(y => y == "Logging")))
                .Returns(handler);

            return mock;
        }

        public static Logger<T> CreateLogger<T>()
        {
            return new Logger<T>();
        }        
    }    
}
