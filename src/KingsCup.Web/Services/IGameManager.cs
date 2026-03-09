using KingsCup.Core.Models;
using System.Collections.Concurrent;

namespace KingsCup.Web.Services;

public interface IGameManager
{
    GameRoom CreateGame();
    GameRoom? GetGame(string roomCode);
    bool TryGetGame(string roomCode, out GameRoom? game);
    void RemoveGame(string roomCode);
    IEnumerable<GameRoom> GetAllGames();
}

public class InMemoryGameManager : IGameManager
{
    private readonly ConcurrentDictionary<string, GameRoom> _games = new();

    public GameRoom CreateGame()
    {
        var game = new GameRoom();
        _games.TryAdd(game.RoomCode, game);
        return game;
    }

    public GameRoom? GetGame(string roomCode)
    {
        _games.TryGetValue(roomCode.ToUpperInvariant(), out var game);
        return game;
    }

    public bool TryGetGame(string roomCode, out GameRoom? game)
    {
        return _games.TryGetValue(roomCode.ToUpperInvariant(), out game);
    }

    public void RemoveGame(string roomCode)
    {
        _games.TryRemove(roomCode.ToUpperInvariant(), out _);
    }

    public IEnumerable<GameRoom> GetAllGames()
    {
        return _games.Values;
    }
}
