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
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); }
            if (key == null) { throw new ArgumentNullException(nameof(key)); }

            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
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

        public static IWhenDoCommand ToCommand(this CommandDefinition definition)
        {
            var command = new WhenDoCommand()
            {
                Id = Guid.NewGuid().ToString("N").ToLower(),
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
