namespace KingsCup.Core.Models;

public record Card(Suit Suit, Rank Rank)
{
    public override string ToString() => $"{Rank} of {Suit}";
}
