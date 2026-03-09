using KingsCup.Core.Models;

namespace KingsCup.Tests;

public class KingsCupRulesTests
{
    [Fact]
    public void AllThirteenRanks_HaveRules()
    {
        // Arrange
        var allRanks = Enum.GetValues<Rank>();

        // Act & Assert
        foreach (var rank in allRanks)
        {
            var rule = KingsCupRules.GetRule(rank);
            Assert.NotNull(rule);
        }
    }

    [Fact]
    public void AllRules_HaveNonEmptyName()
    {
        // Arrange
        var allRanks = Enum.GetValues<Rank>();

        // Act & Assert
        foreach (var rank in allRanks)
        {
            var rule = KingsCupRules.GetRule(rank);
            Assert.False(string.IsNullOrWhiteSpace(rule.Name));
        }
    }

    [Fact]
    public void AllRules_HaveNonEmptyDescription()
    {
        // Arrange
        var allRanks = Enum.GetValues<Rank>();

        // Act & Assert
        foreach (var rank in allRanks)
        {
            var rule = KingsCupRules.GetRule(rank);
            Assert.False(string.IsNullOrWhiteSpace(rule.Description));
        }
    }

    [Fact]
    public void Ace_IsWaterfall()
    {
        // Arrange & Act
        var rule = KingsCupRules.GetRule(Rank.Ace);

        // Assert
        Assert.Equal("Waterfall", rule.Name);
        Assert.Contains("Everyone drinks", rule.Description);
    }

    [Fact]
    public void Two_IsYou()
    {
        // Arrange & Act
        var rule = KingsCupRules.GetRule(Rank.Two);

        // Assert
        Assert.Equal("You", rule.Name);
        Assert.Contains("Pick someone to drink", rule.Description);
    }

    [Fact]
    public void Three_IsMe()
    {
        // Arrange & Act
        var rule = KingsCupRules.GetRule(Rank.Three);

        // Assert
        Assert.Equal("Me", rule.Name);
        Assert.Contains("You drink", rule.Description);
    }

    [Fact]
    public void King_IsKingsCup()
    {
        // Arrange & Act
        var rule = KingsCupRules.GetRule(Rank.King);

        // Assert
        Assert.Equal("King's Cup", rule.Name);
        Assert.Contains("King's Cup", rule.Description);
        Assert.Contains("4th King", rule.Description);
    }

    [Fact]
    public void Jack_IsMakeARule()
    {
        // Arrange & Act
        var rule = KingsCupRules.GetRule(Rank.Jack);

        // Assert
        Assert.Equal("Make a Rule", rule.Name);
        Assert.Contains("rule", rule.Description, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Queen_IsQuestions()
    {
        // Arrange & Act
        var rule = KingsCupRules.GetRule(Rank.Queen);

        // Assert
        Assert.Equal("Questions", rule.Name);
        Assert.Contains("question", rule.Description, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Ten_IsCategories()
    {
        // Arrange & Act
        var rule = KingsCupRules.GetRule(Rank.Ten);

        // Assert
        Assert.Equal("Categories", rule.Name);
        Assert.Contains("category", rule.Description, StringComparison.OrdinalIgnoreCase);
    }
}
