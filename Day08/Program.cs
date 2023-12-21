using System.Text.RegularExpressions;

using AndreSteenveld.AoC;
using static AndreSteenveld.AoC.Functions;

using Step = (int step, string node);

var lines = Console.In.ReadLines().ToArray();

var directions = lines.First();

var map = Enumerable.ToDictionary(
    from line in lines.Skip(2)
    let nodes = Regex.Match(line, "([0-9A-Z]{3}) = [(]([0-9A-Z]{3}), ([0-9A-Z]{3})[)]")
    select (nodes.Groups[1].Value, (L : nodes.Groups[2].Value, R : nodes.Groups[3].Value))
);

var (steps_to_zzz, _) = map.Keys.Contains("AAA") is false 
    ? (-1, null) 
    : steps_starting_at("AAA").First( (_, node) => node is "ZZZ" );

Console.WriteLine( $"Found ZZZ at step [ { steps_to_zzz } ]" );

var steps_from_starts_to_ends = 
    (
        from node in map.Keys 
        where node is [ .., 'A' ] 
        select node
    )
    .AsParallel()
    .Select( start => (long)steps_starting_at(start).First( ( _, n ) => n is [ .., 'Z' ] ).Item1 )
    .ToArray()
    ;

var all_steps_land_on_z_after = steps_from_starts_to_ends.Aggregate(LeastCommonMultiple);

Console.WriteLine( $"All steps land on Z after [ { all_steps_land_on_z_after } ]" );

IEnumerable<Step> steps_starting_at(string start) => directions
    .Repeat()
    .Select(
        (step : 0, node : start),
        (previous, direction, step) => direction switch {
            'L' => (step + 1, map[previous.node].L),
            'R' => (step + 1, map[previous.node].R),
        }
    );