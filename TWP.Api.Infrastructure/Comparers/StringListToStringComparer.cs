using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace TWP.Api.Infrastructure.Comparers
{
    public class StringListToStringComparer : ValueComparer<List<string>>
    {
        public StringListToStringComparer() : base(ListStringEqualsExpression(), ListStringHashCodeExpression()) { }

        private static Expression<Func<List<string>, List<string>, bool>> ListStringEqualsExpression() => (a, b) => a.SequenceEqual(b);
        private static Expression<Func<List<string>, int>> ListStringHashCodeExpression() => a => a.Sum(a => a.GetHashCode());
    }
}
