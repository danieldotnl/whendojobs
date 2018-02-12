using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;

namespace WhenDoJobs.Core
{
    public static class WhenDoHelpers
    {
        public static Delegate ParseExpression(string condition, string contextString, Type context)
        {
            var param = Expression.Parameter(context, contextString);
            var e = DynamicExpressionParser.ParseLambda(new[] { param }, typeof(bool), condition);
            return e.Compile();
        }
    }
}
