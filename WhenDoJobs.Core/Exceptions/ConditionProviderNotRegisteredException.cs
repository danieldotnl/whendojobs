using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Exceptions
{
    public class ConditionProviderNotRegisteredException : ArgumentException
    {
        public ConditionProviderNotRegisteredException(string message, string paramName) : base(message, paramName)
        {

        }
    }
}
