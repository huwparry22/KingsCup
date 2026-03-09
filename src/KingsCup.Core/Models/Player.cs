namespace KingsCup.Core.Models;

public class Player
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public string ConnectionId { get; set; } = string.Empty;
    public bool IsConnected { get; set; } = true;
    public DateTime JoinedAt { get; init; } = DateTime.UtcNow;
}
