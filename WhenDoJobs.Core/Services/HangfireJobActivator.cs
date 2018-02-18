using Hangfire;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Services
{
    public class HangfireJobActivator : JobActivator
    {
        private IServiceProvider provider;

        public HangfireJobActivator(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public override object ActivateJob(Type type)
        {
            return provider.GetService(type);
        }
    }
}
