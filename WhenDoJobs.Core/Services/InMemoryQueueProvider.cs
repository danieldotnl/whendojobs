using System;
using System.Collections.Concurrent;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Services
{
    public class InMemoryQueueProvider : IInMemoryQueueProvider
    {
        private ConcurrentQueue<IMessageContext> queue = new ConcurrentQueue<IMessageContext>();

        public void EnqueueMessage(IMessageContext message)
        {
            queue.Enqueue(message);
        }

        public bool GetMessage(out IMessageContext message)
        {
            var success = queue.TryDequeue(out IMessageContext result);
            message = result;
            return success;
        }
    }
}
