using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoRepository<T> where T:IWhenDoJob
    {
        Task SaveAsync(T job);
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task RemoveAllAsync();
    }
}
