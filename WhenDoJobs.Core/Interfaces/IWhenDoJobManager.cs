using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoJobManager
    {
        Task Handle(IWhenDoMessage message);
    }
}
