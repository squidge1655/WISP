# Will-o'-the-Wisp - Script Documentation

## Overview
This document provides a high-level overview of all game scripts and how they work together.

---

## Core Game Flow

### Turn Sequence
```
1. Player presses movement key
   ↓
2. PlayerController.HandleInput() reads input
   ↓
3. PlayerController.AttemptMove() validates move:
   - Check if within bounds
   - Check if enemy blocks position
   ↓
4. If valid: PlayerController.SetPosition() updates position
   ↓
5. GameManager.ProcessTurn() executes turn sequence:
   a. Decrease mud trap counters
   b. Activate dormant enemies adjacent to player
   c. Move all active, non-trapped enemies toward player
   d. Resolve collisions (enemy merging)
   e. Remove enemies on goal square
   f. Check if player caught (LOSE)
   g. Check if all enemies eliminated (WIN)
   ↓
6. Turn sequence completes, player can move again
```

---

## Script Structure

### 1. GameManager.cs (Singleton)
**Purpose:** Central controller managing all game state and logic

**Key Responsibilities:**
- Track game state (Playing, Won, Lost, Paused)
- Manage player and enemies
- Store level layout (obstacles, mud, goal)
- Execute turn sequence
- Check win/lose conditions
- Handle collisions and merging

**Key Methods:**
- `ProcessTurn()` - Execute full turn sequence
- `IsValidPosition()` - Check if position in bounds
- `IsEnemyAtPosition()` - Check if enemy at position
- `SetGameState()` - Change game state

**Used By:** PlayerController, EnemyController, LevelManager

---

### 2. PlayerController.cs
**Purpose:** Handle player character input and movement

**Key Responsibilities:**
- Read keyboard input (arrow keys, WASD, QEZC)
- Validate moves (bounds, collision checks)
- Update player position
- Animate movement
- Trigger turn processing

**Control Scheme:**
- Arrow Keys / WASD - Cardinal directions
- QEZC - Diagonal directions
- R - Reset level

**Key Methods:**
- `HandleInput()` - Read keyboard input
- `AttemptMove()` - Validate and execute move
- `SetPosition()` - Update position and animate
- `AnimateMoveCoroutine()` - Smooth movement over time

**Used By:** GameManager

---

### 3. EnemyController.cs
**Purpose:** Implement individual enemy behavior and AI

**Key Responsibilities:**
- Implement pathfinding (move toward player)
- Handle dormant/active states
- Manage mud trap mechanics
- Detect player collision
- Handle elimination/purification
- Update visuals (color based on state)

**Enemy AI:**
- Always chase player (if active and not trapped)
- Use Manhattan distance pathfinding
- Only move cardinal directions (no diagonals)
- Avoid goal square (prefer moves away from goal when equidistant)
- Cannot move through trees or different-colored enemies

**Key Methods:**
- `MoveTowardPlayer()` - Move one square toward player
- `FindNextMoveTowardPlayer()` - Calculate optimal move
- `Awaken()` - Activate dormant enemy
- `Purify()` - Eliminate enemy
- `TrappedInMud()` - Immobilize for N turns

**Used By:** GameManager

---

### 4. GridHelper.cs (Static Utility)
**Purpose:** Provide coordinate conversion and grid utilities

**Key Concepts:**
- Grid coordinates: 0-4 for both X and Y (5x5 grid)
- World coordinates: -2 to 3 for both X and Y (visual positions)
- Cell size: 1 unit per cell

**Key Methods:**
- `GridToWorld()` - Convert grid to world coordinates
- `WorldToGrid()` - Convert world to grid coordinates
- `IsValidGridPosition()` - Check if in bounds
- `GetAdjacentPositions()` - Get neighboring cells
- `ManhattanDistance()` - Calculate grid distance

**Used By:** PlayerController, EnemyController, GameManager, LevelManager

---

### 5. LevelData.cs (ScriptableObject)
**Purpose:** Store configuration for individual levels

**Contains:**
- Player starting position
- Goal position
- Enemy spawns (position, type, dormancy)
- Obstacles (tree positions)
- Mud patches
- Level name/number
- Optimal move count

**Validation:**
- `ValidateLevel()` - Check all positions in bounds (Editor only)

**Used By:** LevelManager

---

### 6. LevelManager.cs
**Purpose:** Load, instantiate, and manage game levels

**Key Responsibilities:**
- Load levels from level list
- Instantiate player, enemies, obstacles, goal
- Setup GameManager with level configuration
- Handle level progression (next/previous)
- Clean up when changing levels

**Key Methods:**
- `LoadLevel()` - Load and instantiate a level
- `NextLevel()` - Progress to next level
- `PreviousLevel()` - Go to previous level
- `ReloadCurrentLevel()` - Restart current level
- `ClearLevel()` - Destroy all entities

**Used By:** UIManager, GameManager

---

### 7. UIManager.cs
**Purpose:** Display game UI and handle button interactions

**Features:**
- Display level name and number
- Display enemy counter
- Show win/lose screens
- Handle reset/next level buttons
- Display messages

**Key Methods:**
- `UpdateUI()` - Refresh UI display
- `OnResetClicked()` - Handle reset button
- `OnNextLevelClicked()` - Handle next level button
- `ShowWinScreen()` / `ShowLoseScreen()` - Display end states

**Used By:** LevelManager

---

### 8. GridRenderer.cs
**Purpose:** Visualize the 5x5 grid (Editor/Debug)

**Features:**
- Draw grid lines using Gizmos
- Highlight cells for debugging
- Visual guide for designers

**Used By:** Level design/debugging

---

## Data Flow Example

### When Player Moves Up:

```
PlayerController.Update()
  ↓
Input.GetKeyDown(KeyCode.UpArrow)
  ↓
HandleInput() → AttemptMove(Vector2Int.up)
  ↓
AttemptMove():
  - Calculate newPos = (2, 2)
  - Check GameManager.IsValidPosition(newPos) ✓
  - Check GameManager.IsEnemyAtPosition(newPos) ✓
  - SetPosition(newPos)
  - GameManager.ProcessTurn()
  ↓
ProcessTurn():
  1. Loop through enemies:
     - DecrementTrapCounter() for trapped enemies
  2. Check for dormant awakening:
     - For each enemy adjacent to new player pos
     - Call enemy.Awaken()
  3. Move active enemies:
     - For each enemy: MoveTowardPlayer(playerPos)
  4. ResolveCollisions():
     - Group enemies by position
     - Merge same-colored enemies
  5. PurifyEnemiesOnGoal():
     - Eliminate enemies on goal square
  6. Check defeat:
     - if (IsEnemyAtPosition(player.Position)) → Lost
  7. Check victory:
     - if (GetAllActiveEnemies().Count == 0) → Won
  ↓
Return to gameplay (player can move again)
```

---

## Key Algorithms

### Enemy Pathfinding (FindNextMoveTowardPlayer)

```
For each cardinal direction (up, down, left, right):
  1. Calculate next position in this direction
  2. Validate move:
     - In bounds?
     - No tree obstacle?
     - No other enemy blocking?
  3. Calculate Manhattan distance to player from next position
  4. If move gets closer to player:
     - If closest so far: choose this move
  5. Else if move keeps same distance to player:
     - Calculate distance to goal from next position
     - If further from goal: choose this move (avoidance)

Return best move found (or stay in place if stuck)
```

### Collision Resolution (ResolveCollisions)

```
Group all alive enemies by position:
  Dictionary<Vector2Int, List<EnemyController>>

For each group at same position:
  If group has multiple enemies:
    Get color of first enemy
    If ALL enemies same color:
      Eliminate all but first (they merge)
    Else:
      Do nothing (different colors block each other)
```

---

## Design Patterns Used

### 1. Singleton Pattern (GameManager)
- Ensures only one GameManager exists
- Accessible globally via `GameManager.Instance`
- Perfect for a central game controller

### 2. ScriptableObject (LevelData)
- Data-driven design
- Designers configure levels without coding
- Reusable level assets
- Easy to create level variations

### 3. State Machine (implicit in GameState enum)
- Clear state transitions
- Well-defined behaviors for each state
- Prevents invalid state combinations

### 4. Component Pattern (MonoBehaviour)
- Each script handles one responsibility
- Easy to modify individual systems
- Testable in isolation

### 5. Coroutines (Animation)
- Smooth movement over multiple frames
- Non-blocking animation
- Clean, readable async code

---

## Coordinate Systems

### Grid Coordinates
- Range: 0-4 for both X and Y
- Used for: Logic, pathfinding, collision
- Origin: Top-left (0,0)
- Type: Vector2Int

### World Coordinates
- Range: -2 to 3 for both X and Y
- Used for: Rendering, animations, visual positions
- Origin: Center (0,0)
- Type: Vector3
- Offset: (-2, -2, 0)

### Conversion
```
WorldPos = GridToWorld(GridPos)
GridPos = WorldToGrid(WorldPos)
```

---

## Enemy AI Summary

### Movement Priority
1. Move closer to player (if possible)
2. If equidistant, move away from goal (avoidance)
3. Cannot move through obstacles or different-colored enemies
4. Can only move cardinal directions (not diagonal)

### State Machine
```
Idle (dormant) ←→ Chasing ← Stuck (in mud)
     ↓
  Purified (on goal or merged)
```

---

## Common Modifications

### To Add New Terrain Type
1. Add grid check method in GameManager: `IsSomeTerrainType()`
2. Add to LevelData serialized list
3. Add validation in LevelData.ValidateLevel()
4. Add check in enemy pathfinding
5. Create visual prefab in LevelManager

### To Add New Enemy Behavior
1. Extend EnemyController or create subclass
2. Override MoveTowardPlayer() method
3. Implement new pathfinding logic
4. Update GameManager if new interaction needed

### To Add New Game State
1. Add to GameState enum in GameManager
2. Update SetGameState() if needed
3. Update UIManager to handle new state
4. Update relevant logic for new state

---

## Testing Checklist

- [ ] Player can move in all 8 directions
- [ ] Player cannot move out of bounds
- [ ] Player cannot move through enemies
- [ ] Enemies move toward player
- [ ] Enemies cannot move through trees
- [ ] Enemies cannot move through different-colored enemies
- [ ] Same-colored enemies merge when on same square
- [ ] Dormant enemies awaken when player adjacent
- [ ] Enemies eliminate when on goal
- [ ] Player loses when caught by enemy
- [ ] Player wins when all enemies eliminated
- [ ] Reset button restarts level
- [ ] Next level button progresses to next level
- [ ] Movement animations are smooth
- [ ] Grid display is correct (5x5)
