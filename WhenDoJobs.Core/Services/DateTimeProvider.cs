using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public TimeSpan CurrentTime => DateTimeOffset.Now.TimeOfDay;

        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}
