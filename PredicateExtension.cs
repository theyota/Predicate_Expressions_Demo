using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqKit;

namespace Project.Extension
{
    public class PredicateExtension<TModel> where TModel : class
    {

        public ExpressionStarter<TModel> WhereExpression<TVal>(TVal val)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TModel), "m");
            
            ConstantExpression valExpression = Expression.Constant(val, typeof(TVal));
            
            ConstantExpression body = Expression.Constant(false, typeof(bool));

            IEnumerable<PropertyInfo> properties = typeof(TModel).GetProperties()
                             .Where(p => p.PropertyType == typeof(TVal));

            ExpressionStarter<TModel> predicate = PredicateBuilder.New<TModel>(false);

            foreach (var property in properties)
            {
                var propertyExpression = Expression.Property(parameter, property);

                var equalExpression = Expression.Equal(propertyExpression, valExpression);

                var lamdaPredicate = Expression.Lambda<Func<TModel, bool>>(equalExpression, new[] { parameter});

                predicate = predicate.Or(lamdaPredicate);
            }

            return predicate;

        }

        public ExpressionStarter<TModel> ContainsExpression<TVal>(TVal val)
        {
            ExpressionStarter<TModel> predicate = PredicateBuilder.New<TModel>(false);

            ParameterExpression parameter = Expression.Parameter(typeof(TModel), "m");

            ConstantExpression searchExpression = Expression.Constant(val);

            IEnumerable<PropertyInfo> properties = typeof(TModel).GetProperties()
                 .Where(p => p.PropertyType == typeof(TVal));

            foreach (var property in properties)
            {
                var propertyExpression = Expression.Property(parameter, property);

                // m.Model.Contains(SearchExpression)
                var containsExpression = Expression.Call(propertyExpression, "contains", null, searchExpression);

                var lamdaPredicate = Expression.Lambda<Func<TModel, bool>>(containsExpression, new[] { parameter });

                predicate = predicate.Or(lamdaPredicate);
            }

            return predicate;
        }




    }
}