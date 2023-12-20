using AndreSteenveld.AoC;
using static AndreSteenveld.AoC.Functions;

using Hand = (string cards, int bet);

WaitForDebugger();

var lines = Console.In.ReadLines().ToArray();

var hands = lines
    .Select( line => {
        var ( cards, bet, _ ) = line.Split(" ");
        return (cards, Int32.Parse(bet));
    })
    .Cast<Hand>();

var jack_ordered_hands =
    from hand in hands orderby hand_type(hand.cards, Card.JACK), high_card(hand.cards, Card.JACK)
    select hand
    ;

var jack_winnings = 
    from indexed in jack_ordered_hands.WithIndex()
    select indexed.item.bet * (indexed.index + 1)
    ;

Console.WriteLine( $"Total winning with jacks being jacks [ { jack_winnings.Sum() } ]" );

var joker_ordered_hands =
    from hand in hands orderby hand_type(hand.cards, Card.JOKER), high_card(hand.cards, Card.JOKER)
    select hand
    ;

var joker_winnings =
    from indexed in joker_ordered_hands.WithIndex()
    select indexed.item.bet * (indexed.index + 1)
    ;

Console.WriteLine( $"Total winning with jacks being jokers [ { joker_winnings.Sum() } ]" );
// Wrong answers for joker sum:
//  250555672
//  251134987


// var differing_hands =
//     from hand in hands
//     let jack = hand_type(hand.cards, Card.JACK)
//     let joker = hand_type(hand.cards, Card.JOKER)
//     where jack != joker
//     orderby jack, joker
//     select (hand.cards, jack, joker)
//     ;

// foreach(var hand in differing_hands)
//     Console.WriteLine(hand);

uint high_card(string cards, Card jack_or_joker) =>
    0
    | (uint)(card(cards[0], jack_or_joker) << 4 * 4)
    | (uint)(card(cards[1], jack_or_joker) << 4 * 3)
    | (uint)(card(cards[2], jack_or_joker) << 4 * 2)
    | (uint)(card(cards[3], jack_or_joker) << 4 * 1)
    | (uint)(card(cards[4], jack_or_joker) << 4 * 0)
    ;

HandType hand_type(string cards, Card jack_or_joker) => (count_cards(cards), jack_or_joker) switch {
    ({ Length : 1 }, _) => HandType.FIVE_OF_A_KIND,
    
    ({ Length : 2 } and [4, 1], Card.JOKER) 
        when distinct_cards(cards).Contains('J') => HandType.FIVE_OF_A_KIND,

    ({ Length : 2 } and [4, 1], _) => HandType.FOUR_OF_A_KIND,

    ({ Length : 2 } and [3, 2], Card.JOKER) 
        when distinct_cards(cards).Contains('J') => HandType.FIVE_OF_A_KIND,

    ({ Length : 2 } and [3, 2], _) => HandType.FULL_HOUSE,

    ({ Length : 3 } and [3, 1, 1], Card.JOKER) 
        when distinct_cards(cards).Contains('J') => HandType.FOUR_OF_A_KIND,

    ({ Length : 3 } and [3, 1, 1], _) => HandType.THREE_OF_A_KIND,
    
    ({ Length : 3 } and [2, 2, 1], Card.JOKER) => distinct_cards(cards) switch {
        [ 'J', _, _ ] => HandType.FOUR_OF_A_KIND,
        [ _, 'J', _ ] => HandType.FOUR_OF_A_KIND,
        [ _, _, 'J' ] => HandType.FULL_HOUSE,
        [ .. ] => HandType.TWO_PAIR
    },

    ({ Length : 3 } and [2, 2, 1], _) => HandType.TWO_PAIR,
    
    ({ Length : 4 }, Card.JOKER) when distinct_cards(cards).Contains('J') => HandType.THREE_OF_A_KIND,
    ({ Length : 4 }, _) => HandType.ONE_PAIR,

    ({ Length : 5 }, Card.JOKER) when distinct_cards(cards).Contains('J') => HandType.ONE_PAIR,
    ({ Length : 5 }, _) => HandType.HIGH_CARD,

    (_, _) => throw new Exception("Unknown hand type"),
};

char[] distinct_cards(string cards) => Enumerable.ToArray(
    from card in cards
    group card by card into type
    orderby type.Count() descending, card(type.Key, Card.JOKER)
    select type.Key
);

int[] count_cards(string cards) => Enumerable.ToArray(
    from card in cards
    group card by card into type
    select type.Count() into count
    orderby count descending
    select count
);


byte card(char card, Card jack_or_joker) => card switch {
    '2' => (byte)Card.TWO, 
    '3' => (byte)Card.THREE,   
    '4' => (byte)Card.FOUR,    
    '5' => (byte)Card.FIVE,    
    '6' => (byte)Card.SIX,     
    '7' => (byte)Card.SEVEN,   
    '8' => (byte)Card.EIGHT,   
    '9' => (byte)Card.NINE,    
    'T' => (byte)Card.TEN,     
    'J' => (byte)jack_or_joker,    
    'Q' => (byte)Card.QUEEN,   
    'K' => (byte)Card.KING,    
    'A' => (byte)Card.ACE,
    _ => throw new Exception( "Unknown card" ),
};

[Flags]
enum HandType : byte {
    FIVE_OF_A_KIND  = 0b0100_0000,
    FOUR_OF_A_KIND  = 0b0010_0000,
    FULL_HOUSE      = 0b0001_0000,
    THREE_OF_A_KIND = 0b0000_1000,
    TWO_PAIR        = 0b0000_0100,
    ONE_PAIR        = 0b0000_0010,
    HIGH_CARD       = 0b0000_0001
}

enum Card : byte {
    JOKER   = 0,
    TWO     = 2, 
    THREE   = 3, 
    FOUR    = 4, 
    FIVE    = 5, 
    SIX     = 6, 
    SEVEN   = 7, 
    EIGHT   = 8, 
    NINE    = 9, 
    TEN     = 10, 
    JACK    = 11, 
    QUEEN   = 12, 
    KING    = 13, 
    ACE     = 14
}