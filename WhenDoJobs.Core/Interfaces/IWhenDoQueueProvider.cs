using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoQueueProvider
    {
        void EnqueueMessage(IWhenDoMessage message);
        bool GetMessage(out IWhenDoMessage message);
    }
}
