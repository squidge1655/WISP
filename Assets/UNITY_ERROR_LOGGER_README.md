# Unity Error Logger with Cursor Integration

Real-time Unity error logging system that captures console output to JSON and automatically sends errors to Cursor/Claude Code for debugging assistance.

## 📋 Components

1. **ErrorLogger.cs** - Unity C# script that captures all console logs to JSON
2. **unity_error_monitor.py** - Python script that monitors the JSON file and sends errors to Cursor

## 🚀 Setup Instructions

### Step 1: Unity Setup

1. **Add the ErrorLogger script to your scene:**
   - Create an empty GameObject in your scene (right-click in Hierarchy → Create Empty)
   - Name it "ErrorLogger"
   - Drag the `ErrorLogger.cs` script onto this GameObject
   - The script will persist across scenes (DontDestroyOnLoad)

2. **Configure settings (optional):**
   - Select the ErrorLogger GameObject
   - In the Inspector, you can toggle:
     - Log Messages (regular Debug.Log)
     - Log Warnings
     - Log Errors
     - Include Stack Trace

3. **File location:**
   - Logs are saved to: `YourProject/Logs/unity_errors.json`
   - This is outside the Assets folder to avoid Unity reimporting it

### Step 2: Python Monitor Setup

1. **Install Python 3** (if not already installed)
   - macOS: `brew install python3` or download from python.org
   - Already available on most systems

2. **Make the script executable (macOS/Linux):**
   ```bash
   chmod +x unity_error_monitor.py
   ```

3. **Run the monitor:**
   ```bash
   python3 unity_error_monitor.py
   ```

   Or specify a custom log file path:
   ```bash
   python3 unity_error_monitor.py /path/to/unity_errors.json
   ```

## 🎮 Usage

### Running the System

1. **Start the Python monitor first:**
   ```bash
   cd "/Users/siraj/Error Test Project"
   python3 unity_error_monitor.py
   ```

2. **Launch your Unity project and play the scene**

3. **When errors occur:**
   - They're logged to `Logs/unity_errors.json`
   - The Python monitor detects new errors
   - Error details are automatically opened in Cursor
   - You can ask Claude Code to help debug the error

### Monitor Behavior

The Python script:
- ✅ Automatically sends **Errors, Exceptions, and Assertions** to Cursor
- 📝 Prints **Warnings and Messages** to console only
- 🔄 Checks for new logs every 1 second
- 💾 Creates debug files in `Logs/cursor_debug/`

### Manual Logging from Unity Code

You can also manually log custom errors:

```csharp
// Log a custom error
ErrorLogger.LogCustomError("Player health calculation failed", "HP value was negative");

// Clear all logs
ErrorLogger.ClearLogs();
```

## 📊 Log Format

Each log entry contains:
- **timestamp**: Precise time with milliseconds
- **logType**: Log, Warning, Error, Exception, or Assert
- **message**: The error message
- **stackTrace**: Full stack trace (if enabled)
- **scene**: Active Unity scene name

Example JSON:
```json
{
  "logs": [
    {
      "timestamp": "2025-12-02 14:30:45.123",
      "logType": "Error",
      "message": "NullReferenceException: Object reference not set",
      "stackTrace": "at PlayerController.Update()...",
      "scene": "GameLevel1"
    }
  ]
}
```

## 🔧 Customization

### Filter Which Logs are Sent to Cursor

Edit `unity_error_monitor.py` line 129:

```python
# Current: Only errors, exceptions, and assertions
if log_type in ['Error', 'Exception', 'Assert']:

# To include warnings:
if log_type in ['Error', 'Exception', 'Assert', 'Warning']:

# To send everything:
if log_type in ['Error', 'Exception', 'Assert', 'Warning', 'Log']:
```

### Change Check Interval

Modify the `check_interval` parameter (default: 1 second):

```python
monitor = UnityErrorMonitor(log_file, check_interval=0.5)  # Check every 0.5s
```

### Maximum Stored Logs

Edit `ErrorLogger.cs` line 27:

```csharp
private const int maxLogs = 100;  // Change this number
```

## 🐛 Troubleshooting

### Logs not appearing
- Ensure the ErrorLogger GameObject is in your scene
- Check that the script is enabled in the Inspector
- Verify the Logs directory exists

### Python script not detecting errors
- Make sure Unity is running and generating logs
- Check the log file path is correct
- Verify the JSON file is being created: `ls Logs/`

### Cursor not opening automatically
- The script creates debug files in `Logs/cursor_debug/`
- You can manually open these files in Cursor
- On macOS, ensure Cursor is in your Applications folder

### Permission errors
- Run: `chmod +x unity_error_monitor.py`
- Ensure Python has permission to write to the project directory

## 💡 Tips

1. **Keep the Python monitor running** in a terminal while developing
2. **Use multiple terminals**: One for the monitor, one for other tasks
3. **Clear old logs** periodically: `ErrorLogger.ClearLogs()`
4. **Review debug files** in `Logs/cursor_debug/` for historical errors
5. **Customize the error message** sent to Cursor by editing the Python script

## 🎯 Workflow Example

1. Start Python monitor in terminal
2. Open Unity and press Play
3. Trigger an error in your game
4. Error automatically opens in Cursor
5. Ask Claude Code: "Help me debug this error"
6. Fix the issue based on Claude's suggestions
7. Test again in Unity

## 📝 Notes

- The system keeps the last 100 logs by default to prevent file bloat
- Stack traces are captured for detailed debugging
- The monitor only sends new errors (no duplicates)
- Error files are timestamped for easy tracking
