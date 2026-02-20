using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// ErrorLogger - Captures all Unity console logs to JSON file for debugging
///
/// This script automatically captures console output (logs, warnings, errors)
/// and saves them to a JSON file outside the Assets folder. The Python monitor
/// script (unity_error_monitor.py) watches this file and sends errors to Cursor.
///
/// Usage:
/// 1. Create an empty GameObject in your scene
/// 2. Attach this script to it
/// 3. The script persists across scenes (DontDestroyOnLoad)
/// 4. Configure logging options in the Inspector
/// 5. Run unity_error_monitor.py in a terminal to watch for errors
/// </summary>
public class ErrorLogger : MonoBehaviour
{
    // ============================================================
    // INSPECTOR CONFIGURABLE SETTINGS
    // ============================================================

    [SerializeField]
    private bool logMessages = true;      // Capture Debug.Log() calls
    [SerializeField]
    private bool logWarnings = true;      // Capture warnings
    [SerializeField]
    private bool logErrors = true;        // Capture errors and exceptions
    [SerializeField]
    private bool includeStackTrace = true; // Include full stack trace in logs

    // ============================================================
    // INTERNAL CONSTANTS
    // ============================================================

    private const int maxLogs = 100;      // Maximum logs to keep (prevents file bloat)
    private const string logFileName = "unity_errors.json";

    // ============================================================
    // INTERNAL STATE
    // ============================================================

    private string logFilePath;
    private LogData logData;
    private static ErrorLogger instance;

    /// <summary>
    /// Internal data structure for storing logs
    ///
    /// Pseudocode:
    /// - logs: List of LogEntry objects, each containing:
    ///   - timestamp: When the log was generated
    ///   - logType: Type of log (Log, Warning, Error, Exception, Assert)
    ///   - message: The log message text
    ///   - stackTrace: Full call stack (if enabled)
    ///   - scene: Name of active scene
    /// </summary>
    [System.Serializable]
    public class LogEntry
    {
        public string timestamp;
        public string logType;
        public string message;
        public string stackTrace;
        public string scene;
    }

    /// <summary>
    /// Container for all logs
    ///
    /// Pseudocode:
    /// - Serializable wrapper containing a list of LogEntry objects
    /// - Used for JSON serialization/deserialization
    /// </summary>
    [System.Serializable]
    public class LogData
    {
        public List<LogEntry> logs = new List<LogEntry>();
    }

    // ============================================================
    // UNITY LIFECYCLE METHODS
    // ============================================================

    /// <summary>
    /// Awake - Initialize error logger on game start
    ///
    /// Pseudocode:
    /// 1. Check if instance already exists (Singleton pattern)
    ///    - If yes: Destroy this duplicate and return
    ///    - If no: Set this as the singleton instance
    /// 2. Call DontDestroyOnLoad() so logger persists across scene loads
    /// 3. Set up log file path: YourProject/Logs/unity_errors.json
    /// 4. Create Logs directory if it doesn't exist
    /// 5. Load existing logs from file (if any)
    /// 6. Register callback to capture console output:
    ///    - Subscribe to Application.logMessageReceivedThreaded
    ///    - This captures all Debug.Log, warnings, errors in real-time
    /// </summary>
    private void Awake()
    {
        // Singleton pattern - ensure only one ErrorLogger exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Set up log file path (outside Assets folder to avoid reimport)
        string logsDirectory = Path.Combine(Application.persistentDataPath, "..", "Logs");
        logsDirectory = Path.GetFullPath(logsDirectory); // Resolve .. references

        // Create Logs directory if it doesn't exist
        if (!Directory.Exists(logsDirectory))
        {
            Directory.CreateDirectory(logsDirectory);
        }

        logFilePath = Path.Combine(logsDirectory, logFileName);

        // Load existing logs from file
        LoadLogs();

        // Subscribe to console output capture
        // This callback is called whenever Debug.Log, warning, or error occurs
        Application.logMessageReceivedThreaded += HandleLog;

        Debug.Log($"[ErrorLogger] Initialized - Logs will be saved to: {logFilePath}");
    }

    /// <summary>
    /// OnDestroy - Cleanup when logger is destroyed
    ///
    /// Pseudocode:
    /// 1. Unsubscribe from console output callback
    ///    - This prevents memory leaks if logger is destroyed
    /// 2. Save any remaining logs to file
    /// </summary>
    private void OnDestroy()
    {
        // Unsubscribe from console to prevent memory leaks
        Application.logMessageReceivedThreaded -= HandleLog;

        // Save remaining logs
        SaveLogs();
    }

    // ============================================================
    // LOG CAPTURE METHODS
    // ============================================================

    /// <summary>
    /// HandleLog - Capture console output
    ///
    /// This is called automatically by Unity whenever something is logged
    ///
    /// Parameters:
    /// - logString: The message that was logged
    /// - stackTrace: The call stack at time of logging
    /// - type: Type of log (Log, Warning, Error, Exception, Assert)
    ///
    /// Pseudocode:
    /// 1. Check if this type of log should be captured based on settings
    ///    - logMessages: Capture regular Debug.Log?
    ///    - logWarnings: Capture warnings?
    ///    - logErrors: Capture errors and exceptions?
    /// 2. If not enabled for this type, return early (skip this log)
    /// 3. Create a new LogEntry object containing:
    ///    - timestamp: Current time with milliseconds (formatted as "YYYY-MM-DD HH:MM:SS.mmm")
    ///    - logType: The type as a string ("Error", "Warning", etc.)
    ///    - message: The log message text
    ///    - stackTrace: Full stack trace (only if includeStackTrace is enabled)
    ///    - scene: Name of currently active scene
    /// 4. Add this entry to the logs list
    /// 5. If we've exceeded maxLogs limit:
    ///    - Remove the oldest log entry (keep only recent logs)
    /// 6. Save the updated logs to the JSON file immediately
    ///    - This ensures logs are available for the Python monitor to detect
    /// </summary>
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Check if this log type should be captured
        switch (type)
        {
            case LogType.Log:
                if (!logMessages) return;
                break;
            case LogType.Warning:
                if (!logWarnings) return;
                break;
            case LogType.Error:
            case LogType.Exception:
            case LogType.Assert:
                if (!logErrors) return;
                break;
        }

        // Create a new log entry with current timestamp
        LogEntry entry = new LogEntry
        {
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            logType = type.ToString(),
            message = logString,
            stackTrace = includeStackTrace ? stackTrace : "",
            scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        };

        // Add to logs list
        logData.logs.Add(entry);

        // Keep only the last maxLogs entries (remove oldest if exceeded)
        if (logData.logs.Count > maxLogs)
        {
            logData.logs.RemoveAt(0);
        }

        // Save immediately so Python monitor can detect new errors
        SaveLogs();
    }

    // ============================================================
    // FILE I/O METHODS
    // ============================================================

    /// <summary>
    /// LoadLogs - Load existing logs from JSON file
    ///
    /// Pseudocode:
    /// 1. Check if log file exists
    ///    - If not: Create a new empty LogData structure
    ///    - If yes: Read the entire JSON file as text
    /// 2. Parse the JSON text into LogData object using JsonUtility
    /// 3. Set this as our current logData
    /// 4. If any error occurs during parsing:
    ///    - Print error to console
    ///    - Create new empty LogData (fallback)
    /// </summary>
    private void LoadLogs()
    {
        try
        {
            if (!File.Exists(logFilePath))
            {
                // Create new empty log data if file doesn't exist
                logData = new LogData();
            }
            else
            {
                // Read and parse JSON file
                string json = File.ReadAllText(logFilePath);
                logData = JsonUtility.FromJson<LogData>(json);

                if (logData == null)
                {
                    logData = new LogData();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[ErrorLogger] Failed to load logs: {e.Message}");
            logData = new LogData();
        }
    }

    /// <summary>
    /// SaveLogs - Write current logs to JSON file
    ///
    /// Pseudocode:
    /// 1. Convert logData object to JSON string using JsonUtility
    /// 2. Create logs directory if it doesn't exist
    /// 3. Write JSON string to file at logFilePath
    ///    - This overwrites any previous content
    /// 4. If any error occurs:
    ///    - Print error to console (don't crash the game)
    /// </summary>
    private void SaveLogs()
    {
        try
        {
            // Ensure directory exists
            string directory = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Convert logs to JSON and write to file
            string json = JsonUtility.ToJson(logData, true);
            File.WriteAllText(logFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[ErrorLogger] Failed to save logs: {e.Message}");
        }
    }

    // ============================================================
    // PUBLIC API METHODS
    // ============================================================

    /// <summary>
    /// LogCustomError - Manually log a custom error from your code
    ///
    /// Usage:
    /// ErrorLogger.LogCustomError("Player health invalid", "Health was negative");
    ///
    /// Parameters:
    /// - title: Brief error title
    /// - description: Detailed error description
    ///
    /// Pseudocode:
    /// 1. Format the error message as "title: description"
    /// 2. Call Debug.LogError() to log it
    ///    - This will be automatically captured by HandleLog callback
    /// </summary>
    public static void LogCustomError(string title, string description)
    {
        Debug.LogError($"[CustomError] {title}: {description}");
    }

    /// <summary>
    /// ClearLogs - Delete all stored logs
    ///
    /// Usage:
    /// ErrorLogger.ClearLogs();
    ///
    /// Pseudocode:
    /// 1. Create new empty LogData
    /// 2. Save empty logs to file
    /// 3. This removes the old log file content
    /// </summary>
    public static void ClearLogs()
    {
        if (instance != null)
        {
            instance.logData = new LogData();
            instance.SaveLogs();
            Debug.Log("[ErrorLogger] Logs cleared");
        }
    }

    /// <summary>
    /// GetLogCount - Get the number of stored logs
    ///
    /// Returns: Number of log entries currently stored
    ///
    /// Pseudocode:
    /// 1. If instance exists: return the count of logs
    /// 2. Otherwise: return 0
    /// </summary>
    public static int GetLogCount()
    {
        return instance != null ? instance.logData.logs.Count : 0;
    }
}
