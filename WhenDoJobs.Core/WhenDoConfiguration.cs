using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core
{
    public class WhenDoConfiguration
    {
        internal Func<IServiceProvider, IQueueProvider> QueueFactory;

        public void UseQueue(Func<IServiceProvider, IQueueProvider> factory)
        {
            QueueFactory = factory;
        }
    }
}
