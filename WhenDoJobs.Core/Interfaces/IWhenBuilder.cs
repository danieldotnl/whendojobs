using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenBuilder<TMessage> where TMessage: IMessageContext
    {
        IWhenBuilder<TMessage> When(Func<TMessage, bool> condition);
        Func<TMessage, bool> Build();
    }
}
