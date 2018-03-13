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
        public static Delegate ParseExpression<T>(string condition, List<ExpressionProviderInfo> availableProviders)
        {
            if (typeof(T) == typeof(bool) && string.IsNullOrEmpty(condition))
                condition = "true";

            else if (typeof(T) == typeof(TimeSpan) && TimeSpan.TryParse(condition, out TimeSpan result))
                condition = $"TimeSpan.Parse(\"{condition}\")";

            var parameters = new List<ParameterExpression>();
            var requiredProviderShortNames = condition.ExtractProviderShortNames();

            if(requiredProviderShortNames != null)
            {
                foreach (var requiredProvider in requiredProviderShortNames)
                {
                    parameters.Add(Expression.Parameter(availableProviders.Single(x => x.ShortName == requiredProvider).ProviderType, requiredProvider));
                }
            }

            var e = DynamicExpressionParser.ParseLambda(parameters.ToArray(), typeof(T), condition.Replace("@", String.Empty));
            return e.Compile();
        }
    }
}
