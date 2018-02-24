using System;
using System.Collections.Concurrent;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Services
{
    public class InMemoryQueueProvider : IInMemoryQueueProvider
    {
        private ConcurrentQueue<IWhenDoMessageContext> queue = new ConcurrentQueue<IWhenDoMessageContext>();

        public void EnqueueMessage(IWhenDoMessageContext message)
        {
            queue.Enqueue(message);
        }

        public bool GetMessage(out IWhenDoMessageContext message)
        {
            var success = queue.TryDequeue(out IWhenDoMessageContext result);
            message = result;
            return success;
        }
    }
}
