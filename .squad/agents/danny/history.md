# Project Context

- **Owner:** Huw Parry
- **Project:** KingsCup — Web-based Kings Cup drinking card game
- **Stack:** .NET 10, Blazor, SignalR, C#
- **Description:** Players browse to the game, join via shareable code or start a new game. Standard Kings Cup rules apply when drawing playing cards from a virtual deck.
- **Created:** 2026-03-09

## Learnings

### 2026-03-09 - Initial Architecture Design

**Decision: Blazor Web App with Server Interactivity + SignalR**
- Chose Blazor Web App (not WebAssembly) with interactive server rendering for real-time multiplayer
- SignalR hub provides WebSocket-based communication for instant game state updates
- Server-authoritative model prevents client-side cheating and simplifies state sync

**Solution Structure:**
- `KingsCup.Core` - Game models (Card, Deck, Player, GameRoom, KingsCupRule)
- `KingsCup.Web` - Blazor app + SignalR GameHub + IGameManager service
- `KingsCup.Tests` - xUnit tests

**Key Architectural Patterns:**
- GameRoom is the aggregate root - all game state mutations go through it
- In-memory game storage with `ConcurrentDictionary` (ephemeral sessions, no DB needed)
- 6-character room codes (alphanumeric, case-insensitive, excludes confusing chars)
- Static `KingsCupRules` class maps Rank enum to rule name/description

**File Paths:**
- Core models: `src/KingsCup.Core/Models/*.cs`
- SignalR hub: `src/KingsCup.Web/Hubs/GameHub.cs`
- Game manager: `src/KingsCup.Web/Services/IGameManager.cs`
- Architecture doc: `.squad/decisions.md` (merged from inbox)

**Kings Cup Rules Implemented:**
- All 13 ranks mapped (Ace=Waterfall, 2=You, 3=Me, 4=Floor, 5=Guys, 6=Chicks, 7=Heaven, 8=Mate, 9=Rhyme, 10=Categories, Jack=Make a Rule, Queen=Questions, King=King's Cup)
- Game ends when 4th King is drawn

**User Preferences:**
- Huw wants .NET 10 with modern Blazor (not legacy templates)
- Prefers real-time multiplayer over turn-based REST API
- Standard drinking game rules, no custom rule variants for MVP

### 2026-03-09 - Team Integration

**From Linus (Hub Implementation):**
- GameHub fully implemented with connection tracking and turn validation
- Hub methods ready for client consumption
- All callbacks typed via IGameClient interface
- Server-authoritative game state enforced

**From Rusty (Frontend):**
- Complete Blazor UI built and connected to hub
- Mobile-first responsive design with card table aesthetic
- All SignalR callbacks integrated
- Game flow: Home → Lobby → Playing → Finished

**From Basher (Testing):**
- 62 xUnit tests written, 100% passing
- All core models validated
- Test patterns established for team
- Game state machine thoroughly tested

<!-- Append new learnings below. Each entry is something lasting about the project. -->
