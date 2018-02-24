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
        internal Func<IServiceProvider, IWhenDoQueueProvider> QueueFactory;
        internal Func<IServiceProvider, JobStorage> HangfireStorageFactory;
        internal bool ExternalHangfireServer = false;


        public WhenDoConfiguration()
        {
            QueueFactory = new Func<IServiceProvider, IWhenDoQueueProvider>(sp => new InMemoryQueueProvider());
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
