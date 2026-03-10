using System.Security.Claims;
using KingsCup.Core.Models;
using KingsCup.Web.Hubs;
using KingsCup.Web.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

namespace KingsCup.Tests;

public class GameHubTests
{
    [Fact]
    public async Task StartGame_StartsSuccessfullyAfterSecondPlayerJoins()
    {
        // Arrange
        var gameManager = new InMemoryGameManager();
        var hostClient = new RecordingGameClient();
        var guestClient = new RecordingGameClient();
        var allClients = new Dictionary<string, RecordingGameClient>
        {
            ["host-connection"] = hostClient,
            ["guest-connection"] = guestClient
        };

        var hostHub = CreateHub(gameManager, "host-connection", allClients);
        var guestHub = CreateHub(gameManager, "guest-connection", allClients);

        // Act
        var roomCode = await hostHub.CreateGame("Alice");
        await guestHub.JoinGame(roomCode, "Bob");
        await hostHub.StartGame(roomCode);

        // Assert
        Assert.True(gameManager.TryGetGame(roomCode, out var room));
        Assert.NotNull(room);
        Assert.Equal(GameState.Playing, room!.State);
        Assert.True(room.CanStart is false);
        Assert.Equal(2, room.Players.Count);
        Assert.Equal("Alice", room.CurrentPlayer?.Name);
        Assert.Contains(hostClient.GameStartedCalls, call => call.RoomCode == roomCode && call.PlayerNames.SequenceEqual(["Alice", "Bob"]));
        Assert.Contains(guestClient.GameStartedCalls, call => call.RoomCode == roomCode && call.PlayerNames.SequenceEqual(["Alice", "Bob"]));
    }

    [Fact]
    public async Task JoinGame_AddsSecondPlayerAndNotifiesExistingPlayers()
    {
        // Arrange
        var gameManager = new InMemoryGameManager();
        var hostClient = new RecordingGameClient();
        var guestClient = new RecordingGameClient();
        var allClients = new Dictionary<string, RecordingGameClient>
        {
            ["host-connection"] = hostClient,
            ["guest-connection"] = guestClient
        };

        var hostHub = CreateHub(gameManager, "host-connection", allClients);
        var guestHub = CreateHub(gameManager, "guest-connection", allClients);

        var roomCode = await hostHub.CreateGame("Alice");

        // Act
        await guestHub.JoinGame(roomCode, "Bob");

        // Assert
        Assert.True(gameManager.TryGetGame(roomCode, out var room));
        Assert.NotNull(room);
        Assert.Equal(2, room!.Players.Count);
        Assert.True(room.CanStart);
        Assert.Contains(hostClient.PlayerJoinedCalls, call => call.PlayerName == "Bob");
        Assert.Equal("Bob", room.Players.Single(player => player.Id == guestClient.LastRoomState!.SelfPlayerId).Name);
        Assert.Equal(2, guestClient.LastRoomState!.PlayerList.Count);
    }

    [Fact]
    public async Task StartGame_ThrowsWhenOnlyOnePlayerIsPresent()
    {
        // Arrange
        var gameManager = new InMemoryGameManager();
        var hostClient = new RecordingGameClient();
        var allClients = new Dictionary<string, RecordingGameClient>
        {
            ["host-connection"] = hostClient
        };
        var hostHub = CreateHub(gameManager, "host-connection", allClients);
        var roomCode = await hostHub.CreateGame("Alice");

        // Act
        var exception = await Assert.ThrowsAsync<HubException>(() => hostHub.StartGame(roomCode));

        // Assert
        Assert.Equal("Need at least 2 players to start.", exception.Message);
    }

    private static GameHub CreateHub(IGameManager gameManager, string connectionId, Dictionary<string, RecordingGameClient> clients)
    {
        return new GameHub(gameManager)
        {
            Context = new TestHubCallerContext(connectionId),
            Clients = new TestHubCallerClients(connectionId, clients),
            Groups = new TestGroupManager()
        };
    }

    private sealed class RecordingGameClient : IGameClient
    {
        public List<(string PlayerId, string PlayerName)> PlayerJoinedCalls { get; } = new();
        public List<(string RoomCode, List<string> PlayerNames)> GameStartedCalls { get; } = new();
        public RoomStateSnapshot? LastRoomState { get; private set; }

        public Task PlayerJoined(string playerId, string playerName)
        {
            PlayerJoinedCalls.Add((playerId, playerName));
            return Task.CompletedTask;
        }

        public Task PlayerLeft(string playerId, string playerName) => Task.CompletedTask;

        public Task PlayerDisconnected(string playerId, string playerName) => Task.CompletedTask;

        public Task GameStarted(string roomCode, List<string> playerNames)
        {
            GameStartedCalls.Add((roomCode, playerNames));
            return Task.CompletedTask;
        }

        public Task CardDrawn(string playerId, string playerName, Card card, KingsCupRule rule, int cardsRemaining) => Task.CompletedTask;

        public Task GameOver(string winnerName, string reason) => Task.CompletedTask;

        public Task TurnChanged(string currentPlayerId, string currentPlayerName) => Task.CompletedTask;

        public Task RoomStateChanged(string selfPlayerId, List<Player> playerList, GameState state, string? currentTurnPlayerId)
        {
            LastRoomState = new RoomStateSnapshot(selfPlayerId, playerList, state, currentTurnPlayerId);
            return Task.CompletedTask;
        }
    }

    private sealed record RoomStateSnapshot(string SelfPlayerId, List<Player> PlayerList, GameState State, string? CurrentTurnPlayerId);

    private sealed class TestHubCallerClients : IHubCallerClients<IGameClient>
    {
        private readonly string _callerConnectionId;
        private readonly Dictionary<string, RecordingGameClient> _clients;

        public TestHubCallerClients(string callerConnectionId, Dictionary<string, RecordingGameClient> clients)
        {
            _callerConnectionId = callerConnectionId;
            _clients = clients;
        }

        public IGameClient All => new CompositeGameClient(_clients.Values);

        public IGameClient Caller => _clients[_callerConnectionId];

        public IGameClient Others => new CompositeGameClient(_clients.Where(kvp => kvp.Key != _callerConnectionId).Select(kvp => kvp.Value));

        public IGameClient AllExcept(IReadOnlyList<string> excludedConnectionIds) => new CompositeGameClient(_clients.Where(kvp => !excludedConnectionIds.Contains(kvp.Key)).Select(kvp => kvp.Value));

        public IGameClient Client(string connectionId) => _clients[connectionId];

        public IGameClient Clients(IReadOnlyList<string> connectionIds) => new CompositeGameClient(connectionIds.Select(id => _clients[id]));

        public IGameClient Group(string groupName) => new CompositeGameClient(_clients.Values);

        public IGameClient GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => new CompositeGameClient(_clients.Where(kvp => !excludedConnectionIds.Contains(kvp.Key)).Select(kvp => kvp.Value));

        public IGameClient Groups(IReadOnlyList<string> groupNames) => new CompositeGameClient(_clients.Values);

        public IGameClient OthersInGroup(string groupName) => new CompositeGameClient(_clients.Where(kvp => kvp.Key != _callerConnectionId).Select(kvp => kvp.Value));

        public IGameClient User(string userId) => throw new NotSupportedException();

        public IGameClient Users(IReadOnlyList<string> userIds) => throw new NotSupportedException();
    }

    private sealed class CompositeGameClient : IGameClient
    {
        private readonly IReadOnlyCollection<RecordingGameClient> _clients;

        public CompositeGameClient(IEnumerable<RecordingGameClient> clients)
        {
            _clients = clients.ToList();
        }

        public Task PlayerJoined(string playerId, string playerName) => Broadcast(client => client.PlayerJoined(playerId, playerName));

        public Task PlayerLeft(string playerId, string playerName) => Broadcast(client => client.PlayerLeft(playerId, playerName));

        public Task PlayerDisconnected(string playerId, string playerName) => Broadcast(client => client.PlayerDisconnected(playerId, playerName));

        public Task GameStarted(string roomCode, List<string> playerNames) => Broadcast(client => client.GameStarted(roomCode, playerNames));

        public Task CardDrawn(string playerId, string playerName, Card card, KingsCupRule rule, int cardsRemaining) => Broadcast(client => client.CardDrawn(playerId, playerName, card, rule, cardsRemaining));

        public Task GameOver(string winnerName, string reason) => Broadcast(client => client.GameOver(winnerName, reason));

        public Task TurnChanged(string currentPlayerId, string currentPlayerName) => Broadcast(client => client.TurnChanged(currentPlayerId, currentPlayerName));

        public Task RoomStateChanged(string selfPlayerId, List<Player> playerList, GameState state, string? currentTurnPlayerId) => Broadcast(client => client.RoomStateChanged(selfPlayerId, playerList, state, currentTurnPlayerId));

        private Task Broadcast(Func<RecordingGameClient, Task> callback) => Task.WhenAll(_clients.Select(callback));
    }

    private sealed class TestGroupManager : IGroupManager
    {
        public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class TestHubCallerContext : HubCallerContext
    {
        private readonly IDictionary<object, object?> _items = new Dictionary<object, object?>();

        public TestHubCallerContext(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public override string ConnectionId { get; }

        public override string? UserIdentifier => null;

        public override ClaimsPrincipal? User => null;

        public override IDictionary<object, object?> Items => _items;

        public override IFeatureCollection Features => throw new NotSupportedException();

        public override CancellationToken ConnectionAborted => CancellationToken.None;

        public override void Abort()
        {
        }
    }
}