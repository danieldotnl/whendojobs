using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoQueueProvider
    {
        void EnqueueMessage(IWhenDoMessageContext message);
        bool GetMessage(out IWhenDoMessageContext message);
    }
}
