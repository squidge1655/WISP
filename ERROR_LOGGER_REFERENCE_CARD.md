# Error Logger - Reference Card

Keep this handy while developing!

---

## 🚀 3-Step Startup

```
1. Add ErrorLogger to scene
   └─ Create empty GameObject, attach ErrorLogger.cs

2. Start Python monitor
   └─ cd /Users/siraj/WISP && python3 unity_error_monitor.py

3. Play your game
   └─ Click Play in Unity, errors auto-open in Cursor
```

---

## 📁 Key Locations

```
Your Project Root (/Users/siraj/WISP/)
├── Assets/Scripts/ErrorLogger.cs         ← Unity script
├── unity_error_monitor.py                ← Python monitor (keep running!)
├── Logs/
│   ├── unity_errors.json                 ← JSON log file (read by Python)
│   └── cursor_debug/                     ← Debug files (open in Cursor)
└── Assets/ERROR_LOGGER_INTEGRATION.md    ← Full setup guide
```

---

## 🎮 When an Error Occurs

```
┌──────────────────────┐
│   Error Happens      │
│   (NullRef, etc.)    │
└────────────┬─────────┘
             │
             ↓
┌──────────────────────┐
│ ErrorLogger captures │
│ - Message            │
│ - Stack trace        │
│ - Scene name         │
│ - Timestamp          │
└────────────┬─────────┘
             │
             ↓
┌──────────────────────┐
│ Saves to:            │
│ Logs/unity_errors.   │
│ json                 │
└────────────┬─────────┘
             │
             ↓
┌──────────────────────┐
│ Python monitor       │
│ detects new error    │
│ (every 1 second)     │
└────────────┬─────────┘
             │
             ↓
┌──────────────────────┐
│ Creates debug file:  │
│ Logs/cursor_debug/   │
│ TIMESTAMP_Error_info │
└────────────┬─────────┘
             │
             ↓
┌──────────────────────┐
│ Cursor opens AUTO    │
│ Shows error +        │
│ full stack trace     │
└────────────┬─────────┘
             │
             ↓
┌──────────────────────┐
│ You ask Claude:      │
│ "Help debug this"    │
└──────────────────────┘
```

---

## 💻 Common Commands

### Terminal (Python Monitor)
```bash
# Start monitor
python3 unity_error_monitor.py

# Custom log file path
python3 unity_error_monitor.py /path/to/unity_errors.json

# Stop monitor
^C  (Ctrl+C)
```

### Unity Code (C#)
```csharp
// Manual error logging
ErrorLogger.LogCustomError("Title", "Description");

// Clear all logs
ErrorLogger.ClearLogs();

// Get log count
int count = ErrorLogger.GetLogCount();
```

---

## 🔍 Inspector Settings

**ErrorLogger GameObject → Inspector**

```
✓ Log Messages       (capture Debug.Log)
✓ Log Warnings       (capture warnings)
✓ Log Errors         (capture errors/exceptions)
✓ Include Stack Trace (full call stack)
```

**Don't change these** - they're perfect as-is!

---

## 📊 Log File Format

**File:** `Logs/unity_errors.json`

```json
{
  "logs": [
    {
      "timestamp": "2025-02-16 10:30:45.123",
      "logType": "Error",
      "message": "NullReferenceException: Object reference not set",
      "stackTrace": "at PlayerController.SetPosition()...",
      "scene": "Game"
    },
    {
      "timestamp": "2025-02-16 10:30:50.456",
      "logType": "Warning",
      "message": "Physics is enabled but no rigid body",
      "stackTrace": "",
      "scene": "Game"
    }
  ]
}
```

---

## 🚨 What Gets Sent to Cursor

**SENT** (Auto-opens Cursor):
- ❌ Errors
- ❌ Exceptions
- ❌ Assertions

**NOT SENT** (Console only):
- ⚠️ Warnings
- 📝 Regular logs

---

## ⚙️ Customization

### Send Warnings to Cursor Too

Edit `unity_error_monitor.py` line ~156:
```python
# Change from:
if log_type in ['Error', 'Exception', 'Assert']:

# To:
if log_type in ['Error', 'Exception', 'Assert', 'Warning']:
```

### Check More Frequently

Edit `unity_error_monitor.py` line ~177:
```python
# Change from:
monitor = UnityErrorMonitor(log_file)

# To (check every 0.5 seconds instead of 1):
monitor = UnityErrorMonitor(log_file, check_interval=0.5)
```

### Store More Logs

Edit `ErrorLogger.cs` line ~39:
```csharp
// Change from:
private const int maxLogs = 100;

// To:
private const int maxLogs = 500;
```

---

## 🐛 Quick Troubleshooting

| Issue | Check | Fix |
|-------|-------|-----|
| No debug files in `Logs/cursor_debug/` | ErrorLogger in scene? | Add ErrorLogger GameObject |
| Python says "Waiting..." | Is game running? | Click Play in Unity |
| Cursor won't open | Is Cursor installed? | Open `Logs/cursor_debug/` manually |
| No logs appearing | Script enabled? | Check ErrorLogger script is enabled |
| File not found | Python path correct? | Run from project root: `/Users/siraj/WISP` |

---

## 📚 Documentation Map

```
Quick Answer (30 seconds)
    ↓
ERROR_LOGGER_REFERENCE_CARD.md (this file)
    ↓
    ↙─────────────────────────────────────┐
    │                                     │
Need Setup (2 mins)          Need Details (10 mins)
    │                                     │
    ↓                                     ↓
ERROR_LOGGER_QUICKSTART.md    ERROR_LOGGER_INTEGRATION.md
    │                                     │
    └─────────────────────────────────────┘
                    ↓
            Start building! 🎮
```

---

## 🎯 Daily Workflow

### Morning (Development Start)
```
1. Open Terminal window
2. cd /Users/siraj/WISP
3. python3 unity_error_monitor.py
4. ← Keep this running all day!
5. Open Unity and start developing
```

### When an Error Occurs
```
1. Look at Python terminal
2. See error message
3. Watch Cursor open automatically
4. Ask Claude: "Help me debug this"
5. Fix issue → Test again
```

### Evening (Before Shutdown)
```
1. Game is working
2. Stop Python monitor (Ctrl+C)
3. Review Logs/cursor_debug/ (optional)
4. Check Logs/unity_errors.json (optional)
5. Commit your changes to git
```

---

## 💡 Pro Tips

**Tip 1:** Keep Python monitor visible
- Use a second monitor if possible
- Or use a terminal window you can see

**Tip 2:** Debug files are your friend
- They have everything you need
- Stack trace shows exact line numbers
- Save them for later review

**Tip 3:** Ask Claude for help
- "Why is this NullReferenceException happening?"
- "How do I fix this array index out of range?"
- Claude analyzes the stack trace and helps

**Tip 4:** Use manual logging strategically
```csharp
// Before an operation that might fail
ErrorLogger.LogCustomError("Level Load", "Loading level " + levelIndex);

// After suspicious operations
if (result < 0) {
    ErrorLogger.LogCustomError("Calculation Error", "Result was negative!");
}
```

**Tip 5:** Clean up periodically
```csharp
// At level start or transitions
ErrorLogger.ClearLogs();  // Fresh slate for new level
```

---

## 🎓 Learning Path

### 1st Time Setup
- [ ] Read ERROR_LOGGER_QUICKSTART.md (2 min)
- [ ] Follow 3-Step Startup above (2 min)
- [ ] Test with a game error (1 min)
- [ ] ✅ Ready to code!

### Deeper Understanding
- [ ] Read ERROR_LOGGER_INTEGRATION.md (10 min)
- [ ] Read the .cs comments (5 min)
- [ ] Read the .py comments (5 min)
- [ ] Try customizations (5 min)

### Mastery
- [ ] Integrate with other tools
- [ ] Customize to your workflow
- [ ] Add team guidelines
- [ ] Set up CI/CD integration

---

## 🔗 Quick Links

- **Setup Guide:** `Assets/ERROR_LOGGER_INTEGRATION.md`
- **Quick Start:** `ERROR_LOGGER_QUICKSTART.md`
- **Code Docs:** Comments in `Assets/Scripts/ErrorLogger.cs`
- **Python Docs:** Comments in `unity_error_monitor.py`
- **Full Index:** `Assets/README.md`

---

## ✅ Ready to Go?

- [x] ErrorLogger.cs in Assets/Scripts/
- [x] unity_error_monitor.py in project root
- [x] Documentation files ready
- [x] Python script is executable
- [x] You have this reference card!

### Do This Now:
1. Open Terminal
2. `cd /Users/siraj/WISP`
3. `python3 unity_error_monitor.py`
4. Open Unity
5. Add ErrorLogger to scene
6. Click Play
7. Generate an error
8. Watch Cursor open automatically

**You're ready! Happy debugging! 🎉**

---

## 📊 At a Glance

| Component | File | Purpose | Status |
|-----------|------|---------|--------|
| Unity Script | ErrorLogger.cs | Capture logs | ✅ Ready |
| Python Script | unity_error_monitor.py | Monitor logs | ✅ Ready |
| Setup Guide | ERROR_LOGGER_INTEGRATION.md | Complete guide | ✅ Ready |
| Quick Start | ERROR_LOGGER_QUICKSTART.md | 2-min setup | ✅ Ready |
| This Card | ERROR_LOGGER_REFERENCE_CARD.md | Handy reference | ✅ Ready |

---

**Keep this card bookmarked! 📌**

---

**Version:** 1.0
**Date:** February 16, 2025
**Status:** Production Ready ✅
