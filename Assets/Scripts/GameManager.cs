using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// GAMEMANAGER - Central Game Controller (Singleton Pattern)
///
/// This is the heart of the game logic. It manages:
/// - The current game state (Playing, Won, Lost, etc.)
/// - All game entities (player, enemies, obstacles, goals)
/// - The turn sequence and game flow
/// - Victory and defeat conditions
///
/// Being a singleton ensures there's only ONE GameManager in the entire game,
/// making it accessible from anywhere via GameManager.Instance
///
/// TURN FLOW:
/// 1. Player moves → PlayerController.AttemptMove() called
/// 2. PlayerController calls GameManager.ProcessTurn()
/// 3. ProcessTurn() executes entire turn sequence:
///    - Decrease mud trap counters
///    - Activate dormant enemies (if adjacent to player)
///    - Move all active, non-trapped enemies toward player
///    - Handle enemy merging (same-color enemies combine)
///    - Remove enemies on goal square
///    - Check if player was caught (lose condition)
///    - Check if all enemies eliminated (win condition)
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton instance - accessible via GameManager.Instance from any script
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// GameState enum represents all possible states the game can be in.
    /// Loading = Setting up level
    /// Playing = Active gameplay, player can move
    /// Won = All enemies eliminated, player won
    /// Lost = Enemy caught player, player lost
    /// Paused = Game paused (not currently used in prototype)
    /// </summary>
    [System.Serializable]
    public enum GameState { Loading, Playing, Won, Lost, Paused }

    // ============ PRIVATE SERIALIZED FIELDS (Visible in Inspector) ============
    [SerializeField] private GameState gameState = GameState.Loading;
    [SerializeField] private int currentLevel = 0;

    // ============ PRIVATE FIELDS (Not visible in Inspector) ============
    // References to game entities
    private PlayerController player;                                    // Reference to the player character
    private List<EnemyController> enemies = new List<EnemyController>();  // List of all enemies in the level

    // Level layout data
    private Vector2Int goalPosition;                                    // Position of the purification zone
    private List<Vector2Int> obstacles = new List<Vector2Int>();      // Positions of trees that block movement
    private List<Vector2Int> mudPatches = new List<Vector2Int>();     // Positions of mud that traps enemies

    // ============ CONSTANTS ============
    private const int GRID_WIDTH = 5;   // 5x5 grid width
    private const int GRID_HEIGHT = 5;  // 5x5 grid height

    // ============ PUBLIC PROPERTIES (Read-only access for other scripts) ============
    public GameState CurrentGameState => gameState;
    public int CurrentLevel => currentLevel;
    public Vector2Int GoalPosition => goalPosition;
    public List<Vector2Int> Obstacles => obstacles;
    public List<Vector2Int> MudPatches => mudPatches;

    /// <summary>
    /// AWAKE - Called when script instance is being loaded
    ///
    /// PSEUDOCODE:
    /// - If another instance of GameManager already exists:
    ///   - Destroy this duplicate (prevent multiple GameManagers)
    /// - Otherwise:
    ///   - Set this as THE GameManager instance
    ///
    /// This implements the Singleton pattern, ensuring only one GameManager exists.
    /// </summary>
    private void Awake()
    {
        // Check if a GameManager instance already exists
        if (Instance != null && Instance != this)
        {
            // Destroy this duplicate instance
            Destroy(gameObject);
            return;
        }
        // Set this as the singleton instance
        Instance = this;
    }

    /// <summary>
    /// START - Called on first frame when script is enabled
    ///
    /// PSEUDOCODE:
    /// - Initialize the level (find player and enemies in scene)
    /// </summary>
    private void Start()
    {
        InitializeLevel();
    }

    /// <summary>
    /// INITIALIZELEVEL - Sets up the current level
    ///
    /// PSEUDOCODE:
    /// - Set game state to "Playing" (allow gameplay)
    /// - Search scene for PlayerController and store reference
    /// - If no PlayerController found:
    ///   - Log error and exit
    /// - Search scene for all EnemyControllers and store in list
    /// - Log debug message with enemy count
    /// - Set game state to "Playing" (ready for input)
    /// </summary>
    private void InitializeLevel()
    {
        gameState = GameState.Playing;

        // Find player in scene - returns first object with PlayerController component
        player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            // Player is required, so log error if not found
            Debug.LogError("PlayerController not found in scene!");
            return;
        }

        // Find all enemies in scene - returns array of all EnemyController components
        enemies = new List<EnemyController>(FindObjectsOfType<EnemyController>());

        // Log for debugging
        Debug.Log($"Level initialized with {enemies.Count} enemies");
        gameState = GameState.Playing;
    }

    /// <summary>
    /// SETUPLEVEL - Configure the level layout and entities
    ///
    /// Called by LevelManager when a level is loaded
    ///
    /// PSEUDOCODE:
    /// - Store the goal position
    /// - Store list of obstacle positions
    /// - Store list of mud patch positions
    /// - If player exists:
    ///   - Move player to the starting position
    ///
    /// Parameters:
    /// - playerStart: Grid position where player should spawn
    /// - goal: Grid position of the purification zone
    /// - obs: List of all obstacle positions
    /// - mud: List of all mud patch positions
    /// </summary>
    public void SetupLevel(Vector2Int playerStart, Vector2Int goal, List<Vector2Int> obs, List<Vector2Int> mud)
    {
        // Store the goal position for later reference
        goalPosition = goal;

        // Store obstacle positions (make a copy to avoid external modifications)
        obstacles = new List<Vector2Int>(obs);

        // Store mud patch positions (make a copy to avoid external modifications)
        mudPatches = new List<Vector2Int>(mud);

        // Move player to starting position
        if (player != null)
        {
            player.SetPosition(playerStart);
        }
    }

    /// <summary>
    /// ISVALIDPOSITION - Check if a grid position is within bounds
    ///
    /// PSEUDOCODE:
    /// - If X coordinate is less than 0 or greater/equal to grid width: return false
    /// - If Y coordinate is less than 0 or greater/equal to grid height: return false
    /// - Otherwise: return true (position is valid)
    ///
    /// Parameter:
    /// - pos: The grid position to validate
    ///
    /// Returns: true if position is within 5x5 grid, false otherwise
    /// </summary>
    public bool IsValidPosition(Vector2Int pos)
    {
        // Check if X is in valid range [0, GRID_WIDTH)
        // AND Y is in valid range [0, GRID_HEIGHT)
        return pos.x >= 0 && pos.x < GRID_WIDTH && pos.y >= 0 && pos.y < GRID_HEIGHT;
    }

    /// <summary>
    /// ISOBSTACLE - Check if a position contains a tree/obstacle
    ///
    /// PSEUDOCODE:
    /// - Search obstacle list for the given position
    /// - Return true if found, false if not found
    ///
    /// Parameter:
    /// - pos: The grid position to check
    ///
    /// Returns: true if position has an obstacle, false otherwise
    /// </summary>
    public bool IsObstacle(Vector2Int pos)
    {
        // Check if this position is in the obstacles list
        return obstacles.Contains(pos);
    }

    /// <summary>
    /// ISMUDPATCH - Check if a position contains mud
    ///
    /// PSEUDOCODE:
    /// - Search mud patch list for the given position
    /// - Return true if found, false if not found
    ///
    /// Parameter:
    /// - pos: The grid position to check
    ///
    /// Returns: true if position has mud, false otherwise
    /// </summary>
    public bool IsMudPatch(Vector2Int pos)
    {
        // Check if this position is in the mudPatches list
        return mudPatches.Contains(pos);
    }

    /// <summary>
    /// ISENEMYATPOSITION - Check if an active enemy occupies a position
    ///
    /// PSEUDOCODE:
    /// - Search through all enemies
    /// - If any enemy exists at position AND is active:
    ///   - Return true
    /// - If no active enemy found:
    ///   - Return false
    ///
    /// Parameter:
    /// - pos: The grid position to check
    ///
    /// Returns: true if active enemy at position, false otherwise
    ///
    /// Note: Only checks ACTIVE enemies, not dormant ones
    /// </summary>
    public bool IsEnemyAtPosition(Vector2Int pos)
    {
        // Find first enemy that matches both conditions:
        // 1. Position matches given position
        // 2. Enemy is active (not dormant)
        return enemies.Exists(e => e.Position == pos && e.IsActive);
    }

    /// <summary>
    /// GETENEMYATPOSITION - Get reference to the enemy at a position
    ///
    /// PSEUDOCODE:
    /// - Search through all enemies
    /// - If any enemy exists at position AND is active:
    ///   - Return that enemy
    /// - If no active enemy found:
    ///   - Return null
    ///
    /// Parameter:
    /// - pos: The grid position to check
    ///
    /// Returns: EnemyController reference, or null if no enemy found
    /// </summary>
    public EnemyController GetEnemyAtPosition(Vector2Int pos)
    {
        // Find and return first enemy matching both conditions
        return enemies.Find(e => e.Position == pos && e.IsActive);
    }

    /// <summary>
    /// GETALLACTIVEENEMIES - Get list of all active (non-eliminated) enemies
    ///
    /// PSEUDOCODE:
    /// - Search through all enemies
    /// - Create new list containing only active enemies
    /// - Return new list
    ///
    /// Returns: List of all active enemies. Empty list if all eliminated.
    ///
    /// Used for:
    /// - Checking victory condition (win if list is empty)
    /// - Updating UI enemy counter
    /// </summary>
    public List<EnemyController> GetAllActiveEnemies()
    {
        // Find all enemies where IsActive is true
        return enemies.FindAll(e => e.IsActive);
    }

    /// <summary>
    /// PROCESSTURN - Execute one complete turn of the game
    ///
    /// Called by PlayerController after a successful move
    ///
    /// TURN SEQUENCE (in order):
    /// 1. Mud Check - Decrease trap counters on trapped enemies
    /// 2. Dormant Check - Activate dormant enemies if adjacent to player
    /// 3. Enemy Movement - Move all active, non-trapped enemies toward player
    /// 4. Collision Resolution - Merge same-colored enemies that overlap
    /// 5. Goal Check - Remove enemies on goal square
    /// 6. Capture Check - Defeat player if enemy on player position
    /// 7. Victory Check - Win if no enemies remain
    /// </summary>
    public void ProcessTurn()
    {
        // Only process if game is in Playing state
        if (gameState != GameState.Playing) return;

        // ===== STEP 1: MUD CHECK =====
        // Reduce mud trap timers for trapped enemies
        foreach (var enemy in enemies)
        {
            // If enemy is currently trapped in mud
            if (enemy.IsTrapped)
            {
                // Reduce trapped turn counter by 1
                // When counter reaches 0, enemy can move next turn
                enemy.DecrementTrapCounter();
            }
        }

        // ===== STEP 2: DORMANT CHECK =====
        // Check if player is adjacent to any dormant enemies
        Vector2Int playerPos = player.Position;
        foreach (var enemy in enemies)
        {
            // Check all enemies (both active and dormant)
            if (!enemy.IsActive && !enemy.IsAlive)
            {
                // If enemy is dormant AND alive
                Vector2Int enemyPos = enemy.Position;
                // Check if player is adjacent (within 1 square in any direction)
                if (IsAdjacent(playerPos, enemyPos))
                {
                    // Wake up the dormant enemy
                    enemy.Awaken();
                }
            }
        }

        // ===== STEP 3: ENEMY MOVEMENT =====
        // Move all enemies that can move
        foreach (var enemy in enemies)
        {
            // Move enemy ONLY if:
            // - Enemy is alive (not purified)
            // - Enemy is active (not dormant)
            // - Enemy is not trapped in mud (cannot move this turn)
            if (enemy.IsAlive && enemy.IsActive && !enemy.IsTrapped)
            {
                // Move one square toward the player
                enemy.MoveTowardPlayer(player.Position);
            }
        }

        // ===== STEP 4: COLLISION RESOLUTION =====
        // Handle enemies occupying same square (merging)
        ResolveCollisions();

        // ===== STEP 5: GOAL CHECK =====
        // Remove all enemies on the goal square
        PurifyEnemiesOnGoal();

        // ===== STEP 6: CAPTURE CHECK =====
        // Defeat condition: if enemy occupies same square as player
        if (IsEnemyAtPosition(player.Position))
        {
            // Player was caught by an enemy
            SetGameState(GameState.Lost);
            return;
        }

        // ===== STEP 7: VICTORY CHECK =====
        // Victory condition: if no active enemies remain
        if (GetAllActiveEnemies().Count == 0)
        {
            // All enemies eliminated - player wins
            SetGameState(GameState.Won);
        }
    }

    /// <summary>
    /// ISADJACENT - Check if two positions are adjacent (neighboring)
    ///
    /// Adjacent means: at most 1 square away in X or Y direction
    /// Includes diagonals (up to 1 square away in all directions)
    ///
    /// PSEUDOCODE:
    /// - Calculate absolute difference in X coordinates
    /// - Calculate absolute difference in Y coordinates
    /// - If difference in X is 0-1 AND difference in Y is 0-1:
    ///   - If both differences are 0 (same position): return false (not adjacent to self)
    ///   - Otherwise: return true (adjacent)
    /// - Otherwise: return false (too far apart)
    ///
    /// Examples:
    /// - (0,0) and (1,0) → adjacent (side by side)
    /// - (0,0) and (1,1) → adjacent (diagonal)
    /// - (0,0) and (2,0) → NOT adjacent (too far)
    /// - (0,0) and (0,0) → NOT adjacent (same position)
    /// </summary>
    private bool IsAdjacent(Vector2Int pos1, Vector2Int pos2)
    {
        // Calculate how far apart positions are in each direction
        int dx = Mathf.Abs(pos1.x - pos2.x);
        int dy = Mathf.Abs(pos1.y - pos2.y);

        // Adjacent if: at most 1 away in both directions, AND not same position
        return (dx <= 1 && dy <= 1) && !(dx == 0 && dy == 0);
    }

    /// <summary>
    /// RESOLVECOLLISIONS - Handle enemies in same square (merging)
    ///
    /// MERGING RULES:
    /// - Only same-colored enemies can merge
    /// - When they merge, keep one and eliminate others
    /// - Different-colored enemies block each other (cannot occupy same square)
    ///
    /// PSEUDOCODE:
    /// - Create a grouping of enemies by position:
    ///   - For each alive enemy:
    ///     - If no group exists for this position: create it
    ///     - Add enemy to group
    /// - For each group of enemies at same position:
    ///   - If more than one enemy in group:
    ///     - Get color of first enemy
    ///     - Check if ALL enemies in group are same color:
    ///       - If yes: eliminate all but first one (they merge)
    ///       - If no: do nothing (different colors blocking each other)
    /// </summary>
    private void ResolveCollisions()
    {
        // Group enemies by their position
        // Dictionary key = grid position, value = list of enemies at that position
        Dictionary<Vector2Int, List<EnemyController>> positionGroups = new Dictionary<Vector2Int, List<EnemyController>>();

        // Build groups of enemies by position
        foreach (var enemy in enemies)
        {
            // Skip dead enemies
            if (!enemy.IsAlive) continue;

            // If no group exists for this position yet
            if (!positionGroups.ContainsKey(enemy.Position))
            {
                // Create new empty group
                positionGroups[enemy.Position] = new List<EnemyController>();
            }
            // Add this enemy to its position group
            positionGroups[enemy.Position].Add(enemy);
        }

        // Check each group for merging
        foreach (var group in positionGroups.Values)
        {
            // Only process groups with multiple enemies
            if (group.Count > 1)
            {
                // Get the color/type of the first enemy
                EnemyType firstType = group[0].EnemyType;

                // Check if ALL enemies in group are the same color
                if (group.TrueForAll(e => e.EnemyType == firstType))
                {
                    // All same color - merge them!
                    // Keep first enemy, eliminate rest (starting from index 1)
                    for (int i = 1; i < group.Count; i++)
                    {
                        // Purify (eliminate) this enemy
                        group[i].Purify();
                    }
                }
            }
        }
    }

    /// <summary>
    /// PURIFYENEMIESONGOAL - Remove enemies that reached the goal square
    ///
    /// PSEUDOCODE:
    /// - For each alive enemy:
    ///   - If enemy position equals goal position:
    ///     - Purify (eliminate) the enemy
    ///
    /// This implements the core win mechanic: lure enemies to goal to eliminate them
    /// </summary>
    private void PurifyEnemiesOnGoal()
    {
        // Check all enemies
        foreach (var enemy in enemies)
        {
            // If enemy is alive AND on goal square
            if (enemy.IsAlive && enemy.Position == goalPosition)
            {
                // Eliminate the enemy
                enemy.Purify();
            }
        }
    }

    /// <summary>
    /// SETGAMESTATE - Change the current game state
    ///
    /// PSEUDOCODE:
    /// - Update gameState variable to new state
    /// - Log the state change for debugging
    ///
    /// Parameter:
    /// - newState: The new GameState to transition to
    ///
    /// Called when:
    /// - Player wins (all enemies eliminated) → Won
    /// - Player loses (caught by enemy) → Lost
    /// - Level is reset → Loading
    /// </summary>
    public void SetGameState(GameState newState)
    {
        // Update the game state
        gameState = newState;
        // Log for debugging (helps track game flow)
        Debug.Log($"Game State Changed: {newState}");
    }

    /// <summary>
    /// RESETLEVEL - Restart the current level
    ///
    /// PSEUDOCODE:
    /// - Set game state to Loading
    /// - Reinitialize the level (find player and enemies again)
    /// </summary>
    public void ResetLevel()
    {
        // Set to loading state
        SetGameState(GameState.Loading);
        // Reinitialize all entities
        InitializeLevel();
    }

    /// <summary>
    /// ADDENEMY - Register a new enemy with the GameManager
    ///
    /// Called by EnemyController in its Awake method
    ///
    /// PSEUDOCODE:
    /// - If enemy not already in list:
    ///   - Add enemy to enemies list
    ///
    /// This keeps GameManager aware of all enemies in the scene
    /// </summary>
    public void AddEnemy(EnemyController enemy)
    {
        // Only add if not already in list (prevent duplicates)
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    /// <summary>
    /// REMOVEENEMY - Unregister an enemy from the GameManager
    ///
    /// Called by EnemyController when purified/eliminated
    ///
    /// PSEUDOCODE:
    /// - Remove enemy from enemies list
    ///
    /// This keeps GameManager's enemy list in sync with actual entities
    /// </summary>
    public void RemoveEnemy(EnemyController enemy)
    {
        // Remove from tracking list
        enemies.Remove(enemy);
    }
}
