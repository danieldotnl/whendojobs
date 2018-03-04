using Hangfire;
using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Services
{
    public class WhenDoJobScheduler
    {
        private List<IWhenDoJob> Jobs = new List<IWhenDoJob>();
        IBackgroundJobClient jobClient;

        public WhenDoJobScheduler(IBackgroundJobClient jobClient)
        {
            this.jobClient = jobClient;
        }

        public void Schedule(DateTimeOffset dateTime, IWhenDoJob job)
        {
            //jobClient.Schedule<IWhenDoJobExecutor>(je => je.ExecuteAsync(job), dateTime);
        }

        public void Schedule(DateTimeOffset dateTime, IWhenDoJob job, IWhenDoMessage message)
        {
            jobClient.Schedule<IWhenDoJobExecutor>(je => je.ExecuteAsync(job, message), dateTime);
        }

        public void Delay(TimeSpan time, IWhenDoJob job)
        {
            this.Schedule(DateTimeOffset.Now.Add(time), job);
        }

        public void ScheduleNow(IWhenDoJob job)
        {
            this.Schedule(DateTimeOffset.Now, job);
        }


    }
}
