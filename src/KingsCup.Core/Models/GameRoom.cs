namespace KingsCup.Core.Models;

public class GameRoom
{
    public string RoomCode { get; init; } = GenerateRoomCode();
    public GameState State { get; set; } = GameState.Lobby;
    public List<Player> Players { get; init; } = new();
    public Deck Deck { get; init; } = new();
    public Card? CurrentCard { get; set; }
    public int CurrentPlayerIndex { get; set; } = 0;
    public int KingsDrawn { get; set; } = 0;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    public Player? CurrentPlayer => Players.Count > 0 && CurrentPlayerIndex < Players.Count
        ? Players[CurrentPlayerIndex]
        : null;

    public bool CanStart => State == GameState.Lobby && Players.Count >= 2;

    public void StartGame()
    {
        if (!CanStart)
            throw new InvalidOperationException("Cannot start game. Need at least 2 players.");

        State = GameState.Playing;
        StartedAt = DateTime.UtcNow;
        Deck.Reset();
        CurrentPlayerIndex = 0;
    }

    public Card? DrawCard()
    {
        if (State != GameState.Playing)
            throw new InvalidOperationException("Game is not in playing state.");

        var card = Deck.DrawCard();
        CurrentCard = card;

        if (card?.Rank == Rank.King)
        {
            KingsDrawn++;
            if (KingsDrawn == 4)
            {
                State = GameState.Finished;
                FinishedAt = DateTime.UtcNow;
            }
        }

        if (card != null && State == GameState.Playing)
        {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
        }

        return card;
    }

    public void AddPlayer(Player player)
    {
        if (State != GameState.Lobby)
            throw new InvalidOperationException("Cannot join game in progress.");

        if (Players.Any(p => p.Name.Equals(player.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Player name already taken.");

        Players.Add(player);
    }

    public void RemovePlayer(string playerId)
    {
        var player = Players.FirstOrDefault(p => p.Id == playerId);
        if (player != null)
        {
            Players.Remove(player);

            if (State == GameState.Playing && Players.Count < 2)
            {
                State = GameState.Finished;
                FinishedAt = DateTime.UtcNow;
            }
        }
    }

    private static string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Range(0, 6)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
    }
}
