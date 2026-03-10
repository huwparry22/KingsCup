### 2026-03-10T00:00:00Z: Preserve authoritative room state across joins and game start
**By:** Huw Parry (via Copilot / Linus)
**What:** Broadcast `RoomStateChanged` to each connected player after create, join, leave, and start transitions so every client gets the current room state with its own `selfPlayerId`.
**Why:** The room state already changes correctly on the server, but clients could remain in a stale lobby view or lose their local player identity when only partial events were sent.