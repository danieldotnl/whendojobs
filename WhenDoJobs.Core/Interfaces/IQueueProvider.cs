using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IQueueProvider
    {
        void EnqueueMessage(IMessageContext message);
        bool GetMessage(out IMessageContext message);
    }
}
