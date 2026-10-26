namespace WhoHolds.Cli.Extensions;

public static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        public int Max(Func<T, int> selector, int min) => source.Select(selector).Append(min).Max();
    }
}
