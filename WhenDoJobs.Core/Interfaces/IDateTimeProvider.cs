using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IDateTimeProvider
    {
        TimeSpan CurrentTime { get; }
        DateTimeOffset Now { get; }
    }
}
