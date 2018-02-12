using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core;

namespace WhenDoJobs.InMemoryQueueProvider
{
    public static class ServiceCollectionExtensions
    {
        public static WhenDoOptions UseInMemoryQueue(this WhenDoOptions options)
        {
            options.UseQueue(sp => new InMemoryQueueProvider());
            return options;
        }
    }
}
