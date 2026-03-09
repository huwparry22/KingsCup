using KingsCup.Core.Models;

namespace KingsCup.Tests;

public class CardTests
{
    [Fact]
    public void ToString_FormatsCardCorrectly()
    {
        // Arrange
        var card = new Card(Suit.Hearts, Rank.Ace);

        // Act
        var result = card.ToString();

        // Assert
        Assert.Equal("Ace of Hearts", result);
    }

    [Fact]
    public void RecordEquality_TwoIdenticalCards_AreEqual()
    {
        // Arrange
        var card1 = new Card(Suit.Diamonds, Rank.King);
        var card2 = new Card(Suit.Diamonds, Rank.King);

        // Act & Assert
        Assert.Equal(card1, card2);
    }

    [Fact]
    public void RecordEquality_TwoDifferentCards_AreNotEqual()
    {
        // Arrange
        var card1 = new Card(Suit.Clubs, Rank.Two);
        var card2 = new Card(Suit.Spades, Rank.Two);

        // Act & Assert
        Assert.NotEqual(card1, card2);
    }

    [Fact]
    public void RecordEquality_DifferentRanks_AreNotEqual()
    {
        // Arrange
        var card1 = new Card(Suit.Hearts, Rank.Three);
        var card2 = new Card(Suit.Hearts, Rank.Four);

        // Act & Assert
        Assert.NotEqual(card1, card2);
    }
}
