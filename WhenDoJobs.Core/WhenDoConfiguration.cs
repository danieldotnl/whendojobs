using Hangfire;
using Hangfire.MemoryStorage;
using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace WhenDoJobs.Core
{
    public class WhenDoConfiguration
    {
        internal Func<IServiceProvider, IWhenDoQueueProvider> QueueFactory;
        internal Func<IServiceProvider, JobStorage> HangfireStorageFactory;
        internal bool ExternalHangfireServer = false;

        private IWhenDoQueueProvider queueProvider;

        public WhenDoConfiguration()
        {
            QueueFactory = new Func<IServiceProvider, IWhenDoQueueProvider>(sp =>
            {
                if (queueProvider == null)
                    queueProvider = new InMemoryQueueProvider();
                return queueProvider;
            });
            HangfireStorageFactory = new Func<IServiceProvider, JobStorage>(sp => new MemoryStorage());
        }

        public void UseHangfireStorage(Func<IServiceProvider, JobStorage> factory)
        {
            HangfireStorageFactory = factory;
        }

        public void UseExternalHangfireServer()
        {
            ExternalHangfireServer = true;
        }

        public void UseQueue(Func<IServiceProvider, IWhenDoQueueProvider> factory)
        {
            QueueFactory = factory;
        }


    }
}
