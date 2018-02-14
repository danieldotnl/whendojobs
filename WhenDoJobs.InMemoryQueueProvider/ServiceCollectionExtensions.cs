using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core;

namespace WhenDoJobs.InMemoryQueueProvider
{
    public static class ServiceCollectionExtensions
    {
        public static WhenDoConfiguration UseInMemoryQueue(this WhenDoConfiguration options)
        {
            options.UseQueue(sp => new InMemoryQueueProvider());
            return options;
        }
    }
}
