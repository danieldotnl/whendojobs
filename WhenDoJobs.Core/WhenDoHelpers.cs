using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core
{
    public static class WhenDoHelpers
    {
        public static Delegate ParseExpression(string condition, Dictionary<string, Type> providers)
        {
            if (string.IsNullOrEmpty(condition))
                condition = "true";

            var parameters = new List<ParameterExpression>();

            if (providers != null)
            {
                foreach (var provider in providers)
                {
                    parameters.Add(Expression.Parameter(provider.Value, provider.Key));
                }
            }
            var e = DynamicExpressionParser.ParseLambda(parameters.ToArray(), typeof(bool), condition);
            return e.Compile();
        }
    }
}
