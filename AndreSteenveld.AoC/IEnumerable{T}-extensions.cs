namespace AndreSteenveld.AoC;

public static partial class EnumerableExtensions {

    public static long Product(this IEnumerable<long> source) =>
        source.Aggregate((p, v) => p * v);

    public static long Product(this IEnumerable<int> source) =>
        source.Convert<int, long>().Product();

    public static long Product<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector) =>
        source.Select<TSource, int>(selector).Product();
    
    public static IEnumerable<TTarget> Convert<TSource, TTarget>(this IEnumerable<TSource> source, IFormatProvider? provider = null) where TSource : IConvertible =>
        from element in source select (TTarget)element.ToType(typeof(TTarget), provider);

    public static IEnumerable<(TSource item, int index)> WithIndex<TSource>(this IEnumerable<TSource> source) =>
        source.Select(ValueTuple.Create<TSource, int>);

    #region Tuple splatting select
    public static IEnumerable<TResult> Select<T1, T2, TResult>(this IEnumerable<(T1, T2)> source, Func<T1, T2, TResult> selector)
        => source.Select( t => selector(t.Item1, t.Item2) );

    public static IEnumerable<TResult> Select<T1, T2, T3, TResult>(this IEnumerable<(T1, T2, T3)> source, Func<T1, T2, T3, TResult> selector)
        => source.Select( t => selector(t.Item1, t.Item2, t.Item3) );

    public static IEnumerable<TResult> Select<T1, T2, T3, T4, TResult>(this IEnumerable<(T1, T2, T3, T4)> source, Func<T1, T2, T3, T4, TResult> selector)
        => source.Select( t => selector(t.Item1, t.Item2, t.Item3, t.Item4) );

    public static IEnumerable<TResult> Select<T1, T2, T3, T4, T5, TResult>(this IEnumerable<(T1, T2, T3, T4, T5)> source, Func<T1, T2, T3, T4, T5, TResult> selector)
        => source.Select( t => selector(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5) );

    public static IEnumerable<TResult> Select<T1, T2, T3, T4, T5, T6, TResult>(this IEnumerable<(T1, T2, T3, T4, T5, T6)> source, Func<T1, T2, T3, T4, T5, T6, TResult> selector)
        => source.Select( t => selector(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6) );

    public static IEnumerable<TResult> Select<T1, T2, T3, T4, T5, T6, T7, TResult>(this IEnumerable<(T1, T2, T3, T4, T5, T6, T7)> source, Func<T1, T2, T3, T4, T5, T6, T7, TResult> selector)
        => source.Select( t => selector(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7) );

    #endregion

}