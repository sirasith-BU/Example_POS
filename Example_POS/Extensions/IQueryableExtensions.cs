using System.Linq.Expressions;

namespace Example_POS.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyName, bool ascending)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return source;

            var parameter = Expression.Parameter(typeof(T), "p");
            var property = Expression.PropertyOrField(parameter, propertyName);
            var keySelector = Expression.Lambda(property, parameter);

            var methodName = ascending ? "OrderBy" : "OrderByDescending";

            var method = typeof(Queryable).GetMethods()
                .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                .SingleOrDefault();

            if (method == null)
            {
                throw new InvalidOperationException($"Cannot find method '{methodName}' on IQueryable.");
            }

            method = method.MakeGenericMethod(typeof(T), property.Type);

            return (IQueryable<T>)method.Invoke(null, new object[] { source, keySelector })!;
        }
    }
}
