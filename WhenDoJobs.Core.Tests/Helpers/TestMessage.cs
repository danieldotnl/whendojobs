using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Tests.Helpers
{
    public class TestMessage : IWhenDoMessage
    {
        public double DoubleValue { get; set; }
        public string StringValue { get; set; }
        public int IntValue { get; set; }
    }
}
