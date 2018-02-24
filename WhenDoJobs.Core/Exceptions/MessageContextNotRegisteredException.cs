using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Exceptions
{
    public class MessageContextNotRegisteredException : ArgumentException
    {
        public MessageContextNotRegisteredException(string message, string paramName) : base(message, paramName)
        {

        }
    }
}
