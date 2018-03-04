﻿using System;
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
        public static IWhenDoJob ToJob(this JobDefinition definition, Dictionary<string, Type> providers)
        { 
            var job = new WhenDoJob()
            {
                Id = definition.Id,
                Version = definition.Version,
                Disabled = definition.Disabled,
                DisabledFrom = definition.DisabledFrom,
                DisabledTill = definition.DisabledTill,
                ConditionProviders = GetProviderNames(definition.Providers),

                Condition = WhenDoHelpers.ParseExpression(definition.When, providers),
                Commands = definition.Do.Select(x => x.ToCommand())
            };
            return job;
        }

        private static List<string> GetProviderNames(List<string> providerDefs)
        {
            var names = new List<string>();
            foreach (var prov in providerDefs)
            {
                if (prov.Contains('='))
                    names.Add(prov.Split('=')[1]);
                else
                    names.Add(prov);
            }
            return names;
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
