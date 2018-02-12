using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Interfaces
{
    public interface ICommandBuilder
    {
        ICommandBuilder LogWithLevel(LogLevel level, string text);
    }
}
