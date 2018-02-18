using Hangfire;
using Hangfire.MemoryStorage;
using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Services;

namespace WhenDoJobs.Core
{
    public class WhenDoConfiguration
    {
        internal Func<IServiceProvider, IQueueProvider> QueueFactory;
        internal Func<IServiceProvider, JobStorage> HangfireStorageFactory;
        internal bool ExternalHangfireServer = false;


        public WhenDoConfiguration()
        {
            QueueFactory = new Func<IServiceProvider, IQueueProvider>(sp => new InMemoryQueueProvider());
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

        public void UseQueue(Func<IServiceProvider, IQueueProvider> factory)
        {
            QueueFactory = factory;
        }
    }
}
