using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

using static System.Linq.Enumerable;
using static Extensions;

using Race = (int time, int distance);
using Times = System.Collections.Generic.IEnumerable<int>;

var lines = Console.In.ReadLines().ToArray();

var times =
    from number in Regex.Matches(lines[0], "[0-9]+")
    select Int32.Parse(number.Value);

var distances =
    from number in Regex.Matches(lines[1], "[0-9]+")
    select Int32.Parse(number.Value);

var races = Enumerable.Zip(times, distances).Cast<Race>();

var races_and_press_times = Enumerable.Zip(
    races,
    from race in races
    let min_press = Range(1, race.time).First( press => (race.time - press) * press > race.distance)
    let offset = Range(min_press, race.time - min_press).Count( press => (race.time - press) * press > race.distance)
    select Range(min_press, offset)
);

var product_of_win = Extensions.Product(
    from race in races_and_press_times.Cast<(Race race, Times press_times)>().AsParallel()
    select race.press_times.Count()
);

Console.WriteLine( $"The product of winning [ { product_of_win } ]" );

var ways_of_winning_the_race = 
    from race in new (long time, long distance)[]{(
        Int64.Parse(String.Concat(times)),
        Int64.Parse(String.Concat(distances))
    )}
    let min_press = LongRange(1, race.time).AsParallel().First( press => (race.time - press) * press > race.distance)
    let offset = LongRange(min_press, race.time - min_press).AsParallel().Count( press => (race.time - press) * press > race.distance)
    select offset;

Console.WriteLine( $"You can win the race in [ { ways_of_winning_the_race.Single() } ] ways");


public static class Extensions {


    public static long Product(this IEnumerable<long> source) =>
        source.Aggregate((p, v) => p * v);

    public static long Product(this IEnumerable<int> source) =>
        source.Convert<int, long>().Product();

    public static long Product<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector) =>
        source.Select<TSource, int>(selector).Product();
    
    public static IEnumerable<TTarget> Convert<TSource, TTarget>(this IEnumerable<TSource> source, IFormatProvider? provider = null) where TSource : IConvertible =>
        from element in source select (TTarget)element.ToType(typeof(TTarget), provider);

    public static T[] CreateArray<T>( int capacity, params T[] values ){
        Array.Resize(ref values, capacity);
        return values;
    }
    
    public static IEnumerable<TResult> Select<T1, T2, TResult>(this IEnumerable<(T1, T2)> source, Func<T1, T2, TResult> selector)
        => source.Select( t => selector(t.Item1, t.Item2) );

    public static void WaitForDebugger(){
        if(Environment.GetEnvironmentVariable("DOTNET_WAIT_FOR_DEBUGGER") == "1"){
            Console.WriteLine("Waiting for debugger to attach...");
            
            while(Debugger.IsAttached is false)
                Thread.Sleep(500);

        }
    }

    public static IEnumerable<string> ReadLines(this TextReader reader){
        for( var line = reader.ReadLine(); line is not null; line = reader.ReadLine())
            yield return line;
    }

    public static void Deconstruct<T>(this T[] array, out T first, out T[] rest){
        first = array.Length > 0 ? array[0] : default(T)!;
        rest = array.Skip(1).ToArray();
    }

    public static void Deconstruct<T>(this T[] array, out T first, out T second, out T[] rest)
        => (first, (second, rest)) = array;

    public static void Deconstruct<T>(this T[] array, out T first, out T second, out T third, out T[] rest)
        => (first, second, (third, rest)) = array;

    public static void Deconstruct<T>(this T[] array, out T first, out T second, out T third, out T fourth, out T[] rest)
        => (first, second, third, (fourth, rest)) = array;

    public static void Deconstruct<T>(this T[] array, out T first, out T second, out T third, out T fourth, out T fifth, out T[] rest)
        => (first, second, third, fourth, (fifth, rest)) = array;

    public static IEnumerable<long> LongRange(long start, long count){
        for(long current = 0; current < count; ++current)
            yield return start + current;
    }
}