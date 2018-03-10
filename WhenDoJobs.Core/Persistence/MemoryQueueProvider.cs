using System;
using System.Collections.Concurrent;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Persistence
{
    public class MemoryQueueProvider : IMemoryQueueProvider
    {
        private ConcurrentQueue<IWhenDoMessage> queue = new ConcurrentQueue<IWhenDoMessage>();

        public void EnqueueMessage(IWhenDoMessage message)
        {
            queue.Enqueue(message);
        }

        public bool GetMessage(out IWhenDoMessage message)
        {
            var success = queue.TryDequeue(out IWhenDoMessage result);
            message = result;
            return success;
        }
    }
}
