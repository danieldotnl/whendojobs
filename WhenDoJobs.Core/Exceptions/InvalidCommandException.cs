using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Exceptions
{
    public class InvalidCommandException : Exception
    {
        public IEnumerable<string> CommandParameters { get; } 

        public InvalidCommandException(string message, IEnumerable<string> parameters, Exception inner) : base(message, inner)
        {
            this.CommandParameters = parameters;
        }
    }
}
