using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Providers
{
    public class DateTimeProvider : IDateTimeProvider, IWhenDoConditionProvider
    {
        public TimeSpan CurrentTime => DateTimeOffset.Now.TimeOfDay;

        public DateTimeOffset Now => DateTimeOffset.Now;

        public string Id => "DateTime";
    }
}
