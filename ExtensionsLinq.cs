using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Linq;

namespace System
{
    public static class ExtensionsLinq
    {

        public static IEnumerable<T> Sort<T>(this IEnumerable<T> source, string sortExpression)
        {
            if (string.IsNullOrWhiteSpace(sortExpression))
                return source;

            string[] sortParts = sortExpression.Split(' ');
            var param = Expression.Parameter(typeof(T), string.Empty);
            try
            {
                var property = Expression.Property(param, sortParts[0]);
                var sortLambda = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), param);

                if (sortParts.Length > 1 && sortParts[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    return source.AsQueryable<T>().OrderByDescending<T, object>(sortLambda);
                }
                return source.AsQueryable<T>().OrderBy<T, object>(sortLambda);
            }
            catch (ArgumentException)
            {
                return source;
            }
        }

        public static IQueryable<TResult> Transform<TResult>(this IQueryable source)
        {
            var resultType = typeof(TResult);
            var resultProperties = resultType.GetProperties().Where(p => p.CanWrite);

            ParameterExpression s = Expression.Parameter(source.ElementType, "s");

            var memberBindings =
                resultProperties.Select(p =>
                    Expression.Bind(typeof(TResult).GetMember(p.Name)[0], Expression.Property(s, p.Name))).OfType<MemberBinding>();

            Expression memberInit = Expression.MemberInit(
                Expression.New(typeof(TResult)),
                memberBindings
                );

            var memberInitLambda = Expression.Lambda(memberInit, s);

            var typeArgs = new[]
        {
            source.ElementType, 
            memberInit.Type
        };

            var mc = Expression.Call(typeof(Queryable), "Select", typeArgs, source.Expression, memberInitLambda);

            var query = source.Provider.CreateQuery<TResult>(mc);

            return query;
        }

        public static IEnumerable<TResult> Transform<TResult>(this IEnumerable source)
        {
            return source.AsQueryable().Transform<TResult>();
        }
    }
}
