# Will-o'-the-Wisp - Game Design Document

A Turn-Based Tactical Puzzle Game | Unity Development Project | Version 1.0 | January 2026

---

## Executive Summary

Will-o'-the-Wisp is a turn-based tactical puzzle game where players control a mystical glowing spirit from folklore. Instead of leading travelers astray, players use their ethereal glow to manipulate corrupted forest creatures into sacred purification zones. The game combines strategic positioning with pathfinding manipulation on a compact 5x5 grid, creating elegant puzzles that reward careful planning and clever deception.

**Target Platform:** PC/Mac (Unity)
**Genre:** Turn-based tactical puzzle
**Target Audience:** Puzzle enthusiasts aged 12+, fans of strategy games, players who enjoy games like Into the Breach, Baba Is You, and Stephen's Sausage Roll

---

## Players View (User Interface)

### Game View Layout

The main game screen presents a 5x5 grid rendered as a mystical forest clearing at night. The view is top-down, showing all game elements clearly against a dark atmospheric background.

### Visual Elements

| Element | Visual Representation | Description |
|---------|----------------------|-------------|
| Player (Will-o'-the-Wisp) | Glowing cyan orb with pulse animation and soft glow effect | The player character, a mystical spirit light |
| Active Enemies | Red circles, may display merge count | Corrupted creatures that always pursue the player |
| Dormant Enemies | Purple circles | Creatures that only activate when player is adjacent |
| Goal/Sacred Grove | Green square | Purification zone that eliminates enemies |
| Trees/Obstacles | Dark green tree shapes | Blocks enemy movement, player passes through |
| Mud Patches | Amber/brown circular patches | Traps enemies for one turn |
| Trapped Indicator | Small amber dot on enemy | Shows enemy is stuck in mud this turn |

### UI Components

- **Level Header:** Displays current level number and name with navigation arrows
- **Enemy Counter:** Shows remaining enemies to eliminate
- **Message Display:** Shows win/lose messages and game state feedback
- **Control Instructions:** Keyboard and mouse control reference
- **Action Buttons:** Reset, Show Solution, Next Level buttons
- **How to Play Panel:** Collapsible legend explaining all game elements

### Move Preview System

When hovering over a valid move destination, the UI displays predicted enemy movements with color-coded highlights: orange squares indicate where enemies will move, and yellow squares indicate where multiple enemies will merge.

---

## Game Classification Matrix

### Skill-Luck Matrix

**Position: High Skill, Zero Luck**

Will-o'-the-Wisp is a purely deterministic puzzle game. There is no randomness in gameplay. All puzzles have guaranteed solutions and enemy behavior is 100% predictable based on clear rules. Success depends entirely on player understanding and execution.

### Mental-Physical Matrix

**Position: Purely Mental**

The game requires no physical dexterity, timing, or reflexes. Being turn-based, players have unlimited time to consider their moves. Success comes from logical deduction, spatial reasoning, and understanding enemy pathfinding behavior.

### Complexity Comparison

| Dimension | Rating (1-10) | Comparable Games |
|-----------|---------------|-----------------|
| Strategic Depth | 7 | Into the Breach, Chess puzzles |
| Rules Complexity | 4 | Threes!, 2048 |
| Spatial Reasoning | 8 | Sokoban, Baba Is You |
| Execution Difficulty | 2 | Point-and-click adventures |

---

## Verbs (Mechanics)

### Player Actions

| Verb | Description |
|------|-------------|
| **MOVE** | Move one square in any of 8 directions (orthogonal or diagonal). Player can move through trees but cannot move onto enemy-occupied squares. |
| **LURE** | Passive mechanic. Moving causes enemies to pursue, allowing strategic positioning to guide them toward the goal. |
| **PREVIEW** | Hover over potential moves to see where enemies will go if that move is made. |
| **RESET** | Restart the current level to try a different approach. |
| **SOLVE** | Request a step-by-step solution (optional hint system). |

### Enemy Behaviors

| Verb | Description |
|------|-------------|
| **CHASE** | Active (red) enemies move one square toward the player every turn. Movement is limited to 4 cardinal directions only. |
| **AWAKEN** | Dormant (purple) enemies only begin chasing when the player moves adjacent to them. |
| **AVOID** | When enemies have equally good movement options, they choose the one that moves them further from the goal. |
| **MERGE** | Same-colored enemies that occupy the same square combine into one (reducing total enemy count). |
| **BLOCK** | Different-colored enemies treat each other as obstacles and cannot occupy the same square. |
| **STUCK** | Enemies on mud patches cannot move for one turn. |
| **ELIMINATE** | Enemies that move onto the goal square are eliminated. |

### Restrictions

- Player **CANNOT** move onto squares occupied by enemies
- Player **CANNOT** move outside the 5x5 grid
- Enemies **CANNOT** move diagonally
- Enemies **CANNOT** move through trees
- Enemies **CANNOT** move through different-colored enemies

---

## The Experience of Play (Goals)

### Core Experience Vision

Players should feel like a clever trickster spirit, using their wits to outsmart mindless pursuers. The moment of realization when a player sees how to manipulate enemy pathfinding to their advantage should feel like an 'aha!' moment of discovery.

### Emotional Journey

1. **Initial Assessment:** Player observes the puzzle, notes enemy positions, goal location, and obstacles
2. **Planning:** Player considers different approaches, potentially using the preview system
3. **Tension:** Executing moves while enemies close in creates urgency
4. **Revelation:** Understanding how enemy avoidance behavior can be exploited
5. **Satisfaction:** Successfully luring all enemies to their demise

### Design Pillars

- **Clarity:** Every game rule must be understandable from observation. No hidden mechanics.
- **Fairness:** Every puzzle is tested to be solvable. No guessing required.
- **Elegance:** Simple rules create complex situations. The 5x5 grid constraint forces creative puzzle design.
- **Agency:** Players feel in control because enemy behavior is predictable and previewable.

### Difficulty Progression

Levels are designed with escalating complexity through introduction of new elements and increased puzzle intricacy:

| Phase | Concepts Introduced |
|-------|-------------------|
| Tutorial (1-3) | Basic movement, single enemy pursuit, goal mechanics |
| Foundation (4-6) | Multiple enemies, enemy merging, diagonal movement advantage |
| Intermediate (7-9) | Trees as strategic barriers, mud patch timing |
| Advanced (10-12) | Dormant enemy activation, enemy blocking mechanics |
| Expert (13+) | Combined mechanics, multi-step solutions, tight positioning |

---

## End Game Conditions

### Victory Conditions

**Primary Win Condition:** All enemies have been eliminated by moving onto the goal square.

- Victory screen displays congratulatory message
- 'Next Level' button becomes available
- Level completion is recorded for progression tracking

### Defeat Conditions

**Checkmate:** All options force an enemy to occupy the same square as the player.

- Game over screen with 'You were caught!' message
- Reset button prominently displayed
- No permanent penalty - immediate retry available

**Note:** There are some positions that are a 'draw' i.e, the player is not checkmated but there is no possible solution to move the enemy to the goal square. Virtually impossible to design levels where there is no potential for this.

Either let players figure this out and reset or add a draw state.

### No Unsolvable Levels

The game prevents unsolvable initial states through level design. All levels are verified to be solvable through algorithmic testing (BFS pathfinding with state exploration) before inclusion in the game.

---

## Controls

### Primary Control Schemes

| Input Method | Controls | Notes |
|--------------|----------|-------|
| Mouse/Touch | Click/tap adjacent squares to move | Primary method, most intuitive |
| Arrow Keys | Cardinal directions (Up/Down/Left/Right) | Standard keyboard navigation |
| WASD | W=Up, A=Left, S=Down, D=Right | Gamer-friendly alternative |
| QEZC | Q=Up-Left, E=Up-Right, Z=Down-Left, C=Down-Right | Diagonal keyboard movement |

### UI Controls

| Action | Method |
|--------|--------|
| Reset Level | Click 'Reset' button or press R |
| Show Solution | Click 'Show Solution' button |
| Next Level | Click 'Next' arrow or button after victory |
| Previous Level | Click 'Prev' arrow to replay earlier levels |
| Preview Move | Hover mouse over valid destination square |

### Unity Input System Configuration

The game will use Unity's new Input System for cross-platform compatibility. Input actions will be defined as:

- **Move:** Vector2 composite for 8-directional movement
- **Select:** Button action for click/tap confirmation
- **Reset:** Button action bound to R key
- **Undo:** Button action bound to U key (future feature)

---

## Actions and Events

### Turn Sequence Flowchart

Each turn follows a strict sequence to ensure deterministic behavior:

1. **Input Phase:** Player selects movement direction
2. **Validation:** Check if move is legal (within bounds, not onto enemy)
3. **Player Movement:** Wisp moves to new position
4. **Mud Check:** Decrement trapped counters for stuck enemies
5. **Dormant Check:** Activate any dormant enemies adjacent to new player position
6. **Enemy Movement:** Each non-stuck enemy moves toward player
7. **Collision Resolution:** Handle merging and blocking
8. **Goal Check:** Remove enemies on goal square
9. **Capture Check:** If enemy on player position, trigger defeat
10. **Victory Check:** If no enemies remain, trigger victory

### Event System

| Event | Triggered When / Response |
|-------|--------------------------|
| **OnPlayerMove** | Player successfully moves. Triggers enemy AI update, animations. |
| **OnEnemyMove** | Each enemy moves. Updates position, checks for mud/goal. |
| **OnEnemyMerge** | Two same-color enemies overlap. Merge animation, update count. |
| **OnEnemyElim** | Enemy reaches goal. Purification VFX, removal from game. |
| **OnDormantAwaken** | Player moves adjacent to dormant enemy. Color change, activation. |
| **OnPlayerCaught** | Enemy reaches player position. Defeat screen, reset prompt. |
| **OnLevelComplete** | All enemies eliminated. Victory screen, next level unlock. |
| **OnLevelReset** | Player requests reset. Restore initial state. |

---

## Object Models (Data)

### Core Classes

#### GameManager (Singleton)

Central controller managing game state and turn flow.

| Variable | Type | Purpose |
|----------|------|---------|
| currentLevel | int | Index of current level |
| gameState | GameState enum | Playing, Won, Lost, Paused |
| player | PlayerController | Reference to player object |
| enemies | List<EnemyController> | All active enemies |
| goal | Vector2Int | Goal position |
| obstacles | List<Vector2Int> | Tree positions |
| mudPatches | List<Vector2Int> | Mud positions |

#### PlayerController

Handles player input and movement.

| Variable | Type | Purpose |
|----------|------|---------|
| position | Vector2Int | Grid position (0-4, 0-4) |
| canMove | bool | Input lock during animations |

#### EnemyController

Base class for enemy behavior.

| Variable | Type | Purpose |
|----------|------|---------|
| position | Vector2Int | Current grid position |
| enemyType | EnemyType enum | Normal, Dormant |
| isActive | bool | False for dormant until triggered |
| trappedTurns | int | Turns remaining stuck in mud |
| id | int | Unique identifier for state tracking |

#### LevelData (ScriptableObject)

Serialized level configuration.

| Variable | Type | Purpose |
|----------|------|---------|
| levelName | string | Display name |
| playerStart | Vector2Int | Initial player position |
| goalPosition | Vector2Int | Goal location |
| enemies | EnemySpawn[] | Enemy positions and types |
| obstacles | Vector2Int[] | Tree positions |
| mudPatches | Vector2Int[] | Mud positions |
| minMoves | int | Optimal solution length |

---

## Entity Relationships

### Relationship Diagram Description

The following describes the relationships between core game entities:

| Entity A | Entity B | Relationship |
|----------|----------|--------------|
| GameManager | Player | One-to-One: Single player per game |
| GameManager | Enemy | One-to-Many: Multiple enemies per level |
| GameManager | LevelData | One-to-One: One active level |
| GameManager | GridCell | One-to-Many: 25 cells (5x5) |
| Player | GridCell | One-to-One: Occupies single cell |
| Enemy | GridCell | Many-to-One: Multiple may occupy (merge) |
| Enemy | Enemy | Many-to-Many: Same color merge, diff block |
| GridCell | Terrain | One-to-One: Cell may have terrain type |

---

## Interaction Matrix

|  | Player | Active Enemy | Dormant Enemy | Goal |
|---|--------|--------------|---------------|------|
| **Player** | - | Cannot overlap | Awakens adjacent | No interaction |
| **Active Enemy** | Catches player | Merges same / Blocks diff | Blocks | Eliminated |
| **Dormant** | Catches if active | Blocks | Merges if active | Eliminated if active |
| **Tree** | Pass through | Blocked | Blocked | - |
| **Mud** | No effect | Traps 1 turn | Traps 1 turn | - |

---

## Finite State Machines

### Game State Machine

| State | Allowed Transitions | Trigger |
|-------|-------------------|---------|
| Loading | Playing | Level data loaded |
| Playing | Won, Lost, Paused | Victory/defeat/pause input |
| Won | Loading | Next level or reset |
| Lost | Loading | Reset or level select |
| Paused | Playing | Resume input |

### Enemy State Machine

| State | Behavior | Transitions To |
|-------|----------|----------------|
| Idle (Dormant only) | No movement, waiting for trigger | Chasing (player adjacent) |
| Chasing | Move toward player each turn | Stuck, Purified, Merged |
| Stuck | Cannot move for trappedTurns | Chasing (counter reaches 0) |
| Purified | Reached goal, removal animation | Removed (terminal) |
| Merged | Combined with another enemy | Removed (terminal) |

### Turn Phase State Machine

Each turn cycles through these phases in order:

1. **WaitingForInput** → Player has control
2. **PlayerMoving** → Animation playing
3. **ProcessingEnemies** → AI calculations
4. **EnemiesMoving** → Enemy animations
5. **ResolvingCollisions** → Merge/purify checks
6. **CheckingEndConditions** → Win/lose evaluation
7. → Returns to WaitingForInput or transitions to Won/Lost

---

## Intermediate Expansions

Features that extend the base game with manageable implementation effort.

### Move Counter / Par System

- Track number of moves taken to complete each level
- Display optimal solution length as 'par'
- Award stars based on efficiency (3 stars = optimal, 2 = par+2, 1 = completed)
- **Implementation:** Simple counter variable, UI text

### Undo System

- Allow players to revert to previous turn state
- Stack-based state history
- Limited undos per level (3-5) or unlimited with score penalty
- **Implementation:** Serialize game state to stack, pop on undo

### New Terrain: Ice Patches

- Enemies slide until hitting obstacle or edge
- Creates momentum-based puzzles
- **Implementation:** Extend movement logic with slide check

### New Terrain: Teleport Pads

- Paired tiles that teleport entities between them
- Works for both player and enemies
- **Implementation:** Post-movement position check and swap

### Level Select Screen

- Visual grid of all levels with completion status
- Star ratings displayed
- Locked levels grayed out until prerequisites met
- **Implementation:** ScriptableObject level database, UI scroll view

### Sound Effects & Music

- Atmospheric forest ambience
- Movement sounds (wisp whoosh, enemy footsteps)
- Purification chime, defeat sound, victory fanfare
- **Implementation:** Audio source components, trigger from events

---

## Advanced Expansions

Complex features requiring significant development effort or new systems.

### New Enemy Type: Patrol Guards

- Enemies that follow a fixed patrol route
- Only chase when player enters their 'vision cone'
- Return to patrol if player escapes
- **Complexity:** Requires waypoint system, vision detection, multi-state AI

### Procedural Level Generation

- Algorithm generates random but solvable puzzles
- Difficulty scaling based on player progression
- Daily challenge mode with shared seeds
- **Complexity:** Constraint satisfaction, solvability verification, difficulty estimation

### Level Editor

- Players can create and share custom levels
- Built-in solvability checker before publishing
- Community level browser
- **Complexity:** Full editor UI, cloud storage

### Larger Grid Sizes

- Support for 6x6, 7x7, or variable grid sizes
- Camera zoom/pan for larger puzzles
- Increased solution complexity
- **Complexity:** Dynamic grid generation, camera system, performance optimization

### Narrative Campaign

- Story-driven progression with cutscenes
- Character development for the wisp
- Multiple biomes (swamp, forest, cave, etc.)
- **Complexity:** Art assets, dialogue system, narrative design, voice acting

### Multiplayer: Asynchronous Puzzle Duels

- Players solve the same puzzle, compare move counts
- Leaderboards per level
- Ghost replay of opponent's solution
- **Complexity:** Backend infrastructure, replay system, matchmaking

---

## Technical Specifications

### Unity Project Setup

- **Unity Version:** 2022.3 LTS or newer
- **Render Pipeline:** Universal Render Pipeline (URP) for 2D
- **Input System:** New Input System package
- **Target Platforms:** PC (Windows/Mac), with mobile consideration

### Folder Structure

```
Assets/
  ├── Scripts/ (GameManager, Controllers, UI)
  ├── ScriptableObjects/ (LevelData, GameSettings)
  ├── Prefabs/ (Player, Enemies, Grid, UI)
  ├── Art/ (Sprites, Animations)
  ├── Audio/ (SFX, Music)
  └── Scenes/ (MainMenu, Game, LevelSelect)
```

---

## Development Checklist

### Phase 1: Core Mechanics

- [ ] Grid rendering system
- [ ] Player movement (8-directional)
- [ ] Basic enemy AI (chase behavior)
- [ ] Goal interaction (enemy elimination)
- [ ] Win/lose conditions

### Phase 2: Extended Mechanics

- [ ] Dormant enemy type
- [ ] Tree obstacles
- [ ] Mud patches
- [ ] Enemy merging
- [ ] Move preview system

### Phase 3: UI & Polish

- [ ] Level selection
- [ ] Visual feedback (animations)
- [ ] Audio implementation
- [ ] Settings menu
- [ ] Tutorial levels

### Phase 4: Content

- [ ] Design 20+ levels with progressive difficulty
- [ ] Verify solvability for all levels
- [ ] Playtest and iterate
- [ ] Performance optimization
- [ ] Build and deploy

---

**Version:** 1.0
**Date:** January 2026
**Status:** Ready for Development
**Platform:** Unity 2022.3+

---

*— End of Document —*
