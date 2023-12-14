
var cards = Console.In.ReadLines().Select(Card.FromLine).ToArray();

var card_points =
    from card in cards
    where card.NumberOfWinningNumbers > 0
    select Math.Pow(2, card.NumberOfWinningNumbers - 1);

Console.WriteLine($"The sum of all winning scratch cards [ { card_points.Sum() } ]");

var number_of_cards = cards
    .Aggregate(
        Enumerable.Repeat(1, cards.Length).ToArray(),
        (card_counters, card) => {
            
            var index = Array.IndexOf(cards, card);

            for( var n = card_counters[index]; n > 0; n-- )
                for( var i = 1; i <= card.NumberOfWinningNumbers; i++ )
                    card_counters[index + i] += 1;
            
            return card_counters;

        }
    );

Console.WriteLine($"The total number of cards scratched [ { number_of_cards.Sum() } ]");


record Card( string CardNo, int[] Needles, int[] Haystack ){

    private static readonly char[] splitter = [':', '|'];

    public static Card FromLine(string line){
        var (CardNo, Needles, Haystack, _) = line.Split(splitter);

        return new ( 
            
            CardNo, 

            Needles
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(Int32.Parse)
                .ToArray(),
            
            Haystack
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(Int32.Parse)
                .ToArray()
        
        );
        
    }

    public int NumberOfWinningNumbers => Enumerable.Intersect( Haystack, Needles ).Count();
}

public static class Extensions {

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

}
