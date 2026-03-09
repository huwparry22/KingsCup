# Project Context

- **Owner:** Huw Parry
- **Project:** KingsCup — Web-based Kings Cup drinking card game
- **Stack:** .NET 10, Blazor, SignalR, C#
- **Description:** Players browse to the game, join via shareable code or start a new game. Standard Kings Cup rules apply when drawing playing cards from a virtual deck.
- **Created:** 2026-03-09

## Learnings

### 2026-03-09: Blazor UI Implementation Complete
Built complete frontend UI for Kings Cup game:
- **Home Page** with forms for starting new game or joining existing game via room code
- **Game Page** with SignalR integration (`@rendermode InteractiveServer`) for real-time multiplayer
- **CardDisplay Component** showing playing cards with suit symbols (♥ ♦ ♣ ♠) and proper red/black coloring
- **PlayerList Component** showing all players with turn indicators and connection status
- **Mobile-first responsive design** with dark green card table aesthetic
- Removed default Blazor template pages (Counter, Weather) and simplified layout
- Added Microsoft.AspNetCore.SignalR.Client v10.0.0 package for client-side hub connections
- Game flow: Home → Lobby (shows room code) → Playing (draw cards, see rules) → Finished
- SignalR hub callbacks handle all game state updates: PlayerJoined, CardDrawn, TurnChanged, etc.

### 2026-03-09 — Team Context (from orchestration)

**From Danny (Architecture):**
- Designed complete solution with GameRoom as aggregate root
- In-memory storage strategy for ephemeral game sessions
- 6-character room codes with clear alphanumeric format
- All 13 Kings Cup rules implemented with rule lookup
- Server-authoritative game state model

**From Linus (Hub Implementation):**
- Full GameHub implemented with all game flow methods
- Connection tracking for stateless hub method calls
- Turn validation to prevent out-of-turn draws
- SignalR group management for efficient room broadcasts
- Disconnection handling with IsConnected flag

**From Basher (Testing):**
- 62 xUnit tests written, all passing
- Game models thoroughly tested and validated
- Game state machine tested end-to-end
- Test patterns available for new features

---