using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Common.Linq
{
    public class ExpressionParameterReplacer : ExpressionVisitor
    {
        public ExpressionParameterReplacer(IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters)
        {
            ParameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();
            for (int i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
                ParameterReplacements.Add(fromParameters[i], toParameters[i]);
        }

        private IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements { get; set; }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            ParameterExpression replacement;
            if (ParameterReplacements.TryGetValue(node, out replacement))
                node = replacement;
            return base.VisitParameter(node);
        }

        public static Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2)
        {
            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    exp1.Body, new ExpressionParameterReplacer(exp2.Parameters, exp1.Parameters).Visit(exp2.Body)),
                exp1.Parameters);
        }
    }
}
