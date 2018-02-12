using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core
{
    public static class WhenDoExtensions
    {
        public static IJob ToJob<T>(this JobDefinition definition, IJob job)
        {
            job.Id = definition.Id;
            job.Version = definition.Version;
            job.Disabled = definition.Disabled;
            job.DisabledFrom = definition.DisabledFrom;
            job.DisabledTill = definition.DisabledTill;

            var expression = WhenDoHelpers.ParseExpression(definition.When, definition.Context, typeof(T));

            job.Condition = expression;
            job.CommandDefinitions = definition.Do;

            return job;
        }
    }
}
