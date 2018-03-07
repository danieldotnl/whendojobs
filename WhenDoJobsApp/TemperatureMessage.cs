using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobsApp
{
    public class TemperatureMessage : IWhenDoMessage
    {
        public double Temperature { get; set; }
        public string Area { get; set; }
    }
}
