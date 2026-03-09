# Project Context

- **Owner:** Huw Parry
- **Project:** KingsCup — Web-based Kings Cup drinking card game
- **Stack:** .NET 10, Blazor, SignalR, C#
- **Description:** Players browse to the game, join via shareable code or start a new game. Standard Kings Cup rules apply when drawing playing cards from a virtual deck.
- **Created:** 2026-03-09

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-03-09 — GameHub Implementation

- **Implemented:** Full SignalR GameHub with typed client callbacks (IGameClient)
- **Added:** Connection tracking via static ConcurrentDictionary to map ConnectionId → (RoomCode, PlayerId)
- **Configured:** Program.cs with SignalR services, IGameManager singleton, and hub endpoint at `/gamehub`
- **Validated:** Turn ownership in DrawCard to enforce server-authoritative game state
- **Handled:** Disconnections by marking IsConnected=false; full removal on LeaveGame
- **Key insight:** Hub methods are stateless — connection tracking is essential for multi-method workflows (join → draw → leave)
- **Build status:** Clean build, all components integrated correctly

### 2026-03-09 — Team Context (from orchestration)

**From Danny (Architecture):**
- Designed complete Blazor+SignalR architecture with GameRoom as aggregate root
- In-memory storage via ConcurrentDictionary (ephemeral sessions)
- 6-character alphanumeric room codes (excludes confusing chars)
- All Kings Cup rules mapped (13 ranks with rule names/descriptions)
- Server-authoritative game state prevents cheating

**From Rusty (Frontend):**
- Built complete Blazor Web App with SignalR integration
- Home page with create/join forms; Game page with InteractiveServer mode
- Mobile-first responsive CSS with dark green card table aesthetic
- CardDisplay and PlayerList components for reusability
- Removed template defaults, clean app-specific UI

**From Basher (Testing):**
- 62 xUnit tests written, all passing
- GameRoom, Deck, Player, Card, KingsCupRules all thoroughly tested
- Game state machine validated (Lobby → Playing → Finished)
- Test patterns established (AAA, descriptive names, edge cases)

---
