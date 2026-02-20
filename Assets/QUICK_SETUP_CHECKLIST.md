# Quick Setup Checklist - Will-o'-the-Wisp Prototype

## 5-Minute Quick Start

### Prerequisites
✅ All scripts created in Assets/Scripts/
✅ Unity 2022.3+ LTS installed
✅ Project created and ready

---

## Phase 1: Create Prefabs (15 minutes)

### Prefab 1: Player
```
1. Right-click Hierarchy → Create Empty → Rename "Player"
2. Add Component → PlayerController
3. Add Component → Sprite Renderer
4. Set Sprite Renderer:
   - Sprite: Circle (or simple quad)
   - Color: Cyan (0, 1, 1)
   - Sorting Order: 2
5. Scale: (0.3, 0.3, 0.3)
6. Drag to Assets/Prefabs/ → Save as Prefab
```

### Prefab 2: Enemy_Red
```
1. Right-click Hierarchy → Create Empty → Rename "Enemy_Red"
2. Add Component → EnemyController
3. Add Component → Sprite Renderer
4. Set EnemyController:
   - Enemy Type: Red
   - Is Active: TRUE
5. Set Sprite Renderer:
   - Sprite: Circle
   - Color: Red (1, 0, 0)
   - Sorting Order: 1
6. Scale: (0.25, 0.25, 0.25)
7. Drag to Assets/Prefabs/ → Save as Prefab
```

### Prefab 3: Enemy_Purple
```
1. Right-click Hierarchy → Create Empty → Rename "Enemy_Purple"
2. Add Component → EnemyController
3. Add Component → Sprite Renderer
4. Set EnemyController:
   - Enemy Type: Purple
   - Is Active: FALSE (Dormant)
5. Set Sprite Renderer:
   - Sprite: Circle
   - Color: Purple (0.627, 0.125, 0.941)
   - Sorting Order: 1
6. Scale: (0.25, 0.25, 0.25)
7. Drag to Assets/Prefabs/ → Save as Prefab
```

### Prefab 4: Obstacle
```
1. Right-click Hierarchy → Create Empty → Rename "Obstacle"
2. Add Component → Sprite Renderer
3. Set Sprite Renderer:
   - Sprite: Green square or rectangle
   - Color: Dark Green (0.13, 0.55, 0.13)
   - Sorting Order: 0
4. Scale: (0.8, 0.8, 0.1)
5. Drag to Assets/Prefabs/ → Save as Prefab
```

### Prefab 5: Mud
```
1. Right-click Hierarchy → Create Empty → Rename "Mud"
2. Add Component → Sprite Renderer
3. Set Sprite Renderer:
   - Sprite: Brown/Amber circle
   - Color: Amber (0.75, 0.56, 0)
   - Sorting Order: 0
4. Scale: (0.6, 0.6, 0.1)
5. Drag to Assets/Prefabs/ → Save as Prefab
```

### Prefab 6: Goal
```
1. Right-click Hierarchy → Create Empty → Rename "Goal"
2. Add Component → Sprite Renderer
3. Set Sprite Renderer:
   - Sprite: Green square
   - Color: Bright Green (0, 1, 0)
   - Sorting Order: 0
4. Scale: (0.9, 0.9, 0.1)
5. Drag to Assets/Prefabs/ → Save as Prefab
```

---

## Phase 2: Create Game Scene (10 minutes)

### Scene Setup
```
1. File → New Scene → Save as "Assets/Scenes/Game.unity"

Hierarchy should look like:
├─ Main Camera
│  └─ Position: (0, 0, -10)
│     Size: 5 (Orthographic)
│     Projection: Orthographic
├─ GameManager (Empty)
│  └─ Add Script: GameManager
├─ LevelManager (Empty)
│  └─ Add Script: LevelManager
├─ GridRenderer (Empty)
│  └─ Add Script: GridRenderer
└─ Canvas (UI)
   ├─ LevelHeader (TextMeshPro)
   ├─ EnemyCounter (TextMeshPro)
   ├─ MessageText (TextMeshPro)
   ├─ ResetButton (Button)
   ├─ NextButton (Button)
   ├─ PrevButton (Button)
   ├─ WinPanel (Panel) - inactive
   └─ LosePanel (Panel) - inactive
```

---

## Phase 3: Configure Components (10 minutes)

### LevelManager Configuration
```
1. Select LevelManager in Hierarchy
2. In Inspector:
   - Player Prefab: Drag Player.prefab
   - Enemy Prefab: Drag Enemy_Red.prefab
   - Obstacle Prefab: Drag Obstacle.prefab
   - Mud Prefab: Drag Mud.prefab
   - Goal Prefab: Drag Goal.prefab
   - Levels: Set Size to 1
```

### UIManager Configuration
```
1. Select Canvas → UIManager component
2. Wire up all field references:
   - Level Header Text: LevelHeader
   - Enemy Counter Text: EnemyCounter
   - Message Text: MessageText
   - Reset Button: ResetButton
   - Next Level Button: NextButton
   - Previous Level Button: PrevButton
   - Win Panel: WinPanel
   - Lose Panel: LosePanel
```

### Button Click Events
```
1. Select ResetButton
2. Button → On Click → Add Callback
3. Drag UIManager → Select UIManager.OnResetClicked()
4. Repeat for NextButton → OnNextLevelClicked()
5. Repeat for PrevButton → OnPreviousLevelClicked()
```

---

## Phase 4: Create Test Level (5 minutes)

### Create Level ScriptableObject
```
1. Right-click Assets/ScriptableObjects/Levels
2. Create → Will-o'-the-Wisp → Level Data
3. Rename to "Level_01"
4. Configure:
   - Level Name: "The First Steps"
   - Level Number: 1
   - Player Start: (0, 0)
   - Goal Position: (4, 4)
   - Enemies: Size 2
     [0] Position: (2, 2), Type: Red, Dormant: false
     [1] Position: (3, 2), Type: Red, Dormant: false
   - Obstacles: (empty)
   - Mud Patches: (empty)
   - Min Moves: 4
```

### Add Level to LevelManager
```
1. Select LevelManager
2. Levels Array → Element 0: Drag Level_01.asset
```

---

## Phase 5: Test (5 minutes)

### Play Testing
```
1. Press Play in Editor
2. You should see:
   ✓ 5x5 grid with gizmo lines
   ✓ Cyan player at top-left
   ✓ 2 red enemies in middle
   ✓ Green goal at bottom-right
   ✓ UI showing "Enemies: 2"

3. Test Controls:
   ✓ Arrow keys move player
   ✓ WASD also works
   ✓ Enemies follow player
   ✓ Press R to reset

4. Test Win/Lose:
   ✓ Get caught = Red screen "YOU LOST!"
   ✓ Lure both to goal = Green screen "YOU WIN!"
```

---

## Common Colors (RGB)

| Color | R | G | B | Unity |
|---|---|---|---|---|
| Cyan (Player) | 0 | 255 | 255 | (0, 1, 1) |
| Red (Enemy) | 255 | 0 | 0 | (1, 0, 0) |
| Purple (Enemy) | 160 | 32 | 240 | (0.627, 0.125, 0.941) |
| Dark Green (Tree) | 34 | 139 | 34 | (0.13, 0.55, 0.13) |
| Amber (Mud) | 191 | 144 | 0 | (0.75, 0.56, 0) |
| Green (Goal) | 0 | 255 | 0 | (0, 1, 0) |
| Dark BG | 20 | 20 | 25 | (0.08, 0.08, 0.1) |

---

## If Something Doesn't Work

| Problem | Solution |
|---|---|
| "PlayerController not found" | Add Player prefab instance to scene or check instantiation |
| Enemies don't move | Check gameState is "Playing" and EnemyController attached |
| Player can't move | Verify canMove is true and gameState is "Playing" |
| Buttons don't work | Verify UI callbacks are wired correctly |
| Grid not visible | Enable Gizmos in Scene view (top-right menu) |
| Enemies not spawning | Check LevelManager has Enemy_Red prefab assigned |
| Wrong colors | Verify SpriteRenderer color values match table above |

---

## Keyboard Controls Reference

| Input | Action |
|---|---|
| ↑ or W | Move Up |
| ↓ or S | Move Down |
| ← or A | Move Left |
| → or D | Move Right |
| Q | Move Up-Left |
| E | Move Up-Right |
| Z | Move Down-Left |
| C | Move Down-Right |
| R | Reset Level |

---

## File Structure Created

```
Assets/
├── Scripts/ (8 scripts)
├── Prefabs/
│   ├── Player.prefab
│   ├── Enemy_Red.prefab
│   ├── Enemy_Purple.prefab
│   ├── Obstacle.prefab
│   ├── Mud.prefab
│   └── Goal.prefab
├── Scenes/
│   └── Game.unity
└── ScriptableObjects/Levels/
    └── Level_01.asset
```

---

## Success Criteria ✓

When complete, you should have:

- [x] All 8 scripts in place and no compile errors
- [x] 6 prefabs created with correct components
- [x] Game scene with GameManager, LevelManager, UIManager
- [x] Canvas with all UI elements
- [x] One test level (Level_01)
- [x] Able to play and move player with keyboard
- [x] Enemies chase player each turn
- [x] Can win (all enemies on goal) and lose (caught by enemy)
- [x] UI updates correctly
- [x] Reset/Next/Prev buttons work

---

## Pro Tips 💡

1. **Use Unity's built-in sprites** for quick prototyping
2. **Duplicate prefabs** to create variations (e.g., Enemy_Green)
3. **Test early** - play after each step to catch issues
4. **Use Gizmos** to visualize grid for debugging
5. **Check Console** for error messages if something fails
6. **Inspector Presets** - save component settings for reuse
7. **Prefab variants** - for quick level design

---

## Next Level (Optional)

After basic prototype works:
- [ ] Add Move Counter
- [ ] Implement Par System (1/2/3 stars)
- [ ] Add particle effects for enemy elimination
- [ ] Create more levels with varied difficulty
- [ ] Add obstacle placement guidance
- [ ] Sound effects library
- [ ] Main menu scene
- [ ] Level select screen

---

**You're ready to build! 🎮**

Follow the phases in order, test after each phase, and you'll have a working prototype in 45 minutes!

