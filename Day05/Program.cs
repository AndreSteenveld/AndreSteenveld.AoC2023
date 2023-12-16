using System.Diagnostics;
using System.Text.RegularExpressions;
using static System.Linq.Enumerable;
using static Extensions;

Extensions.WaitForDebugger();

var almanac = Console.In.ReadLines().ToArray();

var seeds = Enumerable.ToArray(
    from line in almanac.Take(1)
    from number in Regex.Matches(line, "[0-9]+")
    select UInt64.Parse(number.Value)
);

Console.WriteLine( $"Seeds to sow :: [ { String.Join(", ", seeds) } ]" );

var Mappings = new Regex(
    @"^(?<from>[a-z]*)-to-(?<to>[a-z]*) map:\n(^(?<mapping>[0-9]+ [0-9]+ [0-9]+)(\n|$))*",
    RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture
);

var maps = Mappings
    .Matches( String.Join("\n", almanac) )
    .Select( match => {

        var mapping = (from: match.Groups["from"].Value, to: match.Groups["to"].Value);

        var ranges = Enumerable.ToArray(
            from range in match.Groups["mapping"].Captures
            
            let parsed = 
                from v in range.Value.Split(" ")
                select UInt64.Parse(v)

            select parsed.ToArray()
        );

        return KeyValuePair.Create(
            mapping.to, 
            new Map(
                mapping.from,
                mapping.to,
                ranges
            )
        );

    })
    .ToDictionary();
    
var steps = new []{ "soil", "fertilizer", "water", "light", "temperature", "humidity", "location" };

var seed_mappings = seeds
    .Select( 
        value => Repeat( value, 1 )
            .Concat(
                from step in steps
                select ( value = maps[step][value] )
            )
            .ToArray()
    );

//foreach( var mapping in seed_mappings )
//    Console.WriteLine($"[ { String.Join(", ", mapping) } ]" );

var min_location = seed_mappings.Min( m => m.Last() );

Console.WriteLine( $"The lowest location is [ { min_location } ]" );


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
        where range.@from < index && index < ( range.@from + range.length )
        select range.to + ( index - range.@from )
    );

}

public static class Extensions {

    public static void WaitForDebugger(){
        if(Environment.GetEnvironmentVariable("DOTNET_WAIT_FOR_DEBUGGER") == "1"){
            Console.WriteLine("Waiting for debugger to attach...");
            
            while(Debugger.IsAttached is false)
                Thread.Sleep(500);
            
            Debugger.Break();
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
