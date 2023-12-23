
using AndreSteenveld.AoC;
using static AndreSteenveld.AoC.Functions;

var lines = Console.In.ReadLines();

var measurements = Enumerable.ToArray(
    from line in lines
    select 
        (
            from number in line.Split(" ")
            select Int32.Parse(number)
        )
        .ToArray()
);


var extrapolated_future_values = ( from line in measurements select extrapolate_future_value(line) ).AsParallel();
var extrapolated_historic_values = ( from line in measurements select extrapolate_historic_value(line) ).AsParallel();

Console.WriteLine( $"Summed value of future extrapolations is [ { extrapolated_future_values.Sum() } ]" );
Console.WriteLine( $"Summed value of historic extrapolations is [ { extrapolated_historic_values.Sum() } ]" );

int extrapolate_future_value(int[] top) =>    
    triangle(top).Reverse().Select( 0, (p, line) => line[^1] + p ).Last();

int extrapolate_historic_value(int[] top) =>
    triangle(top).Reverse().Select( 0, (p, line) => line[0] - p ).Last();

int[][] triangle(int[] top) =>
    Generate(top, differences)
        .TakeWhile( 
            row => row is not { Length : 0 } 
                && row is not ([ 0 , .. , 0 ] or [ 0, 0 ]) 
        )
        .Prepend(top)
        .ToArray();

int[] differences(int[] numbers){
    int[] result = (int[])Array.CreateInstance(typeof(int), numbers.Length - 1);
    
    for(int i = result.Length - 1; i >= 0; i-- )
        result[ i ] = numbers[ i + 1 ] - numbers[ i ];

    return result;
}