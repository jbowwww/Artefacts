using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Artefacts
{
    class QueryTranslator<T> : ExpressionVisitor
    {
        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            return base.VisitMemberBinding(node);
        }
        
        protected override Expression VisitParameter(ParameterExpression node)
        {

            return base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression.NodeType == ExpressionType.Parameter)
            {
                ParameterExpression p = (ParameterExpression)node.Expression;
                if (p.Type == typeof(T))
                {
                    MemberInfo member = Artefact._T.GetMember(node.Member.Name, node.Member.MemberType, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance).FirstOrDefault();
                    if (member == null)
                        throw new MissingMemberException(Artefact._T.FullName, node.Member.Name);
                    return Expression.MakeMemberAccess(
                        Expression.Parameter(Artefact._T, p.Name),
                        member);
                }
            }
            return base.VisitMember(node);
        }
        
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (node.Parameters.Any(p => p.Type == Artefact._T))
            {
                return Expression.Lambda(Visit(node.Body), VisitParameterArray(node.Parameters));
            }
            return base.VisitLambda<T>(node);
        }

        private ParameterExpression[] VisitParameterArray(ReadOnlyCollection<ParameterExpression> parameters)
        {
            List<ParameterExpression> newParameters = new List<ParameterExpression>();
            foreach (ParameterExpression p in parameters)
            {
                newParameters.Add((ParameterExpression)Visit(p));
            }
            return newParameters.ToArray();
        }
    }
}
