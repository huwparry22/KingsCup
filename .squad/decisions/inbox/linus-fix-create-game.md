# Fix: Player Name Already Taken Exception on Game Creation

**Date:** 2025-03-09
**Author:** Linus (Backend Dev)
**Status:** Implemented

## Problem

When creating a new game, users encountered "Player name already taken" exception. Root cause:

1. User navigates to `/game/new?playerName=Alice`
2. `OnInitializedAsync` calls `CreateGame("Alice")` → adds player to room
3. Then navigates to `/game/ABCD12?playerName=Alice` (with real room code)
4. Navigation destroys component and connection, creates new component
5. New `OnInitializedAsync` calls `JoinGame("ABCD12", "Alice")` → tries to add same player again → BOOM

## Solution

Fixed on both client and server sides:

### 1. Client Side (Game.razor)
- **Removed navigation after CreateGame** (line 172)
- Instead, just update `RoomCode` locally and set `isConnecting = false`
- Let the hub's `RoomStateChanged` callback update the UI with full state
- This prevents the component destruction/recreation cycle

### 2. Server Side (GameHub.cs)

#### A. Fixed RoomStateChanged Signature Mismatch
- **Before:** `RoomStateChanged(GameState state, int playerCount)`
- **After:** `RoomStateChanged(string selfPlayerId, List<Player> playerList, GameState state, string? currentTurnPlayerId)`
- Client was expecting the full signature but hub was sending limited data

#### B. CreateGame Now Sends Full State
- After creating game and adding player, immediately send `RoomStateChanged` to caller
- This provides the creator with their player ID, full player list, game state, and current turn player
- Eliminates need for re-join

#### C. JoinGame Handles Reconnection
- Check if player name exists AND is disconnected
- If so, update their connection ID instead of throwing exception
- This handles genuine reconnection scenarios gracefully
- Only throw "name taken" if player is currently connected

#### D. Updated LeaveGame State Broadcasting
- Send individualized `RoomStateChanged` to each connected player with their own `selfPlayerId`
- Ensures each client has correct state after a player leaves

## Key Technical Decisions

1. **Why not just fix AddPlayer in GameRoom?**
   - AddPlayer should remain strict - it's a domain model method
   - The reconnection logic belongs in the Hub (application layer)
   - Keeps domain logic clean and validation clear

2. **Why send full state instead of minimal updates?**
   - SignalR connections can be unreliable
   - Full state ensures clients can recover from missed messages
   - Simpler client-side state management
   - List of players is small (typically < 10), not a performance concern

3. **Why update RoomCode locally instead of using URL?**
   - Avoids component destruction cycle
   - Maintains connection stability
   - URL update not necessary - room code displayed in UI
   - Could add URL update with `Navigation.NavigateTo(..., replace: true, forceLoad: false)` later if needed for bookmarking

## Testing Notes

Build verified successful. To test manually:
1. Navigate to home page
2. Enter player name
3. Click "Create Game"
4. Verify no exception occurs
5. Verify lobby displays with correct room code
6. Verify player appears in player list

## Files Modified

- `src/KingsCup.Web/Components/Pages/Game.razor` - Removed navigation after CreateGame
- `src/KingsCup.Web/Hubs/GameHub.cs` - Fixed RoomStateChanged signature, added state broadcasting, added reconnection handling
