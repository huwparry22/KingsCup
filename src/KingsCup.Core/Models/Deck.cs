namespace KingsCup.Core.Models;

public class Deck
{
    private readonly List<Card> _cards = new();
    private int _currentIndex = 0;

    public int RemainingCards => _cards.Count - _currentIndex;
    public bool IsEmpty => _currentIndex >= _cards.Count;

    public Deck()
    {
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        _cards.Clear();
        _currentIndex = 0;

        foreach (Suit suit in Enum.GetValues<Suit>())
        {
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                _cards.Add(new Card(suit, rank));
            }
        }
    }

    public void Shuffle()
    {
        var rng = new Random();
        int n = _cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (_cards[k], _cards[n]) = (_cards[n], _cards[k]);
        }
        _currentIndex = 0;
    }

    public Card? DrawCard()
    {
        if (IsEmpty)
            return null;

        return _cards[_currentIndex++];
    }

    public void Reset()
    {
        InitializeDeck();
        Shuffle();
    }
}
