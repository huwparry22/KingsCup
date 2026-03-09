using KingsCup.Core.Models;

namespace KingsCup.Tests;

public class GameRoomTests
{
    [Fact]
    public void NewGameRoom_RoomCodeIsSixCharacters()
    {
        // Arrange & Act
        var room = new GameRoom();

        // Assert
        Assert.Equal(6, room.RoomCode.Length);
    }

    [Fact]
    public void RoomCode_OnlyContainsValidCharacters()
    {
        // Arrange
        const string validChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var room = new GameRoom();

        // Act & Assert
        foreach (char c in room.RoomCode)
        {
            Assert.Contains(c, validChars);
        }
        
        // Ensure excluded characters are not present
        Assert.DoesNotContain('I', room.RoomCode);
        Assert.DoesNotContain('O', room.RoomCode);
        Assert.DoesNotContain('0', room.RoomCode);
        Assert.DoesNotContain('1', room.RoomCode);
    }

    [Fact]
    public void NewGameRoom_InitialStateIsLobby()
    {
        // Arrange & Act
        var room = new GameRoom();

        // Assert
        Assert.Equal(GameState.Lobby, room.State);
    }

    [Fact]
    public void AddPlayer_AddsToPlayerList()
    {
        // Arrange
        var room = new GameRoom();
        var player = new Player { Name = "Alice" };

        // Act
        room.AddPlayer(player);

        // Assert
        Assert.Contains(player, room.Players);
        Assert.Single(room.Players);
    }

    [Fact]
    public void AddPlayer_ThrowsIfGameNotInLobby()
    {
        // Arrange
        var room = new GameRoom();
        var player1 = new Player { Name = "Alice" };
        var player2 = new Player { Name = "Bob" };
        var player3 = new Player { Name = "Charlie" };
        
        room.AddPlayer(player1);
        room.AddPlayer(player2);
        room.StartGame();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => room.AddPlayer(player3));
        Assert.Contains("Cannot join game in progress", exception.Message);
    }

    [Fact]
    public void AddPlayer_ThrowsIfNameAlreadyTaken_CaseInsensitive()
    {
        // Arrange
        var room = new GameRoom();
        var player1 = new Player { Name = "Alice" };
        var player2 = new Player { Name = "alice" };
        
        room.AddPlayer(player1);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => room.AddPlayer(player2));
        Assert.Contains("Player name already taken", exception.Message);
    }

    [Fact]
    public void AddPlayer_ThrowsIfNameAlreadyTaken_DifferentCase()
    {
        // Arrange
        var room = new GameRoom();
        var player1 = new Player { Name = "Bob" };
        var player2 = new Player { Name = "BOB" };
        
        room.AddPlayer(player1);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => room.AddPlayer(player2));
        Assert.Contains("Player name already taken", exception.Message);
    }

    [Fact]
    public void CanStart_FalseWithZeroPlayers()
    {
        // Arrange
        var room = new GameRoom();

        // Act & Assert
        Assert.False(room.CanStart);
    }

    [Fact]
    public void CanStart_FalseWithOnePlayer()
    {
        // Arrange
        var room = new GameRoom();
        var player = new Player { Name = "Alice" };
        room.AddPlayer(player);

        // Act & Assert
        Assert.False(room.CanStart);
    }

    [Fact]
    public void CanStart_TrueWithTwoPlayers()
    {
        // Arrange
        var room = new GameRoom();
        var player1 = new Player { Name = "Alice" };
        var player2 = new Player { Name = "Bob" };
        room.AddPlayer(player1);
        room.AddPlayer(player2);

        // Act & Assert
        Assert.True(room.CanStart);
    }

    [Fact]
    public void CanStart_TrueWithThreePlayers()
    {
        // Arrange
        var room = new GameRoom();
        room.AddPlayer(new Player { Name = "Alice" });
        room.AddPlayer(new Player { Name = "Bob" });
        room.AddPlayer(new Player { Name = "Charlie" });

        // Act & Assert
        Assert.True(room.CanStart);
    }

    [Fact]
    public void StartGame_ShufflesDeckAndSetsPlayingState()
    {
        // Arrange
        var room = new GameRoom();
        room.AddPlayer(new Player { Name = "Alice" });
        room.AddPlayer(new Player { Name = "Bob" });

        // Act
        room.StartGame();

        // Assert
        Assert.Equal(GameState.Playing, room.State);
        Assert.NotNull(room.StartedAt);
        Assert.Equal(52, room.Deck.RemainingCards);
    }

    [Fact]
    public void StartGame_ThrowsWithFewerThanTwoPlayers()
    {
        // Arrange
        var room = new GameRoom();
        room.AddPlayer(new Player { Name = "Alice" });

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => room.StartGame());
        Assert.Contains("Cannot start game", exception.Message);
        Assert.Contains("at least 2 players", exception.Message);
    }

    [Fact]
    public void DrawCard_ReturnsCardAndAdvancesTurn()
    {
        // Arrange
        var room = new GameRoom();
        room.AddPlayer(new Player { Name = "Alice" });
        room.AddPlayer(new Player { Name = "Bob" });
        room.StartGame();

        // Act
        var card1 = room.DrawCard();
        var playerAfterFirstDraw = room.CurrentPlayerIndex;
        var card2 = room.DrawCard();
        var playerAfterSecondDraw = room.CurrentPlayerIndex;

        // Assert
        Assert.NotNull(card1);
        Assert.NotNull(card2);
        Assert.Equal(1, playerAfterFirstDraw);
        Assert.Equal(0, playerAfterSecondDraw);
    }

    [Fact]
    public void DrawCard_ThrowsIfNotInPlayingState()
    {
        // Arrange
        var room = new GameRoom();
        room.AddPlayer(new Player { Name = "Alice" });
        room.AddPlayer(new Player { Name = "Bob" });

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => room.DrawCard());
        Assert.Contains("Game is not in playing state", exception.Message);
    }

    [Fact]
    public void TurnAdvances_CyclicallyThroughPlayers()
    {
        // Arrange
        var room = new GameRoom();
        room.AddPlayer(new Player { Name = "Alice" });
        room.AddPlayer(new Player { Name = "Bob" });
        room.AddPlayer(new Player { Name = "Charlie" });
        room.StartGame();

        // Act & Assert
        Assert.Equal(0, room.CurrentPlayerIndex);
        room.DrawCard();
        Assert.Equal(1, room.CurrentPlayerIndex);
        room.DrawCard();
        Assert.Equal(2, room.CurrentPlayerIndex);
        room.DrawCard();
        Assert.Equal(0, room.CurrentPlayerIndex);
    }

    [Fact]
    public void DrawingFourthKing_EndsGame()
    {
        // Arrange
        var room = new GameRoom();
        room.AddPlayer(new Player { Name = "Alice" });
        room.AddPlayer(new Player { Name = "Bob" });
        room.StartGame();

        // Act - Draw cards until we get 4 kings
        int kingsDrawn = 0;
        while (kingsDrawn < 4 && !room.Deck.IsEmpty)
        {
            var card = room.DrawCard();
            if (card?.Rank == Rank.King)
            {
                kingsDrawn++;
            }
        }

        // Assert
        Assert.Equal(4, room.KingsDrawn);
        Assert.Equal(GameState.Finished, room.State);
        Assert.NotNull(room.FinishedAt);
    }

    [Fact]
    public void RemovePlayer_RemovesByPlayerId()
    {
        // Arrange
        var room = new GameRoom();
        var player1 = new Player { Name = "Alice" };
        var player2 = new Player { Name = "Bob" };
        room.AddPlayer(player1);
        room.AddPlayer(player2);

        // Act
        room.RemovePlayer(player1.Id);

        // Assert
        Assert.DoesNotContain(player1, room.Players);
        Assert.Contains(player2, room.Players);
        Assert.Single(room.Players);
    }

    [Fact]
    public void RemovePlayer_EndsGameIfFewerThanTwoPlayersWhilePlaying()
    {
        // Arrange
        var room = new GameRoom();
        var player1 = new Player { Name = "Alice" };
        var player2 = new Player { Name = "Bob" };
        room.AddPlayer(player1);
        room.AddPlayer(player2);
        room.StartGame();

        // Act
        room.RemovePlayer(player1.Id);

        // Assert
        Assert.Equal(GameState.Finished, room.State);
        Assert.NotNull(room.FinishedAt);
    }

    [Fact]
    public void RemovePlayer_DoesNotEndGameIfInLobby()
    {
        // Arrange
        var room = new GameRoom();
        var player1 = new Player { Name = "Alice" };
        var player2 = new Player { Name = "Bob" };
        room.AddPlayer(player1);
        room.AddPlayer(player2);

        // Act
        room.RemovePlayer(player1.Id);

        // Assert
        Assert.Equal(GameState.Lobby, room.State);
    }

    [Fact]
    public void CurrentPlayer_ReturnsCorrectPlayerBasedOnIndex()
    {
        // Arrange
        var room = new GameRoom();
        var player1 = new Player { Name = "Alice" };
        var player2 = new Player { Name = "Bob" };
        var player3 = new Player { Name = "Charlie" };
        room.AddPlayer(player1);
        room.AddPlayer(player2);
        room.AddPlayer(player3);
        room.StartGame();

        // Act & Assert
        Assert.Equal(player1, room.CurrentPlayer);
        room.DrawCard();
        Assert.Equal(player2, room.CurrentPlayer);
        room.DrawCard();
        Assert.Equal(player3, room.CurrentPlayer);
        room.DrawCard();
        Assert.Equal(player1, room.CurrentPlayer);
    }

    [Fact]
    public void CurrentPlayer_ReturnsNullWhenNoPlayers()
    {
        // Arrange
        var room = new GameRoom();

        // Act & Assert
        Assert.Null(room.CurrentPlayer);
    }

    [Fact]
    public void CurrentPlayer_ReturnsNullWhenIndexOutOfBounds()
    {
        // Arrange
        var room = new GameRoom();
        room.AddPlayer(new Player { Name = "Alice" });
        room.AddPlayer(new Player { Name = "Bob" });

        // Act
        room.CurrentPlayerIndex = 5;

        // Assert
        Assert.Null(room.CurrentPlayer);
    }

    [Fact]
    public void NewGameRoom_SetsCreatedAt()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var room = new GameRoom();

        // Assert
        var after = DateTime.UtcNow;
        Assert.InRange(room.CreatedAt, before.AddSeconds(-1), after.AddSeconds(1));
    }

    [Fact]
    public void StartGame_SetsStartedAt()
    {
        // Arrange
        var room = new GameRoom();
        room.AddPlayer(new Player { Name = "Alice" });
        room.AddPlayer(new Player { Name = "Bob" });
        var before = DateTime.UtcNow;

        // Act
        room.StartGame();

        // Assert
        var after = DateTime.UtcNow;
        Assert.NotNull(room.StartedAt);
        Assert.InRange(room.StartedAt.Value, before.AddSeconds(-1), after.AddSeconds(1));
    }

    [Fact]
    public void CurrentCard_IsSetAfterDrawing()
    {
        // Arrange
        var room = new GameRoom();
        room.AddPlayer(new Player { Name = "Alice" });
        room.AddPlayer(new Player { Name = "Bob" });
        room.StartGame();

        // Act
        var drawnCard = room.DrawCard();

        // Assert
        Assert.Equal(drawnCard, room.CurrentCard);
    }

    [Fact]
    public void KingsDrawn_IncrementsWhenKingIsDrawn()
    {
        // Arrange
        var room = new GameRoom();
        room.AddPlayer(new Player { Name = "Alice" });
        room.AddPlayer(new Player { Name = "Bob" });
        room.StartGame();

        // Act - Draw until we find at least one king
        int kingsFound = 0;
        while (kingsFound < 1 && !room.Deck.IsEmpty)
        {
            var card = room.DrawCard();
            if (card?.Rank == Rank.King)
            {
                kingsFound++;
            }
        }

        // Assert
        Assert.True(room.KingsDrawn >= 1);
    }
}
