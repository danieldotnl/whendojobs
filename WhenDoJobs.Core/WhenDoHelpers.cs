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
        public static Delegate ParseExpression(string condition, string contextString, Type context)
        {
            var param = Expression.Parameter(context, contextString);
            var e = DynamicExpressionParser.ParseLambda(new[] { param }, typeof(bool), condition);
            return e.Compile();
        }
    }
}
