using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobsApp.Messages
{
    public class TemperatureMessage : IMessageContext
    {
        public double Temperature { get; set; }
    }
}
