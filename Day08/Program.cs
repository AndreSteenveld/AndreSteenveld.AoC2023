using System.Text.RegularExpressions;

using AndreSteenveld.AoC;
using static AndreSteenveld.AoC.Functions;

var lines = Console.In.ReadLines().ToArray();

var directions = lines.First();

var map = Enumerable.ToDictionary(
    from line in lines.Skip(1)
    let nodes = Regex.Match(line, "([A-Z]{3}) = [(]([A-Z]{3}), ([A-Z]{3})[)]")
    select (nodes.Groups[1].Value, (L : nodes.Groups[2].Value, R : nodes.Groups[3].Value))
);

var step = directions
    .Repeat()
    .Select(
        (step : 0, node : "AAA"),
        (previous, direction, step) => direction switch {
            'L' => (step + 1, map[previous.node].L),
            'R' => (step + 1, map[previous.node].R),
        }
    )
    .First( (step, node) => node is "ZZZ" );

Console.WriteLine( $"Found ZZZ at step [ { step.Item1 } ]" );

