# Error Logger - Quick Start (2 Minutes)

## ⚡ Fastest Setup Ever

### Step 1: Add to Unity (1 minute)
```
1. Hierarchy → Right-click → Create Empty
2. Rename to "ErrorLogger"
3. Add Component → Scripts → ErrorLogger
4. Save scene
```

### Step 2: Start Monitor (30 seconds)
```bash
cd /Users/siraj/WISP
python3 unity_error_monitor.py
```

Keep this terminal open while developing!

### Step 3: Test It (30 seconds)
1. Click Play in Unity
2. Trigger an error (or wait for one naturally)
3. Watch Python terminal - error appears
4. Cursor opens automatically with debug info

✅ **Done! You're now catching errors in real-time.**

---

## 🎯 What Happens When an Error Occurs

```
Unity Game Error
    ↓
ErrorLogger captures it
    ↓
Saves to Logs/unity_errors.json
    ↓
Python monitor detects new error
    ↓
Creates debug file in Logs/cursor_debug/
    ↓
Cursor opens automatically
    ↓
You ask Claude Code for help debugging
```

---

## 📋 Inspector Settings (Already Good)

The default settings are perfect:
- ✅ Log Messages (enabled)
- ✅ Log Warnings (enabled)
- ✅ Log Errors (enabled)
- ✅ Include Stack Trace (enabled)

**Don't change unless you have a specific reason!**

---

## 🔍 Where Are Your Logs?

| Location | Purpose |
|----------|---------|
| `Logs/unity_errors.json` | Raw JSON log file (updating in real-time) |
| `Logs/cursor_debug/` | Timestamped debug files (opens in Cursor) |

Delete `Logs/cursor_debug/` files anytime to clean up (doesn't affect logging).

---

## 🛠️ Common Commands

### In your game code (C#):
```csharp
// Manually log an error
ErrorLogger.LogCustomError("Title", "Description");

// Clear all logs
ErrorLogger.ClearLogs();

// Check how many logs
int count = ErrorLogger.GetLogCount();
```

### In Terminal:
```bash
# Start monitoring
python3 unity_error_monitor.py

# Specify custom log file path
python3 unity_error_monitor.py /custom/path/to/unity_errors.json

# Stop monitoring (Ctrl+C)
^C
```

---

## ❓ Why Isn't It Working?

| Problem | Fix |
|---------|-----|
| No debug files in `Logs/cursor_debug/` | Generate an error in game, wait 1 second |
| Python says "Waiting for logs..." | Click Play in Unity to generate logs |
| Cursor doesn't open | Manual fallback: open `Logs/cursor_debug/` file manually |
| "File not found" error | Make sure `ErrorLogger` GameObject is in scene |

---

## 💡 Pro Tips

1. **Use a second monitor or terminal window** for the Python monitor
   - That way you see errors appear in real-time
   - Never close the terminal while developing

2. **The first error takes ~1 second to appear** in Cursor
   - This is normal (Python checks every 1 second)

3. **All the info is in the debug files**
   - Even if Cursor doesn't open, the files are still there
   - Check `Logs/cursor_debug/` folder

4. **Stack trace is your best friend**
   - Shows exactly which line of code caused the error
   - Use this to find the bug quickly

---

## 📊 Example Error Flow

### Scenario: NullReferenceException in PlayerController

```
1. You click Play in Unity
2. Game crashes: "NullReferenceException: Object reference not set"
3. ErrorLogger captures the error + stack trace
4. Python monitor detects it
5. Debug file created: "2025-02-16_10-30-45_Error_NullReferenceException.md"
6. Cursor opens with the file
7. You read the stack trace: "at PlayerController.SetPosition() line 89"
8. You ask Claude: "Help me debug this NullReferenceException"
9. Claude finds the bug: "player object reference is null"
10. You fix it and test again
11. No more errors!
```

---

## ✅ You're Ready!

That's it. Seriously. Your error logging is:
- ✅ Installed
- ✅ Configured
- ✅ Running
- ✅ Sending errors to Cursor

Now go build your game! Errors will be caught and reported automatically.

---

## 📚 Full Documentation

For more details, see:
- [ERROR_LOGGER_INTEGRATION.md](Assets/ERROR_LOGGER_INTEGRATION.md) - Complete setup guide
- [UNITY_ERROR_LOGGER_README.md](Assets/UNITY_ERROR_LOGGER_README.md) - Original docs
- [ErrorLogger.cs](Assets/Scripts/ErrorLogger.cs) - Code comments
- [unity_error_monitor.py](unity_error_monitor.py) - Code comments

---

**Enjoy real-time error debugging! 🎉**
