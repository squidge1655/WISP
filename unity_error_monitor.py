#!/usr/bin/env python3
"""
Unity Error Monitor - Watches JSON log file and sends errors to Cursor

This script monitors a JSON file created by ErrorLogger.cs (a Unity script).
When new errors are detected, it:
1. Extracts error details
2. Creates a debug file with the error information
3. Automatically opens the file in Cursor for analysis

Usage:
    python3 unity_error_monitor.py [log_file_path]

Example:
    python3 unity_error_monitor.py /Users/siraj/WISP/Logs/unity_errors.json
    python3 unity_error_monitor.py  # Uses default path

Author: Claude Code
Purpose: Real-time error monitoring for Unity games
"""

import json
import os
import sys
import time
import subprocess
import platform
from pathlib import Path
from datetime import datetime

class UnityErrorMonitor:
    """
    Monitors Unity error log file and sends errors to Cursor

    High-level description:
    This class watches a JSON file that Unity's ErrorLogger.cs continuously updates.
    Every second, it checks if new error/exception/assertion logs have been added.
    When detected, it creates a debug file and opens it in Cursor for analysis.

    Key responsibilities:
    - Monitor JSON file for changes
    - Parse JSON log entries
    - Filter for errors (skip warnings/messages)
    - Track which errors have been processed (avoid duplicates)
    - Create debug files with error details
    - Open files in Cursor IDE
    """

    def __init__(self, log_file, check_interval=1, debug_dir=None):
        """
        Initialize the error monitor

        Parameters:
        - log_file: Path to the unity_errors.json file created by ErrorLogger.cs
        - check_interval: How often to check for new logs (in seconds)
        - debug_dir: Where to save debug files (default: Logs/cursor_debug/)

        Pseudocode:
        1. Store the log file path and check interval
        2. Initialize processed logs set (empty) - used to track which errors we've already seen
        3. Determine debug directory:
           - If not provided: use parent of log file + /cursor_debug/
           - Create directory if it doesn't exist
        4. Print startup message with file paths
        """
        self.log_file = log_file
        self.check_interval = check_interval
        self.processed_logs = set()  # Track processed logs to avoid duplicates

        # Determine debug directory
        if debug_dir is None:
            log_dir = Path(log_file).parent
            self.debug_dir = log_dir / "cursor_debug"
        else:
            self.debug_dir = Path(debug_dir)

        # Create debug directory if it doesn't exist
        self.debug_dir.mkdir(parents=True, exist_ok=True)

        print(f"[UnityErrorMonitor] Initialized")
        print(f"  📁 Log file: {self.log_file}")
        print(f"  📁 Debug dir: {self.debug_dir}")
        print(f"  ⏱️  Check interval: {check_interval}s")
        print(f"  🎯 Filters: Errors, Exceptions, Assertions")
        print(f"  ⏳ Waiting for logs...\n")

    def read_logs(self):
        """
        Read and parse the JSON log file

        Returns: List of log entries, or empty list if file doesn't exist

        Pseudocode:
        1. Check if log file exists
           - If not: print waiting message, return empty list
        2. Try to read the JSON file:
           - Read entire file as text
           - Parse JSON into Python dictionary
           - Extract the "logs" array from the dictionary
           - Return the list of logs
        3. If any error occurs (JSON parse error, missing "logs" key, etc.):
           - Print error message
           - Return empty list (graceful failure)
        """
        if not os.path.exists(self.log_file):
            return []

        try:
            with open(self.log_file, 'r') as f:
                data = json.load(f)
                return data.get("logs", [])
        except json.JSONDecodeError as e:
            print(f"[ERROR] Failed to parse JSON: {e}")
            return []
        except Exception as e:
            print(f"[ERROR] Failed to read log file: {e}")
            return []

    def process_log(self, log_entry):
        """
        Process a single log entry and create debug file if needed

        Parameters:
        - log_entry: Dictionary containing one log entry with keys:
          - timestamp: When it occurred
          - logType: Type of log ("Error", "Exception", "Assert", "Warning", "Log")
          - message: The error message
          - stackTrace: Full call stack
          - scene: Active scene name

        Pseudocode:
        1. Extract log type, message, timestamp from entry
        2. Create unique identifier for this log
           - Use: logType + message + timestamp (prevents duplicates)
        3. Check if we've already processed this exact log
           - If yes: Skip it (return without action)
           - If no: Continue processing
        4. Add this log to processed set (mark as seen)
        5. Check if this is an error type we should send to Cursor
           - Only send: Error, Exception, Assert
           - Skip: Warning, Log
        6. If sending to Cursor:
           - Extract stack trace (if available)
           - Determine scene name (or use "Unknown")
           - Create debug file with error details
           - Open the file in Cursor
        """
        # Extract log information
        log_type = log_entry.get("logType", "Unknown")
        message = log_entry.get("message", "Unknown error")
        timestamp = log_entry.get("timestamp", "Unknown time")
        stack_trace = log_entry.get("stackTrace", "")
        scene = log_entry.get("scene", "Unknown")

        # Create unique identifier for this log (prevent duplicates)
        log_id = f"{log_type}:{message}:{timestamp}"

        # Skip if already processed
        if log_id in self.processed_logs:
            return

        # Mark as processed
        self.processed_logs.add(log_id)

        # Only send errors, exceptions, and assertions to Cursor
        # (Skip warnings and regular log messages)
        if log_type not in ['Error', 'Exception', 'Assert']:
            # Print to console only
            print(f"[{log_type}] {message}")
            return

        # Send to Cursor
        print(f"\n🚨 [SENDING TO CURSOR] {log_type}: {message}")
        self.send_to_cursor(log_type, message, stack_trace, scene, timestamp)

    def send_to_cursor(self, log_type, message, stack_trace, scene, timestamp):
        """
        Create debug file and open in Cursor

        Parameters:
        - log_type: Type of error ("Error", "Exception", "Assert")
        - message: Error message text
        - stack_trace: Full call stack
        - scene: Scene where error occurred
        - timestamp: When error occurred

        Pseudocode:
        1. Create a timestamped filename for the debug file
           - Use current time + sanitize message to create filename
           - Example: "2025-12-02_14-30-45_NullReferenceException.md"
        2. Create formatted debug file content with:
           - Error type and message as heading
           - Timestamp
           - Scene information
           - Stack trace
           - Helpful debugging tips
        3. Write formatted content to debug file
        4. Print confirmation message
        5. Try to open file in Cursor IDE:
           - On macOS: Use 'open -a Cursor' command
           - On Linux: Use 'code' or 'cursor' command
           - On Windows: Use 'start cursor' command
           - If open fails: print message suggesting manual opening
        """
        # Create debug filename
        now = datetime.now().strftime("%Y-%m-%d_%H-%M-%S")
        # Sanitize message for filename (remove unsafe characters)
        safe_message = "".join(c if c.isalnum() else "_" for c in message[:50])
        debug_filename = f"{now}_{log_type}_{safe_message}.md"
        debug_file = self.debug_dir / debug_filename

        # Create formatted debug content
        debug_content = f"""# {log_type}: {message}

## Error Details
- **Type:** {log_type}
- **Timestamp:** {timestamp}
- **Scene:** {scene}

## Stack Trace
```
{stack_trace if stack_trace else 'No stack trace available'}
```

## Debugging Tips
1. Check the error message above - what is it trying to tell you?
2. Review the stack trace to find the problematic code location
3. Look at the scene name - what was happening when this occurred?
4. Common issues:
   - NullReferenceException: Trying to use an object that doesn't exist
   - OutOfRangeException: Accessing array/list index that's out of bounds
   - ArgumentException: Invalid argument passed to a function
   - MissingComponentException: Missing required component on GameObject

## Next Steps
1. Ask Claude Code: "Help me debug this {log_type}"
2. Provide context about what was happening when the error occurred
3. Ask for suggestions on how to fix the issue

---
*Auto-generated debug file from UnityErrorMonitor*
"""

        # Write debug file
        try:
            with open(debug_file, 'w') as f:
                f.write(debug_content)
            print(f"  ✅ Debug file created: {debug_file}")
        except Exception as e:
            print(f"  ❌ Failed to create debug file: {e}")
            return

        # Try to open in Cursor
        self.open_in_cursor(debug_file)

    def open_in_cursor(self, file_path):
        """
        Open a file in Cursor IDE

        Parameters:
        - file_path: Path to the file to open

        Pseudocode:
        1. Detect operating system (macOS, Linux, Windows)
        2. Try to open file with appropriate command:
           - macOS: Use 'open -a Cursor' (Cursor app in Applications folder)
           - Linux: Try 'code' or 'cursor' command
           - Windows: Use 'start cursor' command
        3. If command succeeds:
           - Print success message
        4. If command fails:
           - Print message suggesting manual opening
           - User can open the file from Logs/cursor_debug/ folder
        """
        system = platform.system()

        try:
            if system == "Darwin":  # macOS
                subprocess.Popen(["open", "-a", "Cursor", str(file_path)])
                print(f"  🔓 Opening in Cursor...")
            elif system == "Linux":
                subprocess.Popen(["code", str(file_path)])
                print(f"  🔓 Opening in VS Code...")
            elif system == "Windows":
                subprocess.Popen(["start", "cursor", str(file_path)], shell=True)
                print(f"  🔓 Opening in Cursor...")
            else:
                print(f"  ⚠️  Unsupported OS: {system}")
                print(f"     Please open manually: {file_path}")
        except Exception as e:
            print(f"  ⚠️  Could not open in Cursor automatically: {e}")
            print(f"     Please open manually: {file_path}")

    def monitor(self):
        """
        Main monitoring loop

        Runs continuously, checking for new logs every `check_interval` seconds

        Pseudocode:
        1. Enter infinite loop
        2. Read current logs from JSON file
        3. For each log entry:
           - Process it (check if error, send to Cursor if needed)
        4. Sleep for check_interval seconds
        5. Repeat (loop back to step 2)

        The loop runs until Ctrl+C is pressed (KeyboardInterrupt)
        """
        try:
            while True:
                # Read current logs from file
                logs = self.read_logs()

                # Process each log entry
                for log_entry in logs:
                    self.process_log(log_entry)

                # Wait before checking again
                time.sleep(self.check_interval)

        except KeyboardInterrupt:
            # User pressed Ctrl+C
            print("\n[UnityErrorMonitor] Stopped (Ctrl+C)")
            sys.exit(0)


def find_log_file():
    """
    Try to find the Unity error log file automatically

    Returns: Path to log file, or None if not found

    Pseudocode:
    1. Check common locations for Logs/unity_errors.json:
       - Current working directory: ./Logs/unity_errors.json
       - Parent directory: ../Logs/unity_errors.json
       - User home: ~/WISP/Logs/unity_errors.json
    2. For each location:
       - Check if file exists
       - If yes: return that path
    3. If none found:
       - Print message and return None
    """
    potential_paths = [
        Path("Logs/unity_errors.json"),
        Path("../Logs/unity_errors.json"),
        Path.home() / "WISP" / "Logs" / "unity_errors.json",
    ]

    for path in potential_paths:
        if path.exists():
            return str(path)

    return None


def main():
    """
    Main entry point for the script

    Pseudocode:
    1. Check command-line arguments:
       - If provided: use as log file path
       - If not: try to find log file automatically
    2. If log file path found:
       - Create UnityErrorMonitor instance
       - Start monitoring loop
    3. If log file not found:
       - Print error message with instructions
       - Exit with error code
    """
    # Get log file path from command-line argument or auto-find
    if len(sys.argv) > 1:
        log_file = sys.argv[1]
    else:
        log_file = find_log_file()

    # Validate log file path
    if not log_file:
        print("[ERROR] Could not find unity_errors.json")
        print("\nUsage:")
        print("  python3 unity_error_monitor.py /path/to/unity_errors.json")
        print("\nOr place this script in the project root directory:")
        print("  python3 unity_error_monitor.py")
        print("\nExpected file location:")
        print("  ProjectRoot/Logs/unity_errors.json")
        sys.exit(1)

    # Create and start monitor
    monitor = UnityErrorMonitor(log_file)
    monitor.monitor()


if __name__ == "__main__":
    main()
