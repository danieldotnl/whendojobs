using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core
{
    public static class WhenDoExtensions
    {
        public static IJob ToJob(this JobDefinition definition)
        {
            var mi = typeof(WhenDoExtensions).GetMethod("ToJob", true);
            var type = Type.GetType($"WhenDoJobsApp.Messages.{definition.Context}, WhenDoJobsApp"); //TODO: make generic
            var method = mi.MakeGenericMethod(type);
            return (IJob) method.Invoke(typeof(WhenDoExtensions), new object[] { definition });
        }

        public static IJob ToJob<TContext>(this JobDefinition definition) where TContext : IMessageContext
        {
            var job = new Job<TContext>()
            {
                Id = definition.Id,
                Version = definition.Version,
                Disabled = definition.Disabled,
                DisabledFrom = definition.DisabledFrom,
                DisabledTill = definition.DisabledTill,

                Condition = WhenDoHelpers.ParseExpression(definition.When, definition.Context, typeof(TContext)),
                Commands = definition.Do.Select(x => x.ToCommand())
            };
            return job;
        }

        public static MethodInfo GetMethod(this Type type, string name, bool generic)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return type.GetMethods()
                .FirstOrDefault(method => method.Name == name & method.IsGenericMethod == generic);
        }

        public static ICommand ToCommand(this CommandDefinition definition)
        {
            var command = new Command()
            {
                Type = definition.Type,
                MethodName = definition.Command,
                Parameters = definition.Parameters,
                ExecutionStrategy = definition.Execution.ToExecutionStrategy()
            };
            return command;            
        }

        public static ExecutionStrategy ToExecutionStrategy(this ExecutionStrategyDefinition definition)
        {
            return new ExecutionStrategy()
            {
                Mode = definition.Mode,
                Time = definition.Time
            };
        }
    }
}
