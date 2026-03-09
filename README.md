# Kings Cup - Web Game

A multiplayer Kings Cup drinking card game built with .NET 10 and Blazor.

## Quick Start

```powershell
# Build the solution
dotnet build

# Run the web app
dotnet run --project src/KingsCup.Web

# Run tests
dotnet test
```

## Architecture

- **KingsCup.Web** - Blazor Web App with SignalR for real-time multiplayer
- **KingsCup.Core** - Game models and domain logic
- **KingsCup.Tests** - xUnit test suite

## Game Flow

1. **Create Game** - Player creates a new game and receives a 6-character room code
2. **Join Lobby** - Other players join using the room code
3. **Start Game** - Once 2+ players are ready, start the game (deck is shuffled)
4. **Draw Cards** - Players take turns drawing cards, each triggering a Kings Cup rule
5. **Game Over** - Game ends when the 4th King is drawn

## Kings Cup Rules

| Card | Rule | Description |
|------|------|-------------|
| Ace | Waterfall | Everyone drinks in order |
| 2 | You | Pick someone to drink |
| 3 | Me | You drink |
| 4 | Floor | Last to touch floor drinks |
| 5 | Guys | All guys drink |
| 6 | Chicks | All girls drink |
| 7 | Heaven | Last to point up drinks |
| 8 | Mate | Pick a drinking buddy |
| 9 | Rhyme | Rhyme words, first to fail drinks |
| 10 | Categories | Name things in a category |
| Jack | Make a Rule | Create a new rule |
| Queen | Questions | Question master mode |
| King | King's Cup | Pour drink in cup (4th ends game) |

## Tech Stack

- .NET 10
- Blazor Web App (Interactive Server)
- SignalR (WebSockets)
- xUnit (Testing)

## Architecture Decisions

See `.squad/decisions/inbox/danny-architecture.md` for detailed architectural decisions and rationale.

## Team

- **Danny** (Lead) - Architecture & code review
- **Rusty** (UI) - Blazor components
- **Linus** (Logic) - Game logic & hub implementation
- **Basher** (Testing) - Test coverage
