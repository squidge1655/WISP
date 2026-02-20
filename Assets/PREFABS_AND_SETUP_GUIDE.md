# Will-o'-the-Wisp - Prefabs & Unity Setup Guide

## Table of Contents
1. [Prefabs Overview](#prefabs-overview)
2. [Creating Prefabs](#creating-prefabs)
3. [Scene Setup](#scene-setup)
4. [Complete Step-by-Step Tutorial](#complete-step-by-step-tutorial)
5. [Testing Checklist](#testing-checklist)

---

## Prefabs Overview

### Required Prefabs Summary

| Prefab Name | Purpose | Components | Notes |
|---|---|---|---|
| **Player** | Player character (Will-o'-the-Wisp) | PlayerController, SpriteRenderer, Transform | Cyan glowing orb |
| **Enemy_Red** | Active red enemy | EnemyController, SpriteRenderer, Transform | Chases immediately |
| **Enemy_Purple** | Dormant purple enemy | EnemyController, SpriteRenderer, Transform | Activates when adjacent |
| **Obstacle** | Tree/obstacle | SpriteRenderer, Transform | Blocks all movement |
| **Mud** | Mud patch | SpriteRenderer, Transform | Traps enemies (future) |
| **Goal** | Sacred grove/goal zone | SpriteRenderer, Transform | Green square |

---

## Prefab Specifications

### 1. Player Prefab

**File Path:** `Assets/Prefabs/Player.prefab`

**Hierarchy:**
```
Player (GameObject)
  ├─ PlayerController (Script Component)
  ├─ SpriteRenderer (Component)
  └─ Transform (Component)
```

**Setup Instructions:**

1. **Create the GameObject:**
   - In Hierarchy, right-click → Create Empty
   - Rename to "Player"

2. **Add Components:**
   - Select Player → Add Component → PlayerController
   - Add Component → Sprite Renderer

3. **Configure SpriteRenderer:**
   - Material: Default Material (or leave blank)
   - Color: Cyan (RGB: 0, 255, 255) or use Color(0, 1, 1, 1)
   - Sorting Order: 2 (above enemies)

4. **Create Sprite (Simple):**
   - Create a simple circular sprite:
     - Use a basic circle PNG or Unity's built-in circle sprite
     - Or create a circle in a sprite editor
   - Set Sprite field in SpriteRenderer to your circle sprite

5. **Alternative (No Sprite):**
   - If no sprite, use a colored quad:
     - Add a 3D Cube as child
     - Scale it down (0.5, 0.5, 0.1)
     - Change material color to cyan

6. **Set Position:**
   - Transform Position: (0, 0, 0) - will be set by script at runtime

7. **Tag/Layer (Optional):**
   - Add Tag: "Player"
   - Add Layer: "Player"

8. **Save as Prefab:**
   - Drag Player from Hierarchy to Assets/Prefabs folder
   - Delete from Hierarchy

**Inspector Settings:**
```
Player (Prefab)
├─ Transform
│  ├─ Position: (0, 0, 0)
│  ├─ Scale: (0.3, 0.3, 0.3)
│  └─ Rotation: (0, 0, 0)
├─ Sprite Renderer
│  ├─ Sprite: (Circle or custom)
│  ├─ Color: Cyan
│  └─ Sorting Order: 2
└─ PlayerController (Script)
   ├─ Position: (0, 0)
   └─ Move Animation Speed: 0.1
```

---

### 2. Enemy Prefabs

**File Paths:**
- `Assets/Prefabs/Enemy_Red.prefab`
- `Assets/Prefabs/Enemy_Purple.prefab`

**Hierarchy (Same for both):**
```
Enemy_[Color] (GameObject)
  ├─ EnemyController (Script Component)
  ├─ SpriteRenderer (Component)
  └─ Transform (Component)
```

**Setup Instructions:**

1. **Create the GameObject:**
   - In Hierarchy, right-click → Create Empty
   - Rename to "Enemy_Red" or "Enemy_Purple"

2. **Add Components:**
   - Add Component → EnemyController
   - Add Component → Sprite Renderer

3. **Configure SpriteRenderer:**
   - **For Enemy_Red:**
     - Color: Red (RGB: 255, 0, 0) or Color(1, 0, 0, 1)
   - **For Enemy_Purple:**
     - Color: Purple (RGB: 160, 32, 240) or Color(0.627f, 0.125f, 0.941f)
   - Sorting Order: 1 (above background, below player)

4. **Create Sprite:**
   - Use a circle sprite (same as player or slightly different)
   - Or use a colored quad

5. **Configure EnemyController Script:**
   - Enemy Type: Red or Purple (from dropdown)
   - Position: (0, 0) - will be set by LevelManager
   - Is Active:
     - **Red:** true (chases immediately)
     - **Purple:** false (dormant, activates on player approach)
   - Is Dormant: (auto-synced with Is Active)
   - Move Animation Speed: 0.1

6. **Save as Prefab:**
   - Drag Enemy_Red/Enemy_Purple to Assets/Prefabs
   - Delete from Hierarchy

**Inspector Settings (Enemy_Red):**
```
Enemy_Red (Prefab)
├─ Transform
│  ├─ Position: (0, 0, 0)
│  ├─ Scale: (0.25, 0.25, 0.25)
│  └─ Rotation: (0, 0, 0)
├─ Sprite Renderer
│  ├─ Sprite: (Circle or custom)
│  ├─ Color: Red
│  └─ Sorting Order: 1
└─ EnemyController (Script)
   ├─ Enemy Type: Red
   ├─ Position: (0, 0)
   ├─ Is Active: true
   ├─ Is Dormant: false
   └─ Move Animation Speed: 0.1
```

**Inspector Settings (Enemy_Purple):**
```
Enemy_Purple (Prefab)
├─ Transform
│  ├─ Position: (0, 0, 0)
│  ├─ Scale: (0.25, 0.25, 0.25)
│  └─ Rotation: (0, 0, 0)
├─ Sprite Renderer
│  ├─ Sprite: (Circle or custom)
│  ├─ Color: Purple (dimmed: half brightness)
│  └─ Sorting Order: 1
└─ EnemyController (Script)
   ├─ Enemy Type: Purple
   ├─ Position: (0, 0)
   ├─ Is Active: false (DORMANT)
   ├─ Is Dormant: true
   └─ Move Animation Speed: 0.1
```

---

### 3. Obstacle (Tree) Prefab

**File Path:** `Assets/Prefabs/Obstacle.prefab`

**Setup Instructions:**

1. **Create the GameObject:**
   - Right-click in Hierarchy → Create Empty
   - Rename to "Obstacle"

2. **Add Component:**
   - Add Component → Sprite Renderer

3. **Configure SpriteRenderer:**
   - Sprite: Dark green tree or simple rectangle
   - Color: Dark Green (RGB: 34, 139, 34)
   - Sorting Order: 0 (background level)

4. **Create Sprite:**
   - Use a tree-like sprite
   - Or a simple dark green square/rectangle

5. **Scale:**
   - Scale: (0.8, 0.8, 0.1) - fill most of cell

6. **Save as Prefab:**
   - Drag to Assets/Prefabs/Obstacle
   - Delete from Hierarchy

**Inspector Settings:**
```
Obstacle (Prefab)
├─ Transform
│  ├─ Position: (0, 0, 0)
│  ├─ Scale: (0.8, 0.8, 0.1)
│  └─ Rotation: (0, 0, 0)
└─ Sprite Renderer
   ├─ Sprite: (Dark green tree)
   ├─ Color: Dark Green
   └─ Sorting Order: 0
```

---

### 4. Mud Patch Prefab

**File Path:** `Assets/Prefabs/Mud.prefab`

**Setup Instructions:**

1. **Create the GameObject:**
   - Right-click → Create Empty
   - Rename to "Mud"

2. **Add Component:**
   - Add Component → Sprite Renderer

3. **Configure SpriteRenderer:**
   - Sprite: Brown/amber circular patch
   - Color: Amber/Brown (RGB: 191, 144, 0)
   - Sorting Order: 0

4. **Scale:**
   - Scale: (0.6, 0.6, 0.1) - slightly smaller than cells

5. **Save as Prefab:**
   - Drag to Assets/Prefabs/Mud
   - Delete from Hierarchy

**Inspector Settings:**
```
Mud (Prefab)
├─ Transform
│  ├─ Position: (0, 0, 0)
│  ├─ Scale: (0.6, 0.6, 0.1)
│  └─ Rotation: (0, 0, 0)
└─ Sprite Renderer
   ├─ Sprite: (Brown circular patch)
   ├─ Color: Amber
   └─ Sorting Order: 0
```

---

### 5. Goal (Sacred Grove) Prefab

**File Path:** `Assets/Prefabs/Goal.prefab`

**Setup Instructions:**

1. **Create the GameObject:**
   - Right-click → Create Empty
   - Rename to "Goal"

2. **Add Component:**
   - Add Component → Sprite Renderer

3. **Configure SpriteRenderer:**
   - Sprite: Green square or mystical symbol
   - Color: Bright Green (RGB: 0, 255, 0)
   - Sorting Order: 0 (background)

4. **Create Sprite:**
   - Simple green square
   - Or mystical circle/glow effect

5. **Scale:**
   - Scale: (0.9, 0.9, 0.1) - fill most of cell

6. **Add Glow (Optional):**
   - Add a second sprite child with larger scale and transparency
   - Creates a "glowing goal" effect

7. **Save as Prefab:**
   - Drag to Assets/Prefabs/Goal
   - Delete from Hierarchy

**Inspector Settings:**
```
Goal (Prefab)
├─ Transform
│  ├─ Position: (0, 0, 0)
│  ├─ Scale: (0.9, 0.9, 0.1)
│  └─ Rotation: (0, 0, 0)
└─ Sprite Renderer
   ├─ Sprite: (Green square)
   ├─ Color: Bright Green
   └─ Sorting Order: 0
```

---

## Scene Setup

### Folder Structure (Create These)
```
Assets/
├─ Scripts/
│  ├─ GameManager.cs
│  ├─ PlayerController.cs
│  ├─ EnemyController.cs
│  ├─ GridHelper.cs
│  ├─ LevelData.cs
│  ├─ LevelManager.cs
│  ├─ UIManager.cs
│  └─ GridRenderer.cs
├─ Prefabs/
│  ├─ Player.prefab
│  ├─ Enemy_Red.prefab
│  ├─ Enemy_Purple.prefab
│  ├─ Obstacle.prefab
│  ├─ Mud.prefab
│  └─ Goal.prefab
├─ Scenes/
│  ├─ Game.unity (Main game scene)
│  ├─ MainMenu.unity (Optional)
│  └─ LevelSelect.unity (Optional)
└─ ScriptableObjects/
   └─ Levels/
      ├─ Level_01.asset
      ├─ Level_02.asset
      └─ (more levels...)
```

---

## Complete Step-by-Step Tutorial

### Part 1: Create the Game Scene

**Step 1.1: Create Main Game Scene**
1. File → New Scene
2. Save as `Assets/Scenes/Game.unity`

**Step 1.2: Setup Camera**
1. In Hierarchy, select "Main Camera"
2. Set Position: (0, 0, -10)
3. Set Size: 5 (orthographic)
4. Background Color: Dark (nearly black) for atmospheric feel
5. Set Projection: Orthographic
6. Clear Flags: Solid Color

**Step 1.3: Create GameManager GameObject**
1. Right-click Hierarchy → Create Empty
2. Rename to "GameManager"
3. Position: (0, 0, 0)
4. Add Component → GameManager (script)
5. Tag it as "GameManager"

**Step 1.4: Create LevelManager GameObject**
1. Right-click Hierarchy → Create Empty
2. Rename to "LevelManager"
3. Position: (0, 0, 0)
4. Add Component → LevelManager (script)
5. Inspector settings:
   - Player Prefab: Drag Player.prefab here
   - Enemy Prefab: Drag Enemy_Red.prefab here
   - Obstacle Prefab: Drag Obstacle.prefab here
   - Mud Prefab: Drag Mud.prefab here
   - Goal Prefab: Drag Goal.prefab here
   - Levels: (leave empty for now, we'll add test level)

**Step 1.5: Create UI Canvas**
1. Right-click Hierarchy → UI → Canvas
2. Rename to "GameUI"
3. Add Component → UIManager (script)
4. Set Canvas Scaler:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920 x 1080

**Step 1.6: Create UI Elements (in Canvas)**

Create these child objects under Canvas:

**Level Header Text:**
1. Right-click Canvas → TextMeshPro - Text
2. Rename to "LevelHeader"
3. Set Text: "Level 1: Test Level"
4. Position: Top-center
5. Font Size: 36

**Enemy Counter Text:**
1. Right-click Canvas → TextMeshPro - Text
2. Rename to "EnemyCounter"
3. Set Text: "Enemies: 3"
4. Position: Top-right
5. Font Size: 24

**Message Text:**
1. Right-click Canvas → TextMeshPro - Text
2. Rename to "MessageText"
3. Set Text: ""
4. Position: Center
5. Font Size: 48

**Reset Button:**
1. Right-click Canvas → Button - TextMeshPro
2. Rename to "ResetButton"
3. Set Button Text: "Reset"
4. Position: Bottom-left

**Next Level Button:**
1. Right-click Canvas → Button - TextMeshPro
2. Rename to "NextButton"
3. Set Button Text: "Next"
4. Position: Bottom-right

**Previous Level Button:**
1. Right-click Canvas → Button - TextMeshPro
2. Rename to "PrevButton"
3. Set Button Text: "Prev"
4. Position: Bottom-center-left

**Win Panel (Background):**
1. Right-click Canvas → Panel - Image
2. Rename to "WinPanel"
3. Color: Black with transparency
4. Position: Full screen
5. Add child Text: "YOU WIN!"
6. Inactive by default (uncheck in Inspector)

**Lose Panel (Background):**
1. Right-click Canvas → Panel - Image
2. Rename to "LosePanel"
3. Color: Red with transparency
4. Position: Full screen
5. Add child Text: "YOU LOST!"
6. Inactive by default (uncheck in Inspector)

**Step 1.7: Configure UIManager**
1. Select UIManager component
2. Drag LevelHeader TextMeshPro to "Level Header Text"
3. Drag EnemyCounter TextMeshPro to "Enemy Counter Text"
4. Drag MessageText TextMeshPro to "Message Text"
5. Drag ResetButton Button to "Reset Button"
6. Drag NextButton Button to "Next Level Button"
7. Drag PrevButton Button to "Previous Level Button"
8. Drag WinPanel Image to "Win Panel"
9. Drag LosePanel Image to "Lose Panel"

**Step 1.8: Setup Grid Renderer (Visual Debug)**
1. Right-click Hierarchy → Create Empty
2. Rename to "GridRenderer"
3. Position: (0, 0, 0)
4. Add Component → GridRenderer (script)
5. This draws the 5x5 grid as lines (for debugging)

---

### Part 2: Create Test Level Data

**Step 2.1: Create LevelData ScriptableObject**
1. Right-click in Assets/ScriptableObjects/Levels
2. Create → Will-o'-the-Wisp → Level Data
3. Rename to "Level_01"
4. Open Inspector

**Step 2.2: Configure Level_01**
```
Level Name: "The First Steps"
Level Number: 1
Player Start: (0, 0)
Goal Position: (4, 4)
Obstacles: (empty for now)
Mud Patches: (empty for now)
Enemies: Size 2
  [0]: Position (2, 2), Type Red, Dormant false
  [1]: Position (3, 2), Type Red, Dormant false
Min Moves: 4
```

**Step 2.3: Add to LevelManager**
1. Select LevelManager in scene
2. In Inspector, find "Levels" array
3. Set Size: 1
4. Drag Level_01.asset into Element 0

---

### Part 3: Wire Up LevelManager References

**Step 3.1: Assign Prefabs to LevelManager**
1. Select LevelManager in Hierarchy
2. In Inspector:
   - **Player Prefab:** Drag Player.prefab
   - **Enemy Prefab:** Drag Enemy_Red.prefab
   - **Obstacle Prefab:** Drag Obstacle.prefab
   - **Mud Prefab:** Drag Mud.prefab
   - **Goal Prefab:** Drag Goal.prefab

**Step 3.2: Wire Button Callbacks**
1. Select ResetButton
2. In Inspector, Button component → On Click
3. Click "+" to add callback
4. Drag UIManager into object field
5. From dropdown: UIManager → OnResetClicked()

Repeat for:
- NextButton → OnNextLevelClicked()
- PrevButton → OnPreviousLevelClicked()

---

### Part 4: Configure Player Spawning

**Step 4.1: Add Initial Player to Scene**
1. In Hierarchy, create empty GameObject "Player_Init"
2. Add PlayerController component
3. Set Position: (0, 0, 0)
4. Set position in script: (0, 0)
5. Save scene

**Alternative:** Let LevelManager instantiate from prefab (better approach)

---

### Part 5: Test the Setup

**Step 5.1: Play in Editor**
1. Press Play button
2. You should see:
   - Empty 5x5 grid with gizmo lines
   - Player cyan circle at (0,0)
   - Two red enemies at (2,2) and (3,2)
   - Green goal square at (4,4)
   - UI showing "Enemies: 2"

**Step 5.2: Test Controls**
1. Press Arrow Keys or WASD to move player
2. Enemies should move toward player
3. Press R to reset level
4. Check that win/lose conditions work:
   - Try to get caught by enemy (should show "LOST")
   - Try to lure both enemies to goal (should show "WON")

---

## Testing Checklist

### Core Gameplay
- [ ] Player can move in all 8 directions (arrow keys + WASD)
- [ ] Player cannot move out of bounds
- [ ] Player cannot move through enemies
- [ ] Player cannot move through obstacles
- [ ] Enemies move toward player each turn
- [ ] Enemies only move cardinal directions (not diagonals)
- [ ] Enemies stop moving when blocked by obstacle
- [ ] Enemies stop moving when blocked by different-colored enemy

### Advanced Mechanics
- [ ] Same-colored enemies merge when on same square
- [ ] Player win when all enemies on goal
- [ ] Player lose when enemy on player position
- [ ] Red enemies chase immediately
- [ ] Purple enemies activate when player adjacent
- [ ] Grid lines display correctly (5x5)
- [ ] Animations are smooth (0.1 second duration)

### UI Functionality
- [ ] Level header displays correctly
- [ ] Enemy counter updates
- [ ] Win screen appears on victory
- [ ] Lose screen appears on defeat
- [ ] Reset button restarts level
- [ ] Next button progresses to next level
- [ ] Previous button goes to previous level

### Visual Feedback
- [ ] Player appears as cyan circle
- [ ] Red enemies appear as red circles
- [ ] Purple enemies appear dimmed purple circles
- [ ] Obstacles appear as dark green
- [ ] Goal appears as bright green
- [ ] Enemies animate when moving
- [ ] Player animates when moving

---

## Troubleshooting

### Common Issues & Solutions

**"PlayerController not found in scene!"**
- Ensure Player.prefab is instantiated in scene
- Or add Player manually to Hierarchy before running

**Enemies not moving**
- Check that EnemyController script is attached
- Verify gameState is "Playing" in GameManager
- Check enemy position is within bounds

**Player can't move**
- Check canMove flag (should be true)
- Verify gameState is "Playing"
- Check Input.GetKeyDown is being called

**Prefabs not instantiating**
- Verify LevelManager has all prefab references set
- Check level has valid configuration
- Look for errors in Console

**UI elements not showing**
- Verify Canvas is in scene
- Check UIManager has all field references
- Ensure Canvas Scaler is configured

**Grid lines not visible**
- GridRenderer draws in Gizmos only
- Must have Gizmos enabled in Scene view (top-right menu)
- Set grid offset correctly (-2, -2, 0)

---

## Next Steps

After getting the basic prototype working:

1. **Create More Levels:**
   - Design levels with different enemy configurations
   - Add obstacles and mud patches
   - Test solvability

2. **Add Visual Polish:**
   - Create proper sprite assets
   - Add particle effects for elimination
   - Sound effects and music

3. **Expand Features:**
   - Move counter / par system
   - Undo system
   - Level select screen

4. **Optimize Performance:**
   - Profile frame rate
   - Optimize pathfinding if needed
   - Test on different devices

---

## File Checklist

✅ Scripts Created:
- GameManager.cs
- PlayerController.cs
- EnemyController.cs
- GridHelper.cs
- LevelData.cs
- LevelManager.cs
- UIManager.cs
- GridRenderer.cs

✅ Prefabs to Create:
- Player.prefab
- Enemy_Red.prefab
- Enemy_Purple.prefab
- Obstacle.prefab
- Mud.prefab
- Goal.prefab

✅ Scenes to Create:
- Game.unity (main gameplay)

✅ ScriptableObjects to Create:
- Level_01.asset (test level)

---

## Quick Reference: Component Setup

### PlayerController Inspector
```
Position: (varies)
Can Move: true
Move Animation Speed: 0.1
```

### EnemyController Inspector
```
Enemy Type: (Red or Purple)
Position: (varies)
Is Active: (true for red, false for purple)
Is Dormant: (opposite of Is Active)
Move Animation Speed: 0.1
```

### GameManager Inspector
```
Game State: Playing
Current Level: 0
(Other fields auto-populated)
```

### LevelManager Inspector
```
Levels: (array of LevelData)
Player Prefab: Player.prefab
Enemy Prefab: Enemy_Red.prefab
Obstacle Prefab: Obstacle.prefab
Mud Prefab: Mud.prefab
Goal Prefab: Goal.prefab
```

### LevelData Inspector
```
Level Name: "string"
Level Number: int
Player Start: Vector2Int
Goal Position: Vector2Int
Enemies: EnemySpawn[]
Obstacles: Vector2Int[]
Mud Patches: Vector2Int[]
Min Moves: int
```

