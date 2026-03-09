# Squad Decisions

## Active Decisions

### 1. Blazor Web App with Interactive Server Rendering (Danny - 2026-03-09)

**Status:** Implemented

**Decision:** Use Blazor Web App (`.NET 10`) with interactive server-side rendering.

**Rationale:**
- Real-time requirements met via SignalR (built into Blazor Server)
- Simplified architecture without separate API layer
- Server maintains authoritative game state
- Low latency WebSocket connection for instant updates
- No client-side complexity; all game logic server-side

**Trade-offs:**
- Server memory scales with concurrent users (acceptable for party game)
- Requires persistent connection (acceptable for active sessions)
- Not offline-capable (not needed for multiplayer)

---

### 2. Solution Architecture with Three Projects (Danny - 2026-03-09)

**Status:** Implemented

**Decision:** Structure as `KingsCup.Core`, `KingsCup.Web`, and `KingsCup.Tests` projects.

**Rationale:**
- Core models separated for testability and reusability
- Single web project keeps deployment simple
- Clear separation: models (Core) vs presentation/communication (Web)

---

### 3. GameRoom as Aggregate Root (Danny - 2026-03-09)

**Status:** Implemented

**Decision:** GameRoom is the single source of truth for all game state mutations.

**Rationale:**
- All state changes go through GameRoom methods
- Validation enforced in domain model, not in hub/controller
- Prevents inconsistent state
- Immutable where possible (Card is a record)

---

### 4. In-Memory Game Storage with ConcurrentDictionary (Danny - 2026-03-09)

**Status:** Implemented

**Decision:** Use `ConcurrentDictionary<string, GameRoom>` as singleton for game storage.

**Rationale:**
- Games are ephemeral (30-minute sessions, no persistence needed)
- Thread-safe for concurrent access
- No database complexity
- Future: add room expiration/cleanup background service

---

### 5. 6-Character Alphanumeric Room Codes (Danny - 2026-03-09)

**Status:** Implemented

**Decision:** Room codes are uppercase, alphanumeric, excluding I, O, 0, 1 for clarity.

**Rationale:**
- ~2 billion combinations (collision risk negligible at party game scale)
- Easy to read/share verbally
- Case-insensitive matching for UX convenience

---

### 6. Connection Tracking via Static Dictionary (Linus - 2026-03-09)

**Status:** Implemented

**Decision:** Use static `ConcurrentDictionary<string, (string RoomCode, string PlayerId)>` to map ConnectionId → room/player.

**Rationale:**
- SignalR connections are stateless across method calls
- Fast lookup needed for proper cleanup on disconnect
- Enables turn validation (CompareCurrentPlayer to connection's PlayerId)

---

### 7. Turn Validation in DrawCard (Linus - 2026-03-09)

**Status:** Implemented

**Decision:** Validate turn ownership by comparing CurrentPlayer.Id with calling connection's PlayerId.

**Rationale:**
- Prevents clients from drawing out of turn
- Server is authoritative
- Connection tracking dictionary provides PlayerId for validation

---

### 8. SignalR Group-Based Broadcasting (Linus - 2026-03-09)

**Status:** Implemented

**Decision:** Add connections to SignalR groups named by RoomCode for efficient room broadcasts.

**Rationale:**
- Broadcasts only to players in the same room
- SignalR handles group membership cleanup on disconnect
- More efficient than manually tracking group membership

---

### 9. Disconnection Handling (Linus - 2026-03-09)

**Status:** Implemented

**Decision:** Mark player as IsConnected = false on disconnect; keep player in room. Explicit LeaveGame removes.

**Rationale:**
- Allows reconnection scenarios
- UI can show disconnected players
- Players must intentionally LeaveGame to be removed
- Future: Could add auto-removal after timeout

---

### 10. Blazor Component-First UI (Rusty - 2026-03-09)

**Status:** Implemented

**Decision:** Build reusable components (CardDisplay, PlayerList) with InteractiveServer on Game page.

**Rationale:**
- Component reusability across views
- InteractiveServer required for SignalR
- Static rendering on Home page (no interactivity needed)
- Mobile-first CSS with card table green aesthetic

---

### 11. Mobile-First Responsive Design (Rusty - 2026-03-09)

**Status:** Implemented

**Decision:** Design UI mobile-first with dark green felt background mimicking physical card table.

**Rationale:**
- Primary use case is phone browsers at social gatherings
- Responsive design scales to tablets/desktops
- Visual continuity with physical game

---

### 12. xUnit Test Suite with AAA Pattern (Basher - 2026-03-09)

**Status:** Implemented

**Decision:** Test all core models with xUnit using Arrange-Act-Assert pattern, one test file per model.

**Rationale:**
- Comprehensive coverage of happy paths, edge cases, error conditions
- Clear test patterns established for team
- Probabilistic shuffle validation avoids flaky tests
- Integration tests validate state machine (not just unit tests)

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
