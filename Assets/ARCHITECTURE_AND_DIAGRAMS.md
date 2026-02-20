# Will-o'-the-Wisp - Architecture & Visual Diagrams

## Table of Contents
1. [System Architecture](#system-architecture)
2. [Data Flow Diagrams](#data-flow-diagrams)
3. [Class Relationships](#class-relationships)
4. [Turn Sequence Flowchart](#turn-sequence-flowchart)
5. [Coordinate System](#coordinate-system)
6. [Component Dependencies](#component-dependencies)

---

## System Architecture

### High-Level System Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                     WILL-O'-THE-WISP GAME                       │
└─────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────┐
│                        INPUT LAYER                               │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ PlayerController                                        │   │
│  │ - Reads keyboard input                                 │   │
│  │ - Validates moves (bounds, collision)                 │   │
│  │ - Triggers GameManager.ProcessTurn()                  │   │
│  └─────────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────────┘
                              ↓
┌──────────────────────────────────────────────────────────────────┐
│                      GAME LOGIC LAYER                            │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ GameManager (Singleton)                               │   │
│  │ - Manages game state (Playing, Won, Lost)            │   │
│  │ - Executes turn sequence                             │   │
│  │ - Handles collisions and merging                     │   │
│  │ - Checks win/lose conditions                         │   │
│  └─────────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────────┘
         ↓                    ↓                    ↓
    ┌─────────────┐   ┌─────────────┐   ┌──────────────────┐
    │   ENTITIES  │   │   LEVEL     │   │   UTILITIES      │
    │             │   │   DATA      │   │                  │
┌───┴─────────────┴──┬┴─────────────┴──┬┴──────────────────┴──┐
│                    │                  │                      │
├────────────────────┼──────────────────┼──────────────────────┤
│ PlayerController   │ LevelData        │ GridHelper           │
│ - Grid position    │ (ScriptableObj)  │ - Coordinate convert │
│ - Movement logic   │ - Level config   │ - Bounds checking    │
│ - Animation        │ - Enemy spawns   │ - Distance calc      │
│                    │ - Obstacles      │ - Adjacent check     │
│ EnemyController    │ - Goal position  │                      │
│ - Pathfinding AI   │                  │ GridRenderer         │
│ - Chase behavior   │ LevelManager     │ - Visual grid debug  │
│ - State mgmt       │ - Load levels    │ - Gizmo drawing      │
│ - Animation        │ - Instantiate    │                      │
│                    │ - Cleanup        │ UIManager            │
│                    │                  │ - Display info       │
│                    │                  │ - Handle buttons     │
│                    │                  │ - Win/lose screens   │
│                    │                  │                      │
└────────────────────┴──────────────────┴──────────────────────┘

              ↓
┌──────────────────────────────────────────────────────────────────┐
│                    RENDERING LAYER                               │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ SpriteRenderer Components                              │   │
│  │ - Player visual (cyan circle)                          │   │
│  │ - Enemy visuals (red/purple circles)                   │   │
│  │ - Obstacle visual (dark green tree)                    │   │
│  │ - Goal visual (green square)                           │   │
│  │ - UI Canvas (buttons, text)                            │   │
│  └─────────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────────┘
```

---

## Data Flow Diagrams

### Move Sequence Data Flow

```
PLAYER PRESSES ARROW KEY
    ↓
PlayerController.Update()
    ↓
Input.GetKeyDown() detects press
    ↓
HandleInput() processes input
    ↓
AttemptMove(direction) called
    ↓
    ├─→ Validate move:
    │   ├─→ GameManager.IsValidPosition() - bounds check
    │   └─→ GameManager.IsEnemyAtPosition() - collision check
    │
    ├─→ If invalid: return false (move blocked)
    │
    └─→ If valid:
        ├─→ PlayerController.SetPosition(newPos)
        │   ├─→ Update grid position
        │   └─→ AnimateToPosition() - start smooth animation
        │
        ├─→ GameManager.ProcessTurn()
        │   ├─→ STEP 1: DecrementTrapCounter() for trapped enemies
        │   ├─→ STEP 2: Check for dormant activation (IsAdjacent)
        │   ├─→ STEP 3: Move enemies (EnemyController.MoveTowardPlayer)
        │   │   └─→ FindNextMoveTowardPlayer() - pathfinding
        │   ├─→ STEP 4: ResolveCollisions() - enemy merging
        │   ├─→ STEP 5: PurifyEnemiesOnGoal() - eliminate on goal
        │   ├─→ STEP 6: Check IsEnemyAtPosition(player) - defeat?
        │   └─→ STEP 7: Check GetAllActiveEnemies().Count == 0 - victory?
        │
        ├─→ GameManager.SetGameState() - update state if won/lost
        │
        └─→ Return to gameplay

RESULT: Enemy positions updated, UI refreshed, turn complete
```

### Enemy Movement Decision Tree

```
EnemyController.MoveTowardPlayer(playerPos)
    ↓
    ├─→ Check if can move:
    │   ├─→ IsAlive? No → Stop
    │   ├─→ IsActive? No → Stop
    │   └─→ IsTrapped? Yes → Stop
    │
    └─→ FindNextMoveTowardPlayer(playerPos)
        ↓
        For each cardinal direction (up, down, left, right):
        │
        ├─→ Calculate nextPos = currentPos + direction
        │
        ├─→ Validate move:
        │   ├─→ In bounds? No → Skip
        │   ├─→ Tree obstacle? Yes → Skip
        │   └─→ Enemy blocking? Yes → Skip
        │
        └─→ If valid move:
            ├─→ Calculate distance to player
            │
            ├─→ If closer to player:
            │   └─→ If closest so far: set as bestMove
            │
            └─→ If equidistant from player:
                ├─→ Calculate distance to goal
                └─→ If further from goal: set as bestMove (avoidance)

RESULT: bestMove = optimal next position toward player
        (or current position if stuck/blocked)
```

---

## Class Relationships

### UML-style Class Diagram

```
┌─────────────────────────────────────┐
│         MonoBehaviour               │
│    (Unity Base Class)               │
└─────────────────────────────────────┘
        ▲           ▲        ▲
        │           │        │
        │           │        │
    ┌───┴─┐      ┌──┴──┐  ┌─┴────┐
    │     │      │     │  │      │
    │     │      │     │  │      │

┌──────────────────────────────────────────┐
│         GameManager                       │
│  (Singleton - Central Controller)        │
├──────────────────────────────────────────┤
│ - currentLevel: int                      │
│ - gameState: GameState enum              │
│ - player: PlayerController ──────┐       │
│ - enemies: List<EnemyController> │       │
│ - goalPosition: Vector2Int       │       │
│ - obstacles: List<Vector2Int>    │       │
│ - mudPatches: List<Vector2Int>   │       │
├──────────────────────────────────────────┤
│ + ProcessTurn()                          │
│ + IsValidPosition()                      │
│ + IsEnemyAtPosition()                    │
│ + SetGameState()                         │
│ + ResetLevel()                           │
└──────────────────────────────────────────┘
        │
        │ manages
        ▼
┌──────────────────────────────┐
│    PlayerController          │
├──────────────────────────────┤
│ - position: Vector2Int       │
│ - canMove: bool              │
│ - moveAnimationSpeed: float  │
├──────────────────────────────┤
│ + AttemptMove()              │
│ + SetPosition()              │
│ + HandleInput()              │
└──────────────────────────────┘


        │
        │ manages many
        ▼
┌──────────────────────────────────────┐
│      EnemyController                 │
├──────────────────────────────────────┤
│ - position: Vector2Int               │
│ - enemyType: EnemyType enum          │
│ - isActive: bool                     │
│ - isAlive: bool                      │
│ - trappedTurns: int                  │
│ - moveAnimationSpeed: float          │
├──────────────────────────────────────┤
│ + MoveTowardPlayer()                 │
│ + FindNextMoveTowardPlayer()         │
│ + Awaken()                           │
│ + Purify()                           │
│ + TrappedInMud()                     │
│ + DecrementTrapCounter()             │
└──────────────────────────────────────┘


┌──────────────────────────────────────┐
│       LevelManager                    │
├──────────────────────────────────────┤
│ - levels: List<LevelData>            │
│ - playerPrefab: GameObject           │
│ - enemyPrefab: GameObject            │
│ - currentLevelIndex: int             │
├──────────────────────────────────────┤
│ + LoadLevel()                        │
│ + NextLevel()                        │
│ + PreviousLevel()                    │
│ + ReloadCurrentLevel()               │
└──────────────────────────────────────┘
        │
        │ loads
        ▼
┌──────────────────────────────────────┐
│     LevelData (ScriptableObject)    │
├──────────────────────────────────────┤
│ - levelName: string                  │
│ - levelNumber: int                   │
│ - playerStart: Vector2Int            │
│ - goalPosition: Vector2Int           │
│ - enemies: EnemySpawn[]              │
│ - obstacles: List<Vector2Int>        │
│ - mudPatches: List<Vector2Int>       │
│ - minMoves: int                      │
├──────────────────────────────────────┤
│ + ValidateLevel()                    │
└──────────────────────────────────────┘


┌──────────────────────────────────────┐
│       UIManager                       │
├──────────────────────────────────────┤
│ - levelHeaderText: TextMeshProUGUI   │
│ - enemyCounterText: TextMeshProUGUI  │
│ - messageText: TextMeshProUGUI       │
│ - resetButton: Button                │
│ - nextLevelButton: Button            │
│ - winPanel: GameObject               │
│ - losePanel: GameObject              │
├──────────────────────────────────────┤
│ + UpdateUI()                         │
│ + OnResetClicked()                   │
│ + OnNextLevelClicked()               │
│ + ShowWinScreen()                    │
│ + ShowLoseScreen()                   │
└──────────────────────────────────────┘


┌──────────────────────────────────────┐
│       GridHelper (Static)            │
│   (Coordinate Conversion Utility)    │
├──────────────────────────────────────┤
│ - GRID_WIDTH: const = 5              │
│ - GRID_HEIGHT: const = 5             │
│ - CELL_SIZE: const = 1.0             │
│ - GRID_OFFSET: const = (-2,-2,0)     │
├──────────────────────────────────────┤
│ + GridToWorld()                      │
│ + WorldToGrid()                      │
│ + IsValidGridPosition()              │
│ + GetAdjacentPositions()             │
│ + ManhattanDistance()                │
└──────────────────────────────────────┘
```

---

## Turn Sequence Flowchart

```
TURN START
    ↓
    ├─ STEP 1: MUD CHECK
    │  └─ For each enemy with IsTrapped == true:
    │     └─ Call DecrementTrapCounter()
    │        (Reduce trapped counter by 1)
    │
    ├─ STEP 2: DORMANT CHECK
    │  └─ For each enemy with IsActive == false:
    │     └─ If player is adjacent:
    │        └─ Call enemy.Awaken()
    │           (Enemy becomes active, changes color)
    │
    ├─ STEP 3: ENEMY MOVEMENT
    │  └─ For each enemy:
    │     └─ If IsAlive && IsActive && !IsTrapped:
    │        └─ Call MoveTowardPlayer(playerPos)
    │           ├─ Calculate optimal move
    │           ├─ Update position
    │           └─ Animate movement
    │
    ├─ STEP 4: COLLISION RESOLUTION
    │  └─ Group enemies by position:
    │     └─ For each group with 2+ enemies:
    │        └─ If all same color:
    │           └─ Merge (keep 1, eliminate rest)
    │        └─ Else:
    │           └─ Do nothing (different colors block)
    │
    ├─ STEP 5: GOAL CHECK
    │  └─ For each enemy:
    │     └─ If Position == GoalPosition:
    │        └─ Call Purify()
    │           (Enemy eliminated)
    │
    ├─ STEP 6: CAPTURE CHECK
    │  └─ If any enemy at PlayerPosition:
    │     ├─ SetGameState(Lost)
    │     └─ Exit (no need to check victory)
    │
    └─ STEP 7: VICTORY CHECK
       └─ If GetAllActiveEnemies().Count == 0:
          └─ SetGameState(Won)

TURN END
    ↓
Return to waiting for player input
```

---

## Coordinate System

### Grid to World Mapping

```
GAME GRID (5x5)          WORLD SPACE (Visual)

(0,0) (1,0) (2,0) ...    (-2,2)  (-1,2)  (0,2) ...
(0,1) (1,1) (2,1) ...    (-2,1)  (-1,1)  (0,1) ...
(0,2) (1,2) (2,2) ...    (-2,0)  (-1,0)  (0,0) ...
 ...   ...   ...         ...     ...     ...

Grid Origin: Top-Left (0,0)       World Origin: Center (0,0)
Grid Size: 5x5 cells               World Size: 5x5 units
Cell Size: 1 unit                  Offset: (-2, -2, 0)
Grid Y: increases downward         World Y: increases upward


CONVERSION FORMULAS:

GridToWorld(grid: Vector2Int) → Vector3
  ├─ offsetX = grid.x * CELL_SIZE + CELL_SIZE * 0.5f = grid.x + 0.5
  ├─ offsetY = grid.y * CELL_SIZE + CELL_SIZE * 0.5f = grid.y + 0.5
  └─ world = GRID_OFFSET + (offsetX, offsetY, 0)
           = (-2, -2, 0) + (grid.x + 0.5, grid.y + 0.5, 0)
           = (grid.x - 1.5, grid.y - 1.5, 0)

Example:
  Grid (0,0) → World (-1.5, -1.5, 0)
  Grid (2,2) → World (0.5, 0.5, 0)
  Grid (4,4) → World (2.5, 2.5, 0)

WorldToGrid(world: Vector3) → Vector2Int
  ├─ relative = world - GRID_OFFSET = (world + (2,2,0))
  └─ grid = FloorToInt(relative / CELL_SIZE)
          = FloorToInt(world + (2,2,0))


VISUAL REPRESENTATION:

Grid Space (0-4):          World Space (-2 to 3):
┌─────────────────┐        ┌─────────────────┐
│(0,0) (1,0)...  │        │(-2,2) (-1,2)..  │
│(0,1) (1,1)...  │        │(-2,1) (-1,1)..  │
│(0,2) (1,2)...  │   →    │(-2,0) (-1,0)..  │
│(0,3) (1,3)...  │        │(-2,-1)(-1,-1).. │
│(0,4) (1,4)...  │        │(-2,-2)(-1,-2).. │
└─────────────────┘        └─────────────────┘

5x5 Grid centered at origin for balanced visuals
```

---

## Component Dependencies

### Dependency Graph

```
┌──────────────────────────────────────────────────────────────┐
│                      GAME SCENE                              │
└──────────────────────────────────────────────────────────────┘

Main Camera (Orthographic)
├─ View: 5x5 grid area
├─ Position: (0, 0, -10)
└─ Shows all game entities

┌──────────────────────┐
│   GameManager        │  (Singleton)
│  ┌────────────────┐  │  - Loaded first
│  │ Dependencies:  │  │  - Accessible from anywhere
│  │ - NONE         │  │  - Manages all game flow
│  └────────────────┘  │
└──────────────────────┘
    ▲   ▲   ▲
    │   │   │ references
    │   │   │
    │   │   └──────────────────────┐
    │   │                          │
    │   └──────────────┐           │
    │                  │           │
    ▼                  ▼           ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│PlayerControl │  │Enemy[0]      │  │Enemy[1]      │
│  Dependence: │  │  Dependence: │  │  Dependence: │
│  - Game Mgr  │  │  - Game Mgr  │  │  - Game Mgr  │
│  - Grid      │  │  - Grid      │  │  - Grid      │
│  - Input     │  │  - None      │  │  - None      │
│  - Animation │  │  - Animation │  │  - Animation │
└──────────────┘  └──────────────┘  └──────────────┘

┌──────────────────┐
│  LevelManager    │  - Loaded second
│  Dependencies:   │  - Loads levels
│  - Game Mgr      │  - Instantiates entities
│  - LevelData     │  - Manages level transitions
│  - Prefabs       │
└──────────────────┘

┌──────────────────┐
│  UIManager       │  - Loaded with Canvas
│  Dependencies:   │  - Updates UI display
│  - Game Mgr      │  - Handles button clicks
│  - Level Mgr     │  - Shows win/lose screens
│  - UI Elements   │
└──────────────────┘

┌──────────────────┐
│  GridRenderer    │  - Debug visualization
│  Dependencies:   │  - Draws gizmo grid
│  - Grid Helper   │  - Not needed for gameplay
└──────────────────┘

┌──────────────────┐
│ GridHelper       │  - Static utility class
│  Dependencies:   │  - Used by all movement
│  - NONE          │  - No dependencies on others
└──────────────────┘
```

### Initialization Order

```
1. SCENE LOADS
   └─ MonoBehaviour.Awake() called for all objects
      ├─ GameManager.Awake()
      │  └─ Sets up singleton instance
      ├─ PlayerController.Awake()
      │  └─ (No dependencies)
      ├─ EnemyController.Awake() for each enemy
      │  └─ Calls GameManager.Instance.AddEnemy(this)
      └─ LevelManager.Awake()
         └─ (No dependencies)

2. OBJECTS INITIALIZED
   └─ MonoBehaviour.Start() called for all objects
      ├─ GameManager.Start()
      │  └─ Calls InitializeLevel()
      │     └─ Finds PlayerController and EnemyControllers
      ├─ PlayerController.Start()
      │  └─ Sets position and animation
      ├─ EnemyController.Start() for each enemy
      │  └─ Sets position and visual appearance
      ├─ LevelManager.Start()
      │  └─ Loads first level from list
      │     └─ Instantiates player, enemies, obstacles from prefabs
      └─ UIManager.Start()
         └─ Updates UI with current level info

3. GAME RUNNING
   └─ MonoBehaviour.Update() every frame
      ├─ PlayerController.Update()
      │  └─ Handles input each frame
      └─ UIManager.Update()
         └─ Updates UI elements each frame

4. TURN EXECUTES
   └─ When player presses move key:
      ├─ PlayerController.AttemptMove()
      │  └─ Validates move via GameManager
      │     └─ If valid: GameManager.ProcessTurn()
      │        └─ Executes 7-step turn sequence
      │           └─ Updates all enemies via EnemyController
      │              └─ Calls FindNextMoveTowardPlayer() via GridHelper
      │           └─ Updates UI via UIManager
      │              └─ Checks GameState and shows appropriate screens
      └─ Loop back to waiting for input
```

---

## Event Flow Diagram

```
GAME STARTUP
    ↓
[Scene Loads]
    ↓
Awake() Phase
├─ GameManager.Awake() → becomes Singleton
├─ PlayerController.Awake() → (no-op)
└─ EnemyController.Awake() → registers with GameManager
    ↓
Start() Phase
├─ GameManager.Start() → InitializeLevel()
├─ PlayerController.Start() → sets initial position
├─ EnemyController.Start() → sets initial position + visuals
├─ LevelManager.Start() → LoadLevel(0)
└─ UIManager.Start() → UpdateUI()
    ↓
MAIN GAME LOOP RUNNING
    ↓
[Every Frame]
├─ PlayerController.Update()
│  └─ Reads input
│     └─ If key pressed: HandleInput()
│        └─ AttemptMove()
│           ├─ Validate move
│           ├─ SetPosition() + animation
│           └─ GameManager.ProcessTurn()
│              ├─ Update all enemies
│              ├─ Check collisions
│              └─ Check win/lose
│                 └─ SetGameState()
│                    └─ Events: OnGameStateChanged()
│
└─ UIManager.Update()
   └─ Check GameState
      └─ UpdateUI()
         ├─ Show/hide win panel
         └─ Show/hide lose panel

[Button Clicked]
├─ Reset Button → UIManager.OnResetClicked()
│  └─ LevelManager.ReloadCurrentLevel()
│     └─ Destroys all entities
│     └─ LoadLevel(currentIndex) again
│
├─ Next Button → UIManager.OnNextLevelClicked()
│  └─ LevelManager.NextLevel()
│     └─ LoadLevel(currentIndex + 1)
│
└─ Prev Button → UIManager.OnPreviousLevelClicked()
   └─ LevelManager.PreviousLevel()
      └─ LoadLevel(currentIndex - 1)
```

---

## State Machine Diagram

### Game State Machine

```
         ┌────────────────────────────────┐
         │        LOADING STATE           │
         │  (Setting up level)            │
         │  - Instantiate prefabs         │
         │  - Initialize positions        │
         │  - Setup enemy list            │
         └────────────────────────────────┘
                        ↓
                  [Level Ready]
                        ↓
         ┌────────────────────────────────┐
         │      PLAYING STATE             │ ◄────┐
         │  (Gameplay in progress)        │      │
         │  - Accept player input         │      │
         │  - Execute turn sequence       │      │
         │  - Update enemies              │      │
         │  - Check win/lose conditions   │      │
         └────────────────────────────────┘      │
              ↙                  ↘               │
        [Player Caught]    [All Enemies Defeated] │
             ↓                    ↓              │
    ┌─────────────────┐  ┌──────────────────┐   │
    │   LOST STATE    │  │    WON STATE     │   │
    │                 │  │                  │   │
    │ Show Lose       │  │ Show Win         │   │
    │ Screen          │  │ Screen           │   │
    │                 │  │                  │   │
    └─────────────────┘  └──────────────────┘   │
         ↓                      ↓                │
      [Reset Clicked]    [Next Level Clicked]   │
         └──────────────────────────────────────┘
                     ↓
              [Return to LOADING]


PAUSED STATE (Not implemented in prototype)
─────────────────────────────────────────
         ┌────────────────────────────────┐
         │      PAUSED STATE              │
         │  (Game suspended)              │
         │  - Pause all movement          │
         │  - Show pause menu             │
         └────────────────────────────────┘
              ↙              ↘
        [Resume]        [Quit to Menu]
             ↓               ↓
        [PLAYING]      [Return to MainMenu]
```

### Enemy State Machine

```
ACTIVE ENEMY (e.g., Red)          DORMANT ENEMY (e.g., Purple)
┌────────────────────────┐        ┌────────────────────────┐
│    CHASING STATE       │        │     IDLE STATE         │
│  Every turn:           │        │  Waiting for trigger:  │
│  - Move toward player  │        │  - Don't move          │
│  - If on goal: Purify  │        │  - Dimmed appearance   │
│  - If caught player:   │        │                        │
│    Mark player lost    │        └────────────────────────┘
└────────────────────────┘              ↓
         ↙        ↘        ┌─────────────────────────────┐
    [Stuck]     [On Goal]  │ [Player Adjacent]           │
      ↓            ↓       │ (IsAdjacent check)          │
   ┌─────┐    ┌────────┐   └─────────────────────────────┘
   │STUCK│    │PURIFIED│                ↓
   │STATE│    │ STATE  │     ┌──────────────────────┐
   │     │    │        │     │   CHASING STATE      │
   │Trapped│  │Eliminated   │ - Now actively moves  │
   │1 turn│  │Destroyed    │ - No longer dormant   │
   └─────┘    └────────────┘ │ - Color brightens   │
      ↓                      │ - Same as Active    │
   Counter──→[Counter = 0]   └──────────────────────┘
      ↓
   CHASING

STATE TRANSITIONS SUMMARY:
─────────────────────────
Active Enemy:   CHASING → STUCK → CHASING → PURIFIED (terminal)
                         ↓ (1 turn)
Dormant Enemy:  IDLE → CHASING → STUCK/PURIFIED
                (on player adjacent)
```

---

## Summary

This architecture ensures:

✓ **Separation of Concerns** - Each class has single responsibility
✓ **Low Coupling** - Minimal dependencies between classes
✓ **High Cohesion** - Related functionality grouped together
✓ **Singleton Pattern** - GameManager accessible globally
✓ **Event-Driven** - State changes trigger appropriate actions
✓ **Deterministic** - No randomness, fully predictable
✓ **Testable** - Each component can be tested independently

The design prioritizes clarity and maintainability while keeping gameplay deterministic and fair.

