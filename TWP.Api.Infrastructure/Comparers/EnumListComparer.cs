using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace TWP.Api.Infrastructure.Comparers
{
    public class EnumListComparer<T> : ValueComparer<List<T>>
       where T : Enum
    {
        public EnumListComparer() : base(EnumListEqualsExpression(), EnumListGetHashCodeExpression()) { }

        private static Expression<Func<List<T>, List<T>, bool>> EnumListEqualsExpression() => (a, b) => a.SequenceEqual(b);
        private static Expression<Func<List<T>, int>> EnumListGetHashCodeExpression() => a => a.Sum(a => a.GetHashCode());
    }
}
