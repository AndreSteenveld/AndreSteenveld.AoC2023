using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static System.Linq.Enumerable;
using static Extensions;

Extensions.WaitForDebugger();

var almanac = Console.In.ReadLines().ToArray();

var Mappings = new Regex(
    @"^(?<from>[a-z]*)-to-(?<to>[a-z]*) map:\n(^(?<mapping>[0-9]+ [0-9]+ [0-9]+)(\n|$))*",
    RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture
);

var maps = Mappings
    .Matches( String.Join("\n", almanac) )
    .Select( match => {

        var @from = match.Groups["from"].Value;
        var to = match.Groups["to"].Value;

        var ranges = Enumerable.ToArray(
            from range in match.Groups["mapping"].Captures
            
            let parsed = 
                from v in range.Value.Split(" ")
                select UInt64.Parse(v)

            select parsed.ToArray()
        );

        return KeyValuePair.Create(
            to, 
            new Map(@from, to, ranges)
        );

    })
    .ToDictionary();

var steps = new []{ "seeds", "soil", "fertilizer", "water", "light", "temperature", "humidity", "location" };

var mappings_from_seeds_as_list = 
    from line in almanac.Take(1)
    from match in Regex.Matches(line, "[0-9]+")
    
    let seed = UInt64.Parse(match.Value)

    select steps.Skip(1).Aggregate(
        CreateArray<ulong>(steps.Length, seed),
        map_steps_over_seed
    );

Console.WriteLine( $"Lowest location for seeds as list [ { mappings_from_seeds_as_list.Min(Enumerable.Last) } ]" );

//foreach(var mapping in mappings_from_seeds_as_list)
//    Console.WriteLine( $"\t [ { String.Join(", ", mapping ) } ]" );

var ranges_from_seeds =
    from seed_line in almanac.Take(1)
    from match in Regex.Matches(seed_line, "([0-9]+) ([0-9]+)")
    
    let start = UInt64.Parse(match.Groups[1].Value)
    let length = UInt64.Parse(match.Groups[2].Value)
    select ULongRange(start, length)
    ;

var mappings_from_seeds_as_ranges =
    from range in ranges_from_seeds.AsParallel()
    from seed in range 
    select steps.Skip(1).Aggregate(
        CreateArray<ulong>(steps.Length, seed),
        map_steps_over_seed
    )
    ;

//
// Running this parrallel version on my mac mini 2011:
//  Lowest location for seeds as list [ 379811651 ]
//  Lowest location for seeds as range [ 27992443 ]
//      13078.33 user 
//      338.08 system 
//      1:32:24 elapsed 
//      241% CPU 
//
var min_location_from_seeds_as_ranges = mappings_from_seeds_as_ranges.Min(Enumerable.Last);

Console.WriteLine( $"Lowest location for seeds as range [ { min_location_from_seeds_as_ranges } ]");

//foreach(var mapping in mappings_from_seeds_as_ranges)
//    Console.WriteLine( $"\t [ { String.Join(", ", mapping ) } ]" );


ulong[] map_steps_over_seed( ulong[] result, string step ){
    var index = Array.IndexOf(steps, step);
    var value = result[index - 1];
    result[index] = maps[step][value];
    return result;
}

class Map {

    public string From { get; init; }
    public string To { get; init; }

    private (ulong to, ulong @from, ulong length)[] map;

    public Map(string From, string To, ulong[][] ranges){
        this.From = From;
        this.To = To;

        this.map = ranges
            .OrderBy( t => t[1] )
            .Select( r => (r[0], r[1], r[2]) )
            .Append( (UInt64.MinValue, UInt64.MinValue, UInt64.MaxValue) )
            .ToArray();

    }

    public ulong this[ulong index] => Enumerable.First<ulong>(
        from range in map
        where range.@from <= index && index < ( range.@from + range.length )
        select range.to + ( index - range.@from )
    );

}

public static class Extensions {

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

    public static IEnumerable<ulong> ULongRange(ulong start, ulong count){
        for(ulong current = 0; current < count; ++current)
            yield return start + current;
    }

    public static IEnumerable<long> LongRange(long start, long count){
        for(long current = 0; current < count; ++current)
            yield return start + current;
    }

}
