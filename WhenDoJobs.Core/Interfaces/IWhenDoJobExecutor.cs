﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoJobExecutor
    {
        Task ExecuteAsync(IWhenDoJob job, IWhenDoMessageContext context);
    }
}
