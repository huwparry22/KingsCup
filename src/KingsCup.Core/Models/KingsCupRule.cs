namespace KingsCup.Core.Models;

public record KingsCupRule(string Name, string Description);

public static class KingsCupRules
{
    private static readonly Dictionary<Rank, KingsCupRule> _rules = new()
    {
        { Rank.Ace, new KingsCupRule("Waterfall", "Everyone drinks in order. You can't stop until the person before you stops.") },
        { Rank.Two, new KingsCupRule("You", "Pick someone to drink.") },
        { Rank.Three, new KingsCupRule("Me", "You drink.") },
        { Rank.Four, new KingsCupRule("Floor", "Last person to touch the floor drinks.") },
        { Rank.Five, new KingsCupRule("Guys", "All guys drink.") },
        { Rank.Six, new KingsCupRule("Chicks", "All girls drink.") },
        { Rank.Seven, new KingsCupRule("Heaven", "Last person to point up drinks.") },
        { Rank.Eight, new KingsCupRule("Mate", "Pick a drinking buddy. When you drink, they drink.") },
        { Rank.Nine, new KingsCupRule("Rhyme", "Say a word, go around rhyming. First to fail drinks.") },
        { Rank.Ten, new KingsCupRule("Categories", "Pick a category, go around naming things. First to fail drinks.") },
        { Rank.Jack, new KingsCupRule("Make a Rule", "Create a rule everyone must follow. Break it and drink.") },
        { Rank.Queen, new KingsCupRule("Questions", "You're the question master. Anyone who answers your questions drinks.") },
        { Rank.King, new KingsCupRule("King's Cup", "Pour your drink into the King's Cup. The 4th King drinks it all.") }
    };

    public static KingsCupRule GetRule(Rank rank) => _rules[rank];
}
