using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ENEMYSPAWN - Serializable data structure for spawning enemies
///
/// This struct contains all the information needed to create an enemy in the level.
/// It's serialized so designers can configure enemy placements in the Unity Inspector.
///
/// FIELDS:
/// - position: Grid coordinates where this enemy spawns
/// - type: Color/type of enemy (Red, Purple, Green)
/// - isDormant: If true, enemy starts dormant and only activates when player adjacent
/// </summary>
[System.Serializable]
public struct EnemySpawn
{
    /// <summary>
    /// Grid position (0-4, 0-4) where this enemy is placed
    /// </summary>
    public Vector2Int position;

    /// <summary>
    /// Type/color of this enemy (Red, Purple, or Green)
    /// Different colors have different behaviors
    /// </summary>
    public EnemyType type;

    /// <summary>
    /// If true, enemy starts dormant (won't chase until player adjacent)
    /// If false, enemy starts active (chases immediately)
    /// </summary>
    public bool isDormant;
}

/// <summary>
/// LEVELDATA - ScriptableObject storing all configuration for a single level
///
/// This asset contains the complete blueprint for a game level:
/// - Player starting position
/// - Goal/sacred grove position
/// - Enemy placements and types
/// - Obstacle positions (trees)
/// - Mud patch positions
/// - Level name and number
/// - Optimal solution length (for scoring)
///
/// USAGE:
/// 1. In Unity, right-click in Assets > Create > Will-o'-the-Wisp > Level Data
/// 2. Configure the level in the Inspector
/// 3. Save the asset
/// 4. Add to LevelManager's level list
/// 5. LevelManager loads it when that level is played
///
/// DESIGNER WORKFLOW:
/// - Designer creates new LevelData for each puzzle
/// - Sets player start, goal position, obstacles
/// - Places enemies and specifies if dormant
/// - Validates using ValidateLevel()
/// - Plays to verify puzzle is solvable
/// - Adjusts as needed
/// </summary>
[CreateAssetMenu(fileName = "Level_", menuName = "Will-o'-the-Wisp/Level Data")]
public class LevelData : ScriptableObject
{
    // ============ SERIALIZED FIELDS ============
    /// <summary>
    /// Display name for this level (e.g., "The Forest's Edge")
    /// Shown in UI and level select
    /// </summary>
    [SerializeField] private string levelName = "Level 1";

    /// <summary>
    /// Sequential level number (1, 2, 3, etc.)
    /// Used for progression and display
    /// </summary>
    [SerializeField] private int levelNumber = 1;

    /// <summary>
    /// Grid position where player spawns
    /// Usually somewhere safe with space to move
    /// </summary>
    [SerializeField] private Vector2Int playerStart = Vector2Int.zero;

    /// <summary>
    /// Grid position of the goal/sacred grove
    /// Where enemies must be lured to be eliminated
    /// </summary>
    [SerializeField] private Vector2Int goalPosition = new Vector2Int(4, 4);

    /// <summary>
    /// List of grid positions containing trees/obstacles
    /// Blocks movement for both player and enemies
    /// Player can walk through, enemies cannot
    /// </summary>
    [SerializeField] private List<Vector2Int> obstacles = new List<Vector2Int>();

    /// <summary>
    /// List of grid positions containing mud patches
    /// Traps enemies for one turn (not implemented yet, for future use)
    /// </summary>
    [SerializeField] private List<Vector2Int> mudPatches = new List<Vector2Int>();

    /// <summary>
    /// Array of enemy spawn configurations
    /// Each element specifies position, type, and dormancy
    /// </summary>
    [SerializeField] private EnemySpawn[] enemies = new EnemySpawn[0];

    /// <summary>
    /// Optimal number of moves to solve this level
    /// Used for scoring/rating solution quality
    /// 1 star = solved, 2 stars = par+2, 3 stars = optimal
    /// </summary>
    [SerializeField] private int minMoves = 0;

    // ============ PUBLIC PROPERTIES (Read-only) ============
    public string LevelName => levelName;
    public int LevelNumber => levelNumber;
    public Vector2Int PlayerStart => playerStart;
    public Vector2Int GoalPosition => goalPosition;
    public List<Vector2Int> Obstacles => obstacles;
    public List<Vector2Int> MudPatches => mudPatches;
    public EnemySpawn[] Enemies => enemies;
    public int MinMoves => minMoves;

    /// <summary>
    /// VALIDATELEVEL - Check for configuration errors (Unity Editor only)
    ///
    /// This function validates that all positions in the level are within bounds.
    /// Helps catch designer mistakes before playing.
    ///
    /// PSEUDOCODE:
    /// - Check if player start is in bounds (0-4, 0-4):
    ///   - If not: log warning
    /// - Check if goal is in bounds:
    ///   - If not: log warning
    /// - For each obstacle position:
    ///   - Check if in bounds:
    ///     - If not: log warning with position
    /// - For each mud patch position:
    ///   - Check if in bounds:
    ///     - If not: log warning with position
    /// - For each enemy:
    ///   - Check if position is in bounds:
    ///     - If not: log warning with position
    ///
    /// Called from:
    /// - Custom Inspector script (when designer clicks Validate button)
    /// - Or manually in editor
    ///
    /// Note: Only available in Unity Editor, not in runtime builds
    /// (marked with #if UNITY_EDITOR)
    /// </summary>
#if UNITY_EDITOR
    public void ValidateLevel()
    {
        // ===== CHECK PLAYER START =====
        if (!GridHelper.IsValidGridPosition(playerStart))
            Debug.LogWarning($"Player start position {playerStart} is out of bounds!");

        // ===== CHECK GOAL =====
        if (!GridHelper.IsValidGridPosition(goalPosition))
            Debug.LogWarning($"Goal position {goalPosition} is out of bounds!");

        // ===== CHECK OBSTACLES =====
        foreach (var obstacle in obstacles)
        {
            if (!GridHelper.IsValidGridPosition(obstacle))
                Debug.LogWarning($"Obstacle position {obstacle} is out of bounds!");
        }

        // ===== CHECK MUD PATCHES =====
        foreach (var mud in mudPatches)
        {
            if (!GridHelper.IsValidGridPosition(mud))
                Debug.LogWarning($"Mud patch position {mud} is out of bounds!");
        }

        // ===== CHECK ENEMIES =====
        foreach (var enemy in enemies)
        {
            if (!GridHelper.IsValidGridPosition(enemy.position))
                Debug.LogWarning($"Enemy position {enemy.position} is out of bounds!");
        }
    }
#endif
}
