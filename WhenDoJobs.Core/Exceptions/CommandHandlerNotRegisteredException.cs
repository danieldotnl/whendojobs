using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Exceptions
{
    public class CommandHandlerNotRegisteredException : ArgumentException
    {
        public CommandHandlerNotRegisteredException(string message, string paramName) : base(message, paramName)
        {

        }
    }
}
