using System.Text.RegularExpressions;

var result = Lines(Console.In)
    .Select(Game.FromLine)
    .Where(Game.IsValid)
    .ToArray();

Console.WriteLine( $"The there were [ { result.Count() } ] valid games. The sum of all game IDs is [{ result.Sum( game => game.Id ) }]");


IEnumerable<string> Lines(TextReader reader){
    for( var r = reader.ReadLine(); r is not null; r = reader.ReadLine())
        yield return r;
}

public static class Extensions {

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

}
