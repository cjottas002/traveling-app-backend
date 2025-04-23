using System.Linq.Expressions;
using System.Reflection;

namespace TravelingApp.Application.Extensions
{
    public static class QueryExtensions
    {
        public static IQueryable<TEntity> Page<TEntity, TKey>(this IQueryable<TEntity> query, int pageSize, int pageIndex, Expression<Func<TEntity, TKey>> orderBy, bool ascending)
            where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(orderBy);
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            if (pageIndex < 0) throw new ArgumentOutOfRangeException(nameof(pageIndex), "Page index cannot be negative.");

            var ordered = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            return ordered.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public static IQueryable<TEntity> Page<TEntity>(this IQueryable<TEntity> query, int pageSize, int pageIndex, string? orderBy, bool ascending)
            where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(query);
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            if (pageIndex < 0) throw new ArgumentOutOfRangeException(nameof(pageIndex), "Page index cannot be negative.");

            bool first = true;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                foreach (var field in orderBy.Split(',', ';').Select(f => f.Trim()).Where(f => f.Length > 0))
                {
                    if (first)
                    {
                        query = ascending ? query.OrderBy(field) : query.OrderByDescending(field);
                        first = false;
                    }
                    else
                    {
                        query = ascending ? query.ThenBy(field) : query.ThenByDescending(field);
                    }
                }
            } 


            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, string fieldName) where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(query);
            if (string.IsNullOrWhiteSpace(fieldName)) throw new ArgumentException("Field name must be provided.", nameof(fieldName));
            var resultExp = GenerateMethodCall(query, nameof(OrderBy), fieldName);
            return (IOrderedQueryable<TEntity>)query.Provider.CreateQuery(resultExp);
        }

        public static IOrderedQueryable<TEntity> ThenBy<TEntity>(this IQueryable<TEntity> query, string fieldName) where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(query);
            if (string.IsNullOrWhiteSpace(fieldName)) throw new ArgumentException("Field name must be provided.", nameof(fieldName));
            var resultExp = GenerateMethodCall(query, nameof(ThenBy), fieldName);
            return (IOrderedQueryable<TEntity>)query.Provider.CreateQuery(resultExp);
        }

        public static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> query, string fieldName) where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(query);
            if (string.IsNullOrWhiteSpace(fieldName)) throw new ArgumentException("Field name must be provided.", nameof(fieldName));
            var resultExp = GenerateMethodCall(query, nameof(OrderByDescending), fieldName);
            return (IOrderedQueryable<TEntity>)query.Provider.CreateQuery(resultExp);
        }

        public static IOrderedQueryable<TEntity> ThenByDescending<TEntity>(this IQueryable<TEntity> query, string fieldName) where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(query);
            if (string.IsNullOrWhiteSpace(fieldName)) throw new ArgumentException("Field name must be provided.", nameof(fieldName));
            var resultExp = GenerateMethodCall(query, nameof(ThenByDescending), fieldName);
            return (IOrderedQueryable<TEntity>)query.Provider.CreateQuery(resultExp);
        }

        public static IEnumerable<TEntity> Between<TEntity, TProperty>(this IEnumerable<TEntity> collection, Func<TEntity, TProperty> propertySelector, TProperty min, TProperty max)
            where TProperty : IComparable<TProperty>
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(propertySelector);

            return collection
                .Where(e => {
                    var val = propertySelector(e);
                    return val.CompareTo(min) >= 0 && val.CompareTo(max) <= 0;
                });
        }

        public static IQueryable<TEntity> Between<TEntity, TProperty>(this IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> propertySelector, TProperty min, TProperty max)
            where TProperty : IComparable<TProperty>
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(propertySelector);

            var parameter = propertySelector.Parameters[0];
            var body = Expression.AndAlso(
                Expression.GreaterThanOrEqual(propertySelector.Body, Expression.Constant(min, typeof(TProperty))),
                Expression.LessThanOrEqual(propertySelector.Body, Expression.Constant(max, typeof(TProperty)))
            );
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);
            return source.Where(lambda);
        }

        private static LambdaExpression GenerateSelector<TEntity>(string propertyName, out Type resultType) where TEntity : class
        {
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            Expression propertyAccess = parameter;
            PropertyInfo? propertyInfo = null;

            if (propertyInfo is null) throw new InvalidOperationException($"No se encontró la propiedad '{propertyName}' en el tipo {typeof(TEntity).Name}.");

            foreach (var member in propertyName.Split('.'))
            {
                propertyInfo = propertyAccess.Type.GetProperty(member, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) 
                    ?? throw new ArgumentException($"Property '{member}' not found on type '{propertyAccess.Type.FullName}'.");
                propertyAccess = Expression.MakeMemberAccess(propertyAccess, propertyInfo);
            }

            resultType = propertyInfo.PropertyType;
            return Expression.Lambda(propertyAccess, parameter);
        }

        private static MethodCallExpression GenerateMethodCall<TEntity>(IQueryable<TEntity> source, string methodName, string fieldName) where TEntity : class
        {
            var selector = GenerateSelector<TEntity>(fieldName, out var selectorResultType);

            return Expression.Call(
                typeof(Queryable),
                methodName,
                [typeof(TEntity), selectorResultType],
                source.Expression,
                Expression.Quote(selector)
            );
        }
    }
}