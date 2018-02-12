using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Models
{
    public static class Extensions
    {
        public static bool IsValid(this JobDefinition definition)
        {
            //TODO: check if commands exists, condition is parsable, etc.
            return true;
        }
    }
}
