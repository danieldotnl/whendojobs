using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Exceptions
{
    public class ExpressionProviderNotRegisteredException : ArgumentException
    {
        public ExpressionProviderNotRegisteredException(string message, string paramName) : base(message, paramName)
        {

        }
    }
}
