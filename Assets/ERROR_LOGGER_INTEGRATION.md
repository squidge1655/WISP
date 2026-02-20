# Error Logger Integration - Will-o'-the-Wisp

Complete guide for setting up real-time error monitoring in your Will-o'-the-Wisp game.

## 📋 Overview

The error logging system consists of two parts:

1. **ErrorLogger.cs** - Unity script that captures all console logs to JSON
2. **unity_error_monitor.py** - Python script that monitors logs and opens errors in Cursor

When enabled, any error or exception in your game will automatically open in Cursor for debugging.

---

## 🚀 Setup Instructions

### Step 1: Add ErrorLogger to Your Scene

1. Open your Game scene in Unity (Assets/Scenes/Game.unity)
2. In the Hierarchy, right-click → Create Empty
3. Name the new GameObject: **ErrorLogger**
4. In the Inspector, click Add Component → Scripts → ErrorLogger
5. Leave the default settings (all logging types enabled)
6. Save the scene

**Result:** ErrorLogger now captures all logs, warnings, and errors to `Logs/unity_errors.json`

### Step 2: Start the Python Monitor (macOS/Linux)

1. Open Terminal
2. Navigate to your project root:
   ```bash
   cd /Users/siraj/WISP
   ```

3. Run the monitor:
   ```bash
   python3 unity_error_monitor.py
   ```

   You should see:
   ```
   [UnityErrorMonitor] Initialized
     📁 Log file: /Users/siraj/WISP/Logs/unity_errors.json
     📁 Debug dir: /Users/siraj/WISP/Logs/cursor_debug
     ⏱️  Check interval: 1s
     🎯 Filters: Errors, Exceptions, Assertions
     ⏳ Waiting for logs...
   ```

   **Keep this terminal window open while developing!**

### Step 3: Play Your Game

1. In Unity, click Play
2. Trigger an error (intentionally or naturally)
3. The Python monitor detects the error in real-time
4. A debug file is created in `Logs/cursor_debug/`
5. **Cursor automatically opens** with the error details
6. Ask Claude Code: "Help me debug this error"

---

## 🎮 Example Workflow

### Scenario: Player Movement Bug

1. **You encounter a bug** - Player can't move properly
2. **Unity throws an error** - Maybe a NullReferenceException
3. **ErrorLogger captures it** - Adds to `unity_errors.json`
4. **Python monitor detects it** - Creates debug file
5. **Cursor opens automatically** - Shows error with full stack trace
6. **You ask Claude Code:**
   ```
   "Help me debug this NullReferenceException in PlayerController"
   ```
7. **Claude analyzes** the stack trace and suggests fixes
8. **You fix the code** and test again in Unity
9. **Repeat** until bug is fixed

---

## 📊 What Gets Logged

### Automatically Captured:
- ✅ Debug.Log() - Regular messages
- ✅ Debug.LogWarning() - Warnings
- ✅ Debug.LogError() - Errors
- ✅ Unity exceptions - NullReferenceException, IndexOutOfRangeException, etc.

### Stored in JSON:
Each log contains:
- **timestamp** - Exact time with milliseconds
- **logType** - Type of log (Error, Warning, Log, etc.)
- **message** - The error message
- **stackTrace** - Full call stack for debugging
- **scene** - Active scene when error occurred

### Sent to Cursor:
Only these types trigger automatic Cursor opening:
- 🚨 Errors
- 🚨 Exceptions
- 🚨 Assertions

(Warnings and regular logs are only printed to console)

---

## 🔧 Configuration

### In Unity (Inspector Settings)

Select the **ErrorLogger** GameObject to adjust what gets logged:

| Setting | Purpose |
|---------|---------|
| Log Messages | Capture Debug.Log() calls (default: enabled) |
| Log Warnings | Capture warnings (default: enabled) |
| Log Errors | Capture errors and exceptions (default: enabled) |
| Include Stack Trace | Include full stack trace (default: enabled) |

### In Python (Optional Customization)

Edit `unity_error_monitor.py` to customize behavior:

**Change what types trigger Cursor opening:**
```python
# Line 156 - Add 'Warning' to send warnings too
if log_type in ['Error', 'Exception', 'Assert', 'Warning']:
```

**Change how often monitor checks for new logs:**
```python
# Line 177 - Check more frequently (0.5 seconds instead of 1)
monitor = UnityErrorMonitor(log_file, check_interval=0.5)
```

**Change maximum stored logs:**
Edit `ErrorLogger.cs` line 39:
```csharp
private const int maxLogs = 200;  // Store up to 200 logs instead of 100
```

---

## 💡 Best Practices

1. **Always have the Python monitor running** while developing
   - Use a dedicated terminal window for it
   - Keep it visible so you see errors happen

2. **Check the Inspector** on ErrorLogger if logs aren't appearing
   - Verify the script is attached to a GameObject
   - Ensure log types are enabled

3. **Review `Logs/cursor_debug/` folder** periodically
   - Contains timestamped error debug files
   - Useful for reviewing historical errors
   - Safe to delete old files to save space

4. **Use ErrorLogger.LogCustomError()** for manual logging:
   ```csharp
   ErrorLogger.LogCustomError("Player Health Invalid", "Health was negative: " + health);
   ```

5. **Clear logs during level transitions** to keep file manageable:
   ```csharp
   ErrorLogger.ClearLogs();  // Clears all stored logs
   ```

---

## 🐛 Troubleshooting

### Issue: "Python monitor says 'Waiting for logs...' but nothing happens"

**Solutions:**
- Verify ErrorLogger GameObject is in your scene
- Check that the ErrorLogger script is enabled in Inspector
- Ensure you've saved the scene
- Click Play in Unity to generate logs
- Check that `Logs/` directory was created (it should auto-create)

### Issue: "Cursor doesn't open automatically"

**Solutions:**
- Check that Cursor is installed in `/Applications/Cursor.app` (macOS)
- Debug file is still created in `Logs/cursor_debug/` - open manually
- Verify Python monitor is running in the correct directory

### Issue: "Log file grows too large"

**Solutions:**
- Clear old logs periodically: `ErrorLogger.ClearLogs()`
- Reduce maxLogs constant in ErrorLogger.cs
- Delete old files in `Logs/cursor_debug/` folder

### Issue: "Permission denied when running Python monitor"

**Solutions:**
```bash
# Make script executable again
chmod +x /Users/siraj/WISP/unity_error_monitor.py
```

---

## 📁 File Structure Created

```
WISP/
├── Assets/
│   ├── Scripts/
│   │   └── ErrorLogger.cs (NEW)
│   └── Scenes/
│       └── Game.unity (add ErrorLogger GameObject)
│
├── Logs/ (auto-created by Unity)
│   ├── unity_errors.json (log file)
│   └── cursor_debug/ (debug files)
│
└── unity_error_monitor.py (NEW - in project root)
```

---

## 🎯 Integration with Existing Scripts

The error logger works with all existing scripts without modification:

- **GameManager.cs** - Errors logged automatically
- **PlayerController.cs** - Input validation errors captured
- **EnemyController.cs** - Pathfinding errors captured
- **LevelManager.cs** - Level loading errors captured
- **UIManager.cs** - UI interaction errors captured

**No changes needed!** The logger works through Unity's built-in error callback system.

---

## 🚀 Advanced Usage

### Manually Log Game Events

```csharp
// In any script, log important game events
ErrorLogger.LogCustomError("Level Complete", $"Completed level {currentLevel} in {moveCount} moves");

// Log resource constraints
if (enemyCount < 0)
{
    ErrorLogger.LogCustomError("Invalid Enemy Count", $"Enemy count became negative: {enemyCount}");
}

// Log unexpected states
if (playerPos.x < 0 || playerPos.x > 4)
{
    ErrorLogger.LogCustomError("Out of Bounds", $"Player at invalid position: {playerPos}");
}
```

### Clear Logs During Scene Transitions

```csharp
// In LevelManager.cs, before loading next level
public void NextLevel()
{
    ErrorLogger.ClearLogs();  // Fresh start for new level
    // ... rest of level loading code
}
```

### Check Log Count in UI

```csharp
// Show log count in debug UI
int logCount = ErrorLogger.GetLogCount();
debugText.text = $"Errors: {logCount}";
```

---

## ✅ Verification Checklist

After setup, verify everything works:

- [ ] ErrorLogger GameObject exists in Game scene
- [ ] ErrorLogger script is attached and enabled
- [ ] Python monitor starts without errors
- [ ] `Logs/` directory exists in project root
- [ ] Intentionally create an error in a script
- [ ] Python monitor detects it (prints to console)
- [ ] Debug file created in `Logs/cursor_debug/`
- [ ] Cursor opens automatically with the error
- [ ] `unity_errors.json` exists and contains logs

---

## 📝 Next Steps

1. **Test with current code** - Play a level and let the monitor run
2. **Add custom logging** - Use `ErrorLogger.LogCustomError()` for important events
3. **Review error patterns** - Check `Logs/cursor_debug/` after playing
4. **Ask Claude Code** - When errors occur, ask for debugging help

---

## 🔗 Related Documents

- [SCRIPT_DOCUMENTATION.md](SCRIPT_DOCUMENTATION.md) - Understanding the game code
- [QUICK_SETUP_CHECKLIST.md](QUICK_SETUP_CHECKLIST.md) - Basic game setup
- [UNITY_ERROR_LOGGER_README.md](UNITY_ERROR_LOGGER_README.md) - Original error logger documentation

---

## 💬 Tips for Debugging with Claude Code

When an error opens in Cursor, ask Claude Code:

```
"Help me debug this NullReferenceException"
"What's causing this array index out of range?"
"The player can't move - help me find the bug"
"Why is this enemy not chasing the player?"
```

Claude will:
1. Analyze the stack trace
2. Examine the relevant code
3. Identify the root cause
4. Suggest fixes
5. Explain what went wrong

---

**Your error logging is now ready! 🎉**

Keep the Python monitor running while developing, and you'll catch and fix bugs faster with real-time Cursor integration.
