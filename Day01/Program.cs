using System;
using System.Text.RegularExpressions;

var expression = "(one|two|three|four|five|six|seven|eight|nine|[1-9])";

var first = new Regex(expression);
var last = new Regex(expression, RegexOptions.RightToLeft);

var result = Lines(Console.In)
    .Select( 
        line => (
            first:first.Match(line), 
            last:last.Match(line)
        ) 
    )
    .Select( t => $"{t.first}{t.last}" )
    .Select( number => number
        .Replace("one",     "1")
        .Replace("two",     "2")
        .Replace("three",   "3")
        .Replace("four",    "4")
        .Replace("five",    "5")
        .Replace("six",     "6")
        .Replace("seven",   "7")
        .Replace("eight",   "8")
        .Replace("nine",    "9")
    )
    .Select(Int32.Parse);


Console.WriteLine( $"The sum of all calibration values is [{ result.Sum() }]");


IEnumerable<string> Lines(TextReader reader){
    for( var r = reader.ReadLine(); r is not null; r = reader.ReadLine())
        yield return r;
}