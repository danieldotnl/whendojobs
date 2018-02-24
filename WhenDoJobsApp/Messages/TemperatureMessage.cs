using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobsApp.Messages
{
    public class TemperatureMessage : IWhenDoMessageContext
    {
        public double Temperature { get; set; }
        public string Area { get; set; }
    }
}
