using KingsCup.Core.Models;

namespace KingsCup.Tests;

public class DeckTests
{
    [Fact]
    public void NewDeck_Has52Cards()
    {
        // Arrange & Act
        var deck = new Deck();

        // Assert
        Assert.Equal(52, deck.RemainingCards);
    }

    [Fact]
    public void DrawingAllCards_EmptiesDeck()
    {
        // Arrange
        var deck = new Deck();
        var drawnCards = new List<Card?>();

        // Act
        for (int i = 0; i < 52; i++)
        {
            drawnCards.Add(deck.DrawCard());
        }

        // Assert
        Assert.Equal(52, drawnCards.Count);
        Assert.All(drawnCards, card => Assert.NotNull(card));
        Assert.Equal(0, deck.RemainingCards);
    }

    [Fact]
    public void DrawFromEmptyDeck_ReturnsNull()
    {
        // Arrange
        var deck = new Deck();
        for (int i = 0; i < 52; i++)
        {
            deck.DrawCard();
        }

        // Act
        var card = deck.DrawCard();

        // Assert
        Assert.Null(card);
    }

    [Fact]
    public void IsEmpty_TrueWhenAllCardsDrawn()
    {
        // Arrange
        var deck = new Deck();

        // Act
        for (int i = 0; i < 52; i++)
        {
            deck.DrawCard();
        }

        // Assert
        Assert.True(deck.IsEmpty);
    }

    [Fact]
    public void IsEmpty_FalseWhenCardsRemaining()
    {
        // Arrange
        var deck = new Deck();

        // Act
        deck.DrawCard();

        // Assert
        Assert.False(deck.IsEmpty);
    }

    [Fact]
    public void RemainingCards_DecreasesAfterEachDraw()
    {
        // Arrange
        var deck = new Deck();

        // Act & Assert
        Assert.Equal(52, deck.RemainingCards);
        deck.DrawCard();
        Assert.Equal(51, deck.RemainingCards);
        deck.DrawCard();
        Assert.Equal(50, deck.RemainingCards);
        deck.DrawCard();
        Assert.Equal(49, deck.RemainingCards);
    }

    [Fact]
    public void Shuffle_ProducesDifferentOrder()
    {
        // Arrange
        var deck = new Deck();
        var firstThreeCards = new List<Card?>
        {
            deck.DrawCard(),
            deck.DrawCard(),
            deck.DrawCard()
        };

        // Act
        var deck2 = new Deck();
        deck2.Shuffle();
        var shuffledFirstThree = new List<Card?>
        {
            deck2.DrawCard(),
            deck2.DrawCard(),
            deck2.DrawCard()
        };

        // Assert - probabilistically, shuffled order should differ
        // Note: There's a tiny chance this could fail, but 1 in 132,600
        var differentOrder = !firstThreeCards.SequenceEqual(shuffledFirstThree);
        Assert.True(differentOrder, "Shuffle should produce a different card order");
    }

    [Fact]
    public void Shuffle_ResetsDrawIndex()
    {
        // Arrange
        var deck = new Deck();
        deck.DrawCard();
        deck.DrawCard();
        Assert.Equal(50, deck.RemainingCards);

        // Act
        deck.Shuffle();

        // Assert
        Assert.Equal(52, deck.RemainingCards);
    }

    [Fact]
    public void Reset_Restores52Cards()
    {
        // Arrange
        var deck = new Deck();
        for (int i = 0; i < 30; i++)
        {
            deck.DrawCard();
        }

        // Act
        deck.Reset();

        // Assert
        Assert.Equal(52, deck.RemainingCards);
        Assert.False(deck.IsEmpty);
    }

    [Fact]
    public void NewDeck_ContainsAllFourSuits()
    {
        // Arrange
        var deck = new Deck();
        var drawnCards = new List<Card?>();

        // Act
        for (int i = 0; i < 52; i++)
        {
            drawnCards.Add(deck.DrawCard());
        }

        // Assert
        var suits = drawnCards.Select(c => c!.Suit).Distinct().ToList();
        Assert.Equal(4, suits.Count);
        Assert.Contains(Suit.Hearts, suits);
        Assert.Contains(Suit.Diamonds, suits);
        Assert.Contains(Suit.Clubs, suits);
        Assert.Contains(Suit.Spades, suits);
    }

    [Fact]
    public void NewDeck_ContainsAllThirteenRanks()
    {
        // Arrange
        var deck = new Deck();
        var drawnCards = new List<Card?>();

        // Act
        for (int i = 0; i < 52; i++)
        {
            drawnCards.Add(deck.DrawCard());
        }

        // Assert
        var ranks = drawnCards.Select(c => c!.Rank).Distinct().ToList();
        Assert.Equal(13, ranks.Count);
        Assert.Contains(Rank.Ace, ranks);
        Assert.Contains(Rank.Two, ranks);
        Assert.Contains(Rank.Three, ranks);
        Assert.Contains(Rank.Four, ranks);
        Assert.Contains(Rank.Five, ranks);
        Assert.Contains(Rank.Six, ranks);
        Assert.Contains(Rank.Seven, ranks);
        Assert.Contains(Rank.Eight, ranks);
        Assert.Contains(Rank.Nine, ranks);
        Assert.Contains(Rank.Ten, ranks);
        Assert.Contains(Rank.Jack, ranks);
        Assert.Contains(Rank.Queen, ranks);
        Assert.Contains(Rank.King, ranks);
    }

    [Fact]
    public void NewDeck_HasExactlyFourOfEachRank()
    {
        // Arrange
        var deck = new Deck();
        var drawnCards = new List<Card?>();

        // Act
        for (int i = 0; i < 52; i++)
        {
            drawnCards.Add(deck.DrawCard());
        }

        // Assert
        var rankCounts = drawnCards
            .GroupBy(c => c!.Rank)
            .Select(g => new { Rank = g.Key, Count = g.Count() });

        foreach (var rankCount in rankCounts)
        {
            Assert.Equal(4, rankCount.Count);
        }
    }
}
