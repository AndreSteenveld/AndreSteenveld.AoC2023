using System.Text.RegularExpressions;

var games = Console.In.ReadLines().Select(Game.FromLine).ToArray();

var valid_games = games.Where(Game.IsValid);

var cubed_games = games
    .Select( game => {
        var (r, g, b) = game.Cubes();
        return r * g * b;
    });

Console.WriteLine( $"The there were [ { valid_games.Count() } ] valid games. The sum of all game IDs is [ { valid_games.Sum( game => game.Id ) } ]");
Console.WriteLine( $"Summed value of all cubed game [ { cubed_games.Sum() } ]");


public static class Extensions {

    public static IEnumerable<string> ReadLines(this TextReader reader){
        for( var line = reader.ReadLine(); line is not null; line = reader.ReadLine())
            yield return line;
    }

    public static string? MatchGroup(this Regex @this, string input) =>
        @this.Match(input).Groups.Values.ToArray() switch {
            { Length : < 2 } => null,
            { Length : 2 } groups => groups[1].Value,
            _ => throw new Exception($"Too many groups [ ${@this} ]")
        };

}

record Game(int Id, IEnumerable<(int red, int green, int blue)> Hands){

    private static Regex GameId = new("^Game ([0-9]+):");
    private static Regex GameHands = new("^Game [0-9]+:(.*)");

    private static Regex Red = new("([1-9][0-9]*) red");
    private static Regex Green = new("([1-9][0-9]*) green");
    private static Regex Blue = new("([1-9][0-9]*) blue");

    private static IEnumerable<(int, int, int)> hands_from_line(string line) =>
        from hand in GameHands.MatchGroup(line)!.Split(';')
        select (
            red     : Int32.Parse("0" + Red.MatchGroup(hand) ?? ""),
            green   : Int32.Parse("0" + Green.MatchGroup(hand) ?? ""),
            blue    : Int32.Parse("0" + Blue.MatchGroup(hand) ?? "")
        );

    private static int game_id_from_line(string line) =>
        Int32.Parse( GameId.MatchGroup(line)! );


    public static Game FromLine(string line) => new Game(
        game_id_from_line(line),
        hands_from_line(line)
    );

    public static bool IsValid(Game game) => 
        game.Hands.All( hand => {
            var (red, green, blue) = hand;
            return red <= 12 
                && green <= 13
                && blue <= 14;
        });

    public (int, int, int) Cubes() => 
        Hands.Aggregate( 
            (red:0, green:0, blue:0), 
            (result, hand) => (
                Math.Max(result.red,    hand.red),
                Math.Max(result.green,  hand.green),
                Math.Max(result.blue,   hand.blue)
            )
        );

}
