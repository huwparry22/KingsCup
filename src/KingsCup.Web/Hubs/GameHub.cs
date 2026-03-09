using Microsoft.AspNetCore.SignalR;
using KingsCup.Core.Models;
using KingsCup.Web.Services;
using System.Collections.Concurrent;

namespace KingsCup.Web.Hubs;

public class GameHub : Hub<IGameClient>
{
    private readonly IGameManager _gameManager;
    private static readonly ConcurrentDictionary<string, (string RoomCode, string PlayerId)> _connections = new();

    public GameHub(IGameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public async Task<string> CreateGame(string playerName)
    {
        var room = _gameManager.CreateGame();
        var player = new Player { Name = playerName, ConnectionId = Context.ConnectionId };
        
        room.AddPlayer(player);
        _connections[Context.ConnectionId] = (room.RoomCode, player.Id);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomCode);
        
        return room.RoomCode;
    }

    public async Task JoinGame(string roomCode, string playerName)
    {
        if (!_gameManager.TryGetGame(roomCode, out var room) || room == null)
            throw new HubException("Game not found.");

        if (room.State != GameState.Lobby)
            throw new HubException("Game has already started.");

        var player = new Player { Name = playerName, ConnectionId = Context.ConnectionId };
        
        room.AddPlayer(player);
        _connections[Context.ConnectionId] = (room.RoomCode, player.Id);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomCode);
        
        await Clients.Group(room.RoomCode).PlayerJoined(player.Id, player.Name);
        await Clients.Group(room.RoomCode).RoomStateChanged(room.State, room.Players.Count);
    }

    public async Task StartGame(string roomCode)
    {
        if (!_gameManager.TryGetGame(roomCode, out var room) || room == null)
            throw new HubException("Game not found.");

        if (!room.CanStart)
            throw new HubException("Need at least 2 players to start.");

        room.StartGame();
        
        var playerNames = room.Players.Select(p => p.Name).ToList();
        await Clients.Group(room.RoomCode).GameStarted(room.RoomCode, playerNames);
        
        var currentPlayer = room.CurrentPlayer;
        if (currentPlayer != null)
        {
            await Clients.Group(room.RoomCode).TurnChanged(currentPlayer.Id, currentPlayer.Name);
        }
    }

    public async Task DrawCard(string roomCode)
    {
        if (!_gameManager.TryGetGame(roomCode, out var room) || room == null)
            throw new HubException("Game not found.");

        if (room.State != GameState.Playing)
            throw new HubException("Game is not in progress.");

        if (!_connections.TryGetValue(Context.ConnectionId, out var connectionInfo))
            throw new HubException("Player not found in room.");

        var currentPlayer = room.CurrentPlayer;
        if (currentPlayer == null || currentPlayer.Id != connectionInfo.PlayerId)
            throw new HubException("It's not your turn.");

        var card = room.DrawCard();
        if (card == null)
        {
            await Clients.Group(room.RoomCode).GameOver("No one", "Deck is empty.");
            return;
        }

        var rule = KingsCupRules.GetRule(card.Rank);
        var cardsRemaining = room.Deck.RemainingCards;
        
        await Clients.Group(room.RoomCode).CardDrawn(
            currentPlayer.Id, 
            currentPlayer.Name, 
            card, 
            rule, 
            cardsRemaining
        );

        if (room.State == GameState.Finished)
        {
            await Clients.Group(room.RoomCode).GameOver(currentPlayer.Name, "4th King drawn!");
        }
        else
        {
            var nextPlayer = room.CurrentPlayer;
            if (nextPlayer != null)
            {
                await Clients.Group(room.RoomCode).TurnChanged(nextPlayer.Id, nextPlayer.Name);
            }
        }
    }

    public async Task LeaveGame(string roomCode)
    {
        if (!_gameManager.TryGetGame(roomCode, out var room) || room == null)
            throw new HubException("Game not found.");

        if (!_connections.TryGetValue(Context.ConnectionId, out var connectionInfo))
            throw new HubException("Player not found in room.");

        var player = room.Players.FirstOrDefault(p => p.Id == connectionInfo.PlayerId);
        if (player == null)
            return;

        room.RemovePlayer(player.Id);
        _connections.TryRemove(Context.ConnectionId, out _);
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.RoomCode);
        
        await Clients.Group(room.RoomCode).PlayerLeft(player.Id, player.Name);
        
        if (room.State == GameState.Finished)
        {
            await Clients.Group(room.RoomCode).GameOver("Game ended", "Not enough players.");
            _gameManager.RemoveGame(room.RoomCode);
        }
        else
        {
            await Clients.Group(room.RoomCode).RoomStateChanged(room.State, room.Players.Count);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_connections.TryGetValue(Context.ConnectionId, out var connectionInfo))
        {
            if (_gameManager.TryGetGame(connectionInfo.RoomCode, out var room) && room != null)
            {
                var player = room.Players.FirstOrDefault(p => p.Id == connectionInfo.PlayerId);
                if (player != null)
                {
                    player.IsConnected = false;
                    await Clients.Group(connectionInfo.RoomCode).PlayerDisconnected(player.Id, player.Name);
                }
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}

public interface IGameClient
{
    Task PlayerJoined(string playerId, string playerName);
    Task PlayerLeft(string playerId, string playerName);
    Task PlayerDisconnected(string playerId, string playerName);
    Task GameStarted(string roomCode, List<string> playerNames);
    Task CardDrawn(string playerId, string playerName, Card card, KingsCupRule rule, int cardsRemaining);
    Task GameOver(string winnerName, string reason);
    Task TurnChanged(string currentPlayerId, string currentPlayerName);
    Task RoomStateChanged(GameState state, int playerCount);
}
