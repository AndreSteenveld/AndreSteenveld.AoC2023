using System.Linq;
using static System.Linq.Enumerable;
using System.Text.RegularExpressions;

var lines = Console.In.ReadLines().ToArray();

var x_max = lines.Select(Enumerable.Count).Distinct().Single();
var y_max = lines.Length;

var map = String.Concat(lines);

var Number = new Regex("[1-9][0-9]*", RegexOptions.Compiled);
var Component = new Regex("[^.0-9]{1}", RegexOptions.Compiled);

List<PartNumber> part_numbers = new (
    from match in Number.Matches(map)
        
    let box = box_for_number(match.Index, match.Length)
    
    let components =
        from index in box 
        let component = $"{map[index]}"
        where Component.IsMatch(component)
        select (index, component)
        
    select new PartNumber(
        Match   : match, 
        Value   : Int32.Parse(match.Value), 
        Box     : box, 
        components
    )
);

var summed_part_numbers = Enumerable.Sum( from n in part_numbers select n.Value * n.Components.Count() );

Console.WriteLine( $"Summed part numbers are [ { summed_part_numbers} ]");

var gear_ratios = 
    from part_number in part_numbers
    from component in part_number.Components
    where component.component == "*"
    
    group part_number.Value by component.index into component_numbers_or_ratios
    
    where component_numbers_or_ratios.Count() == 2
    select component_numbers_or_ratios.Aggregate((l,r) => l * r)
    ;

Console.WriteLine($"The summed gear ratios are [ { gear_ratios.Sum() } ]");


HashSet<int> box_for_number(int start, int length){
    
    var boxes = 
        from index in Range(start, length)
        
        let coordinate = index_to_coordinate(index)
        where valid_coordinates(coordinate)
        
        select box(coordinate)
    ;

    return new HashSet<int>(
        from b in boxes
        from i in b
        select i
    );

    int coordinate_to_index((int x, int y) c ) => c.x + (x_max * c.y);
    (int x, int y) index_to_coordinate(int index) => (index % x_max, index / y_max);
        
    bool valid_coordinates((int x, int y) coordinate) => true
        && 0 <= coordinate.x && coordinate.x < x_max
        && 0 <= coordinate.y && coordinate.y < y_max
    ;

    IEnumerable<int> box((int x, int y) c) =>
        from coordinate in new (int, int)[]{
                (   c.x - 1, c.y - 1    ), (    c.x, c.y - 1    ), (    c.x + 1, c.y - 1    ),
                (   c.x - 1, c.y        ),                         (    c.x + 1, c.y        ),
                (   c.x - 1, c.y + 1    ), (    c.x, c.y + 1    ), (    c.x + 1, c.y + 1    )
            }
        where valid_coordinates(coordinate)
        select coordinate_to_index(coordinate)
    ;

}

public static class Extensions {

    public static IEnumerable<string> ReadLines(this TextReader reader){
        for( var line = reader.ReadLine(); line is not null; line = reader.ReadLine())
            yield return line;
    }

}

public record PartNumber( Match Match, int Value, HashSet<int> Box, IEnumerable<(int index, string component)> Components );
