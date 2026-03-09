using KingsCup.Core.Models;

namespace KingsCup.Tests;

public class PlayerTests
{
    [Fact]
    public void NewPlayer_GeneratesUniqueId()
    {
        // Arrange & Act
        var player1 = new Player { Name = "Alice" };
        var player2 = new Player { Name = "Bob" };

        // Assert
        Assert.NotNull(player1.Id);
        Assert.NotNull(player2.Id);
        Assert.NotEqual(player1.Id, player2.Id);
    }

    [Fact]
    public void NewPlayer_RequiresName()
    {
        // Arrange & Act
        var player = new Player { Name = "Charlie" };

        // Assert
        Assert.Equal("Charlie", player.Name);
    }

    [Fact]
    public void NewPlayer_DefaultIsConnectedIsTrue()
    {
        // Arrange & Act
        var player = new Player { Name = "Dave" };

        // Assert
        Assert.True(player.IsConnected);
    }

    [Fact]
    public void NewPlayer_JoinedAtIsSet()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var player = new Player { Name = "Eve" };

        // Assert
        var after = DateTime.UtcNow;
        Assert.InRange(player.JoinedAt, before.AddSeconds(-1), after.AddSeconds(1));
    }

    [Fact]
    public void Player_NameCanBeChanged()
    {
        // Arrange
        var player = new Player { Name = "Frank" };

        // Act
        player.Name = "Franklin";

        // Assert
        Assert.Equal("Franklin", player.Name);
    }

    [Fact]
    public void Player_IsConnectedCanBeChanged()
    {
        // Arrange
        var player = new Player { Name = "Grace" };

        // Act
        player.IsConnected = false;

        // Assert
        Assert.False(player.IsConnected);
    }

    [Fact]
    public void Player_ConnectionIdCanBeSet()
    {
        // Arrange
        var player = new Player { Name = "Henry" };

        // Act
        player.ConnectionId = "conn-123";

        // Assert
        Assert.Equal("conn-123", player.ConnectionId);
    }

    [Fact]
    public void Player_DefaultConnectionIdIsEmpty()
    {
        // Arrange & Act
        var player = new Player { Name = "Iris" };

        // Assert
        Assert.Equal(string.Empty, player.ConnectionId);
    }
}
