# Will-o'-the-Wisp - Complete Setup Checklist

All-in-one checklist for setting up the Will-o'-the-Wisp game with error logging.

---

## Phase 1: Project Foundation (Before Error Logger)

- [ ] Unity 2022.3 LTS or newer installed
- [ ] WISP project created and ready
- [ ] 8 game scripts in place (GameManager, PlayerController, etc.)
- [ ] GDD document reviewed and understood

**Documentation:** See `SCRIPT_DOCUMENTATION.md` and `QUICK_SETUP_CHECKLIST.md`

---

## Phase 2: Game Setup in Unity

- [ ] Create Game.unity scene
- [ ] Add GameManager (empty GameObject + script)
- [ ] Add LevelManager (empty GameObject + script)
- [ ] Add GridRenderer (empty GameObject + script)
- [ ] Create Canvas with UI elements
- [ ] Create all 6 prefabs (Player, Enemy_Red, Enemy_Purple, Obstacle, Mud, Goal)
- [ ] Create first test level (Level_01.asset)
- [ ] Configure LevelManager with prefab references
- [ ] Configure UIManager button callbacks

**Documentation:** See `PREFABS_AND_SETUP_GUIDE.md` and `QUICK_SETUP_CHECKLIST.md`

**Estimated Time:** 45 minutes

---

## Phase 3: Error Logger Setup ⭐ NEW

### Step 1: Add ErrorLogger to Scene
- [ ] Open Game.unity scene
- [ ] Right-click in Hierarchy → Create Empty
- [ ] Rename to "ErrorLogger"
- [ ] In Inspector: Add Component → Scripts → ErrorLogger
- [ ] Verify settings are enabled:
  - [ ] Log Messages ✓
  - [ ] Log Warnings ✓
  - [ ] Log Errors ✓
  - [ ] Include Stack Trace ✓
- [ ] Save the scene

### Step 2: Start Python Monitor
- [ ] Open Terminal
- [ ] Navigate to project: `cd /Users/siraj/WISP`
- [ ] Run monitor: `python3 unity_error_monitor.py`
- [ ] Verify output shows:
  - [ ] "Initialized" message
  - [ ] Log file path
  - [ ] Debug directory path
  - [ ] "Waiting for logs..." message
- [ ] Keep terminal window open

### Step 3: Test the System
- [ ] Click Play in Unity
- [ ] Game starts with ErrorLogger active
- [ ] Intentionally create an error OR wait for one naturally
- [ ] Verify in Python terminal:
  - [ ] Error is detected and logged
  - [ ] Debug file created
  - [ ] File path shown in terminal
- [ ] Verify Cursor opens automatically with error file
  - [ ] If not: Manually check `Logs/cursor_debug/` folder
- [ ] Read error details and stack trace

**Estimated Time:** 5 minutes

---

## Phase 4: Verification Tests

### Basic Functionality
- [ ] Player can move in all 8 directions
- [ ] Enemies move toward player
- [ ] Same-colored enemies merge
- [ ] Win/lose conditions work correctly
- [ ] Reset button works
- [ ] Next/Previous buttons work

### Error Logger Functionality
- [ ] ErrorLogger GameObject exists in scene
- [ ] ErrorLogger script shows in Inspector
- [ ] Python monitor runs without errors
- [ ] `Logs/` directory created automatically
- [ ] `Logs/unity_errors.json` file created
- [ ] Errors are captured to JSON
- [ ] Python monitor detects new errors
- [ ] Debug files created in `Logs/cursor_debug/`
- [ ] Cursor opens automatically on error (or files available for manual opening)

### Integration Tests
- [ ] All game systems work with ErrorLogger active
- [ ] No performance impact from logging
- [ ] ErrorLogger persists across scene changes
- [ ] Old errors don't accumulate indefinitely (max 100 kept)

**Estimated Time:** 10 minutes

---

## Phase 5: Customization (Optional)

### Python Monitor Customization
- [ ] Review `unity_error_monitor.py` configuration options
- [ ] Adjust check interval if needed (default: 1 second)
- [ ] Adjust filter types if needed (default: Error, Exception, Assert)
- [ ] Test custom configuration

### Unity Configuration
- [ ] Review ErrorLogger Inspector settings
- [ ] Adjust maxLogs constant if needed (default: 100)
- [ ] Add custom error logging to relevant scripts
  - Example: `ErrorLogger.LogCustomError("Title", "Description");`

### Directory Organization (Optional)
- [ ] Create subdirectories in `Logs/` for different test runs
- [ ] Organize debug files by date/session
- [ ] Archive old error logs

---

## Phase 6: Integration with Workflow

### Development Workflow
- [ ] Always start Python monitor before opening Unity
- [ ] Keep Python monitor window visible
- [ ] Monitor runs continuously while developing
- [ ] When error occurs:
  - [ ] Check Python terminal for detection
  - [ ] Open Cursor debug file
  - [ ] Ask Claude Code for help debugging
  - [ ] Fix issue and test again

### Team Workflow (if applicable)
- [ ] Document error logger in team guidelines
- [ ] Set up shared logging location (if needed)
- [ ] Create naming convention for debug files
- [ ] Establish error review process

### Continuous Integration (if applicable)
- [ ] ErrorLogger doesn't interfere with CI/CD
- [ ] Logs are cleaned up between test runs
- [ ] Error logger can be disabled for automated tests
- [ ] CI/CD can parse `unity_errors.json` for reporting

---

## Phase 7: Documentation Review

### Required Reading
- [ ] ERROR_LOGGER_QUICKSTART.md (2-minute overview)
- [ ] ERROR_LOGGER_INTEGRATION.md (complete setup guide)

### Recommended Reading
- [ ] SCRIPT_DOCUMENTATION.md (understand game code)
- [ ] ARCHITECTURE_AND_DIAGRAMS.md (understand design)
- [ ] PREFABS_AND_SETUP_GUIDE.md (understand setup)

### Reference Documents
- [ ] QUICK_SETUP_CHECKLIST.md (while setting up)
- [ ] README.md (documentation index)
- [ ] GDD.pdf (game vision)

---

## Success Criteria ✅

After completing all phases, you should have:

### Game Setup
- [x] All 8 scripts in place and compiling
- [x] All 6 prefabs created
- [x] Game scene fully configured
- [x] At least one test level
- [x] UI fully functional
- [x] Game is playable

### Error Logger Setup
- [x] ErrorLogger.cs added to project
- [x] ErrorLogger GameObject in Game scene
- [x] Python monitor script in project root
- [x] Python monitor runs without errors
- [x] Logs directory auto-created
- [x] Errors are captured to JSON
- [x] Debug files are created
- [x] Cursor opens with errors (or manual opening works)

### Documentation
- [x] All documentation files created
- [x] README.md updated with error logger
- [x] Setup instructions clear and tested
- [x] Quick start guide available
- [x] Integration guide available

---

## 🎯 File Checklist

### Code Files
- [x] Assets/Scripts/GameManager.cs (game logic)
- [x] Assets/Scripts/PlayerController.cs (player input)
- [x] Assets/Scripts/EnemyController.cs (enemy AI)
- [x] Assets/Scripts/GridHelper.cs (utilities)
- [x] Assets/Scripts/LevelData.cs (level configuration)
- [x] Assets/Scripts/LevelManager.cs (level loading)
- [x] Assets/Scripts/UIManager.cs (UI management)
- [x] Assets/Scripts/GridRenderer.cs (debug visualization)
- [x] Assets/Scripts/ErrorLogger.cs (error capture) ⭐ NEW

### Prefab Files
- [ ] Assets/Prefabs/Player.prefab
- [ ] Assets/Prefabs/Enemy_Red.prefab
- [ ] Assets/Prefabs/Enemy_Purple.prefab
- [ ] Assets/Prefabs/Obstacle.prefab
- [ ] Assets/Prefabs/Mud.prefab
- [ ] Assets/Prefabs/Goal.prefab

### Scene Files
- [ ] Assets/Scenes/Game.unity (with ErrorLogger GameObject)

### Level Files
- [ ] Assets/ScriptableObjects/Levels/Level_01.asset
- [ ] (Create more as you develop)

### Documentation Files
- [x] Assets/README.md (documentation index) ✏️ UPDATED
- [x] Assets/SCRIPT_DOCUMENTATION.md (code documentation)
- [x] Assets/PREFABS_AND_SETUP_GUIDE.md (setup guide)
- [x] Assets/QUICK_SETUP_CHECKLIST.md (quick reference)
- [x] Assets/ARCHITECTURE_AND_DIAGRAMS.md (architecture)
- [x] Assets/ERROR_LOGGER_INTEGRATION.md (error logger guide) ⭐ NEW
- [x] ERROR_LOGGER_QUICKSTART.md (quick start) ⭐ NEW
- [x] IMPLEMENTATION_SUMMARY.md (what was implemented) ⭐ NEW
- [x] SETUP_CHECKLIST.md (this file) ⭐ NEW
- [x] unity_error_monitor.py (Python monitor) ⭐ NEW

### Auto-Created Directories
- [ ] Logs/ (created when ErrorLogger runs)
- [ ] Logs/cursor_debug/ (created when errors occur)

---

## 📊 Progress Tracking

| Phase | Item | Status | Notes |
|-------|------|--------|-------|
| 1 | Project Foundation | ✅ | Pre-error logger setup |
| 2 | Game Setup | ✅ | 45 minutes |
| 3 | Error Logger | 🟡 | In progress |
| 4 | Verification | ⏳ | Pending error logger setup |
| 5 | Customization | ⏳ | Optional |
| 6 | Integration | ⏳ | After verification |
| 7 | Documentation | ✅ | All docs created |

---

## 🐛 Troubleshooting Quick Links

**Common Issues:**
- [x] See ERROR_LOGGER_INTEGRATION.md - Troubleshooting section
- [x] See ERROR_LOGGER_QUICKSTART.md - Troubleshooting table
- [x] See QUICK_SETUP_CHECKLIST.md - "If Something Doesn't Work"
- [x] See PREFABS_AND_SETUP_GUIDE.md - Troubleshooting section

**Getting Help:**
1. Check the relevant documentation section
2. Review the error message in Python monitor or Cursor
3. Ask Claude Code: "Help me debug this error"
4. Check console output in Unity

---

## 🚀 Quick Start Path

If you're in a hurry:

1. **Fastest: 2 minutes (Error Logger Only)**
   - [ ] Add ErrorLogger to scene
   - [ ] Run Python monitor
   - [ ] Test with an error

2. **Typical: 50 minutes (Full Game + Logger)**
   - [ ] Follow QUICK_SETUP_CHECKLIST.md (45 min)
   - [ ] Add ErrorLogger setup above (5 min)

3. **Thorough: 2 hours (Everything)**
   - [ ] Read GDD.pdf (30 min)
   - [ ] Follow PREFABS_AND_SETUP_GUIDE.md (60 min)
   - [ ] Add ErrorLogger setup (5 min)
   - [ ] Read ERROR_LOGGER_INTEGRATION.md (15 min)
   - [ ] Test everything (10 min)

---

## ✨ You're Ready!

All components are in place. Your Will-o'-the-Wisp game is ready to build and debug!

**Next Steps:**
1. Check off Phase 3 items as you complete them
2. Test with Phase 4 verification tests
3. Start developing your game
4. Use error logger to catch and fix issues in real-time

**Happy developing! 🎮**

---

## 📝 Notes

- **Python Monitor:** Keep running in dedicated terminal while developing
- **ErrorLogger:** Persists across scenes - no need to re-add it
- **Logs:** Automatically created in project root - don't add to version control
- **Updates:** Return to this checklist as you add new features/levels

---

**Checklist Version:** 1.0
**Last Updated:** February 16, 2025
**Status:** Ready for use ✅
