using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Persistence
{
    public class MemoryJobRepository : IWhenDoRepository<IWhenDoJob>
    {
        private ConcurrentDictionary<string, IWhenDoJob> collection = new ConcurrentDictionary<string, IWhenDoJob>();

        public Task<IEnumerable<IWhenDoJob>> GetAllAsync()
        {
            var list = collection.Values.ToList();
            return Task.FromResult((IEnumerable<IWhenDoJob>) list);
        }

        public Task RemoveAllAsync()
        {
            collection.Clear();
            return Task.CompletedTask;
        }

        public Task<IWhenDoJob> GetByIdAsync(string id)
        {
            return Task.FromResult(collection.GetValueOrDefault(id));
        }

        public Task SaveAsync(IWhenDoJob job)
        {
            collection.AddOrUpdate(job.Id, job, (key, value) => {return job; });
            return Task.CompletedTask;
        }

        public Task<IEnumerable<IWhenDoJob>> GetAsync(Expression<Func<IWhenDoJob, bool>> predicate)
        {
            var result = collection.Values.Where(predicate.Compile());
            return Task.FromResult(result);
        }
    }
}
