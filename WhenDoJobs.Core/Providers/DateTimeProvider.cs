using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Providers
{
    public class DateTimeProvider : IDateTimeProvider, IWhenDoExpressionProvider
    {
        public virtual TimeSpan CurrentTime => DateTimeOffset.Now.TimeOfDay;

        public virtual DateTimeOffset Now => DateTimeOffset.Now;

        public virtual string Id => "DateTime";
    }
}
