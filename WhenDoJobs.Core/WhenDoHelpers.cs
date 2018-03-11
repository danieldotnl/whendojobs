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
        public static Delegate ParseExpression<T>(string condition, Dictionary<string, Type> availableProviders)
        {
            if (string.IsNullOrEmpty(condition))
                condition = "true";

            var parameters = new List<ParameterExpression>();
            var requiredProviders = condition.ExtractProviders();

            if(requiredProviders != null)
            {
                foreach (var requiredProvider in requiredProviders)
                {
                    parameters.Add(Expression.Parameter(availableProviders[requiredProvider], requiredProvider));
                }
            }

            var e = DynamicExpressionParser.ParseLambda(parameters.ToArray(), typeof(T), condition.Replace("@", String.Empty));
            return e.Compile();
        }
    }
}
