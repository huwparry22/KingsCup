# Project Context

- **Owner:** Huw Parry
- **Project:** KingsCup — Web-based Kings Cup drinking card game
- **Stack:** .NET 10, Blazor, SignalR, C#
- **Description:** Players browse to the game, join via shareable code or start a new game. Standard Kings Cup rules apply when drawing playing cards from a virtual deck.
- **Created:** 2026-03-09

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-03-09 - Comprehensive Test Coverage for Core Models
- **Test Files Created:** CardTests.cs, DeckTests.cs, GameRoomTests.cs, PlayerTests.cs, KingsCupRulesTests.cs
- **Total Tests:** 62 tests, all passing
- **Key Test Patterns:**
  - Deck shuffle validation uses probabilistic comparison (first 3 cards different after shuffle)
  - Game room code generation validates character set (excludes I, O, 0, 1)
  - Turn advancement tested cyclically through 3 players to verify modulo logic
  - 4th King game-ending scenario validated by drawing until condition met
  - Case-insensitive player name collision tested
- **Coverage Areas:**
  - Card: ToString formatting, record equality
  - Deck: 52-card initialization, draw mechanics, shuffle behavior, reset functionality
  - GameRoom: Room code generation, player management, game state transitions, turn advancement, king tracking
  - Player: ID generation, required fields, defaults, timestamps
  - KingsCupRules: All 13 ranks mapped, non-empty names/descriptions, spot-checked key rules
- **Testing Strategy:** Arrange-Act-Assert pattern throughout, descriptive test names explaining scenarios, edge cases covered (empty deck, invalid states, boundary conditions)

### 2026-03-09 — Team Context (from orchestration)

**From Danny (Architecture):**
- Designed complete solution with domain-driven design principles
- GameRoom as aggregate root with validation in model
- In-memory ephemeral game storage with ConcurrentDictionary
- 6-character room codes with no collision risk at party scale
- All 13 Kings Cup rules mapped in static lookup class

**From Linus (Hub Implementation):**
- Full GameHub implemented with connection tracking
- Turn validation enforces server-authoritative state
- SignalR groups for efficient room broadcasts
- Proper disconnection handling with IsConnected flag
- Build passes clean with all integration working

**From Rusty (Frontend):**
- Complete Blazor Web App built and deployed
- Home page with create/join forms
- Game page with InteractiveServer and SignalR integration
- CardDisplay and PlayerList components
- Mobile-first responsive CSS with dark green theme

---
