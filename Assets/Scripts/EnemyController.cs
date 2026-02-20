using UnityEngine;

/// <summary>
/// ENEMYTYPE ENUM - Categorizes different types of corrupted creatures
///
/// Red = Standard active enemy (chases player immediately)
/// Purple = Dormant enemy (only chases when player adjacent)
/// Green = (Reserved for future expansion)
///
/// Note: Different colored enemies cannot overlap. Same colored enemies merge.
/// </summary>
public enum EnemyType { Red, Purple, Green }

/// <summary>
/// ENEMYCONTROLLER - Controls individual enemy behavior
///
/// This script handles:
/// - Enemy AI pathfinding (chase toward player using Manhattan distance)
/// - Enemy state management (active/dormant, alive/purified)
/// - Mud trap mechanics (enemies stuck for N turns)
/// - Movement animation
/// - Avoidance behavior (prefer moving away from goal when equally close to player)
/// - Purification/elimination
/// - Visual feedback (color changes based on type and state)
///
/// ENEMY AI BEHAVIOR:
/// 1. Active enemies always try to move toward player
/// 2. Movement is limited to 4 cardinal directions (no diagonals)
/// 3. Cannot move through obstacles (trees) or different-colored enemies
/// 4. When equidistant from player, prefer moving away from goal (avoidance)
/// 5. Dormant enemies don't move until player is adjacent
/// 6. Trapped enemies skip movement for N turns
/// 7. When reaching goal square, eliminated/purified
///
/// EXAMPLE TURN SEQUENCE:
/// - Player moves up
/// - GameManager.ProcessTurn() called
/// - Each active, non-trapped enemy calls MoveTowardPlayer()
/// - Enemy calculates next move, animates to new position
/// - If new position = goal: Purify() called, enemy destroyed
/// - If new position = player position: Player caught (defeat)
/// </summary>
public class EnemyController : MonoBehaviour
{
    // ============ SERIALIZED FIELDS (Configurable in Inspector) ============
    /// <summary>
    /// The color/type of this enemy (Red, Purple, or Green)
    /// Determines behavior and visual appearance
    /// </summary>
    [SerializeField] private EnemyType enemyType = EnemyType.Red;

    /// <summary>
    /// Initial grid position where this enemy spawns
    /// </summary>
    [SerializeField] private Vector2Int position = Vector2Int.zero;

    /// <summary>
    /// If true, enemy actively chases player
    /// If false, enemy is dormant and only activates when player adjacent
    /// </summary>
    [SerializeField] private bool isActive = true;

    /// <summary>
    /// If true, enemy is dormant (visual indicator, mirrors !isActive)
    /// </summary>
    [SerializeField] private bool isDormant = false;

    /// <summary>
    /// How long movement animation takes (in seconds)
    /// Should match player animation speed for synchronized movement
    /// </summary>
    [SerializeField] private float moveAnimationSpeed = 0.1f;

    // ============ PRIVATE FIELDS ============
    /// <summary>
    /// Number of turns this enemy is stuck in mud (cannot move)
    /// Decremented each turn, becomes 0 when free
    /// </summary>
    private int trappedTurns = 0;

    /// <summary>
    /// True if enemy is still in the game, false if purified/eliminated
    /// Dead enemies don't move or interact
    /// </summary>
    private bool isAlive = true;

    /// <summary>
    /// Unique identifier for this enemy (used for state tracking)
    /// Generated using GetInstanceID()
    /// </summary>
    private int uniqueId;

    /// <summary>
    /// Target world position for animation to move toward
    /// Updated when enemy moves to convert grid to world coordinates
    /// </summary>
    private Vector3 targetWorldPosition;

    // ============ PUBLIC PROPERTIES (Read-only access) ============
    public EnemyType EnemyType => enemyType;
    public Vector2Int Position => position;
    public bool IsActive => isActive;
    public bool IsAlive => isAlive;
    public bool IsTrapped => trappedTurns > 0;
    public int UniqueId => uniqueId;

    /// <summary>
    /// AWAKE - Called when script instance is loaded (before Start)
    ///
    /// PSEUDOCODE:
    /// - Generate unique ID for this enemy
    /// - Register this enemy with GameManager (add to enemies list)
    ///
    /// This ensures GameManager knows about this enemy from the start
    /// </summary>
    private void Awake()
    {
        // Create unique identifier for this specific enemy instance
        uniqueId = GetInstanceID();

        // Tell GameManager that this enemy exists
        GameManager.Instance.AddEnemy(this);
    }

    /// <summary>
    /// START - Called on first frame when script is enabled
    ///
    /// PSEUDOCODE:
    /// - Set dormant flag based on isActive (inverse relationship)
    /// - Convert grid position to world position
    /// - Teleport enemy to starting position
    /// - Update visual appearance based on type and state
    /// </summary>
    private void Start()
    {
        // Dormant if NOT active (opposite boolean)
        isDormant = !isActive;

        // Convert grid coordinates to world coordinates
        targetWorldPosition = GridHelper.GridToWorld(position);

        // Place enemy at starting position immediately (no animation)
        transform.position = targetWorldPosition;

        // Update color/appearance
        UpdateVisuals();
    }

    /// <summary>
    /// AWAKEN - Activate a dormant enemy so it starts chasing player
    ///
    /// PSEUDOCODE:
    /// - If enemy is not already active:
    ///   - Set isActive to true (enable chasing)
    ///   - Set isDormant to false (visual indicator)
    ///   - Update visuals (change color from muted to full brightness)
    ///
    /// Called by GameManager when player moves adjacent to dormant enemy
    /// </summary>
    public void Awaken()
    {
        // Only awaken if currently dormant
        if (!isActive)
        {
            // Mark as active (will chase player from now on)
            isActive = true;

            // Mark as not dormant (visual/informational)
            isDormant = false;

            // Update color from muted (dormant) to full brightness (active)
            UpdateVisuals();
        }
    }

    /// <summary>
    /// MOVETOWARDPLAYER - Move one square toward player
    ///
    /// PSEUDOCODE:
    /// - If enemy is dead, trapped, or dormant: do nothing
    /// - Calculate best move toward player using pathfinding
    /// - If new position differs from current position:
    ///   - Update grid position
    ///   - Convert to world coordinates
    ///   - Start movement animation
    ///
    /// Called by GameManager during each turn for active, non-trapped enemies
    ///
    /// Parameters:
    /// - playerPos: Current grid position of the player
    /// </summary>
    public void MoveTowardPlayer(Vector2Int playerPos)
    {
        // Safety checks - don't move if:
        // - Enemy is dead (purified)
        // - Enemy is dormant (not activated yet)
        // - Enemy is trapped in mud (cannot move this turn)
        if (!isAlive || !isActive || IsTrapped) return;

        // Calculate the best next move toward the player
        Vector2Int newPosition = FindNextMoveTowardPlayer(playerPos);

        // If we calculated a different position (we can move somewhere)
        if (newPosition != position)
        {
            // Update grid position
            position = newPosition;

            // Convert grid position to world position
            targetWorldPosition = GridHelper.GridToWorld(newPosition);

            // Animate smoothly from old to new position
            AnimateToPosition();
        }
    }

    /// <summary>
    /// FINDNEXTMOVETOWARDPLAYER - Calculate optimal move toward player
    ///
    /// This is the core pathfinding AI. It:
    /// 1. Checks all 4 cardinal directions (up, down, left, right)
    /// 2. For each direction, validates the move (in bounds, no obstacles, etc.)
    /// 3. Prioritizes moves that get closer to player
    /// 4. When equidistant, uses avoidance: prefers moving away from goal
    ///
    /// PSEUDOCODE:
    /// - Initialize best move as current position (no move)
    /// - For each cardinal direction (up, down, left, right):
    ///   - Calculate position if moving in this direction
    ///   - Check if move is valid:
    ///     - In bounds? No? Skip this direction
    ///     - Tree obstacle? No? Skip
    ///     - Enemy blocking? No? Skip
    ///   - Calculate distance from new position to player
    ///   - If new position gets us CLOSER to player:
    ///     - If closer than best move so far: set as new best move
    ///   - Else if distance is EQUAL to current distance:
    ///     - Calculate distance from new position to goal
    ///     - If new position is FURTHER from goal:
    ///       - Set as new best move (avoidance behavior)
    /// - Return best move found
    ///
    /// AVOIDANCE BEHAVIOR EXAMPLE:
    /// Player is north and east, goal is northwest
    /// Moving north toward player also moves toward goal
    /// Moving east toward player also moves away from goal
    /// Enemy chooses east (avoidance: prefers away from goal)
    ///
    /// Parameters:
    /// - playerPos: Current grid position of the player
    ///
    /// Returns: Best grid position to move to (or current position if stuck)
    /// </summary>
    private Vector2Int FindNextMoveTowardPlayer(Vector2Int playerPos)
    {
        // Current position (default if no valid move exists)
        Vector2Int currentPos = position;

        // Track minimum distance to player found so far
        int minDistance = int.MaxValue;

        // Best move to return (default: don't move)
        Vector2Int bestMove = currentPos;

        // Goal position for avoidance calculation
        Vector2Int goalPos = GameManager.Instance.GoalPosition;

        // ===== CARDINAL DIRECTIONS ONLY =====
        // Enemies can only move up, down, left, right (not diagonally)
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        // ===== CHECK EACH POSSIBLE DIRECTION =====
        foreach (var dir in directions)
        {
            // Calculate where we'd be if we moved this direction
            Vector2Int nextPos = currentPos + dir;

            // ===== VALIDATION CHECKS =====
            // Check if new position is in bounds
            if (!GameManager.Instance.IsValidPosition(nextPos))
                continue;    // Out of bounds - can't move here

            // Check if there's a tree obstacle here
            if (GameManager.Instance.IsObstacle(nextPos))
                continue;    // Tree blocking - can't move here

            // Check if another enemy is here (different color blocks, same color merges later)
            if (GameManager.Instance.IsEnemyAtPosition(nextPos))
                continue;    // Enemy blocking - can't move here

            // ===== DISTANCE CALCULATIONS =====
            // Calculate Manhattan distance from new position to player
            // Manhattan distance = |x1-x2| + |y1-y2|
            int distToPlayer = Mathf.Abs(nextPos.x - playerPos.x) + Mathf.Abs(nextPos.y - playerPos.y);

            // Calculate current distance to player from where we are
            int currentDistToPlayer = Mathf.Abs(currentPos.x - playerPos.x) + Mathf.Abs(currentPos.y - playerPos.y);

            // ===== PRIORITY 1: GET CLOSER TO PLAYER =====
            if (distToPlayer < currentDistToPlayer)
            {
                // This move gets us closer to player!
                // Among all moves that get us closer, pick the CLOSEST one
                if (distToPlayer < minDistance)
                {
                    minDistance = distToPlayer;
                    bestMove = nextPos;
                }
            }
            // ===== PRIORITY 2: AVOIDANCE WHEN EQUIDISTANT =====
            else if (distToPlayer == currentDistToPlayer)
            {
                // This move keeps us same distance from player
                // Tiebreaker: prefer moves that go AWAY from goal

                // Calculate distance from new position to goal
                int distToGoal = Mathf.Abs(nextPos.x - goalPos.x) + Mathf.Abs(nextPos.y - goalPos.y);

                // Calculate distance from current position to goal
                int currentDistToGoal = Mathf.Abs(currentPos.x - goalPos.x) + Mathf.Abs(currentPos.y - goalPos.y);

                // If this move takes us further from goal, prefer it
                // This is the "avoidance" mechanic: enemies avoid the goal square
                if (distToGoal > currentDistToGoal)
                {
                    bestMove = nextPos;
                }
            }
        }

        // Return best move found (or current position if stuck)
        return bestMove;
    }

    /// <summary>
    /// TRAPPEDINMUD - Immobilize enemy in mud for N turns
    ///
    /// PSEUDOCODE:
    /// - Set trappedTurns counter to duration (default 1 turn)
    /// - Enemy cannot move while trappedTurns > 0
    /// - GameManager decrements counter each turn
    ///
    /// Parameters:
    /// - turns: How many turns to trap (default: 1)
    /// </summary>
    public void TrappedInMud(int turns = 1)
    {
        // Set number of turns stuck
        // Each turn, GameManager will call DecrementTrapCounter()
        trappedTurns = turns;
    }

    /// <summary>
    /// DECREMENTTAPCOUNTER - Reduce trapped turn counter by 1
    ///
    /// PSEUDOCODE:
    /// - If trapped counter is greater than 0:
    ///   - Decrease counter by 1
    /// - When counter reaches 0, enemy can move next turn
    ///
    /// Called by GameManager at start of each turn for all trapped enemies
    /// </summary>
    public void DecrementTrapCounter()
    {
        // Only decrement if currently trapped
        if (trappedTurns > 0)
        {
            // Reduce counter (enemy gets closer to freedom each turn)
            trappedTurns--;
        }
    }

    /// <summary>
    /// PURIFY - Eliminate this enemy (reached goal or merged)
    ///
    /// PSEUDOCODE:
    /// - If enemy is alive:
    ///   - Mark as dead (isAlive = false)
    ///   - Unregister from GameManager's enemy list
    ///   - Destroy this GameObject
    ///
    /// Called when:
    /// - Enemy moves to goal square
    /// - Enemy merges with same-colored enemy
    /// - Enemy is eliminated any other way
    /// </summary>
    public void Purify()
    {
        // Only purify if not already dead
        if (isAlive)
        {
            // Mark as dead
            isAlive = false;

            // Unregister from GameManager's tracking list
            GameManager.Instance.RemoveEnemy(this);

            // Destroy this GameObject from the scene
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ANIMATETOPOSITION - Prepare and start movement animation
    ///
    /// PSEUDOCODE:
    /// - Stop any currently running animations
    /// - Start new animation coroutine
    ///
    /// This prevents animation overlap and ensures clean movement
    /// </summary>
    private void AnimateToPosition()
    {
        // Cancel any animation that's currently running
        StopAllCoroutines();

        // Start new animation coroutine
        StartCoroutine(AnimateMoveCoroutine());
    }

    /// <summary>
    /// ANIMATEMOVECOROUTINE - Smoothly animate enemy movement over time
    ///
    /// Parallels PlayerController's movement animation. Both use same duration
    /// to keep player and enemies synchronized visually.
    ///
    /// PSEUDOCODE:
    /// - Record starting world position
    /// - Initialize elapsed time to 0
    /// - While elapsed time is less than animation duration:
    ///   - Add frame time to elapsed time
    ///   - Calculate animation progress (0 to 1)
    ///   - Interpolate (blend) between start and target position
    ///   - Move GameObject to interpolated position
    ///   - Wait for next frame
    /// - After loop: snap to exact target position
    ///
    /// Returns: (coroutine, no explicit return value)
    /// </summary>
    private System.Collections.IEnumerator AnimateMoveCoroutine()
    {
        // Remember where we started
        Vector3 startPosition = transform.position;

        // Time elapsed since animation started
        float elapsed = 0f;

        // ===== ANIMATION LOOP =====
        while (elapsed < moveAnimationSpeed)
        {
            // Increment elapsed time
            elapsed += Time.deltaTime;

            // Calculate progress from 0 to 1
            float t = Mathf.Clamp01(elapsed / moveAnimationSpeed);

            // Smoothly interpolate between start and target
            transform.position = Vector3.Lerp(startPosition, targetWorldPosition, t);

            // Resume next frame
            yield return null;
        }

        // Snap to exact target to avoid floating point errors
        transform.position = targetWorldPosition;
    }

    /// <summary>
    /// UPDATEVISUALS - Update enemy appearance based on type and state
    ///
    /// PSEUDOCODE:
    /// - Get base color for enemy type (red, purple, or green)
    /// - If enemy is dormant:
    ///   - Dim the color to 50% brightness (visual indicator)
    /// - If this GameObject has a SpriteRenderer component:
    ///   - Set its color to the calculated color
    ///
    /// This provides visual feedback: players can see which enemies are dormant
    /// </summary>
    private void UpdateVisuals()
    {
        // Get color based on enemy type
        Color color = GetColorForType();

        // If dormant, make color darker (50% brightness)
        // Active dormant enemies appear dimmed as visual feedback
        if (isDormant)
        {
            color = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f, color.a);
        }

        // Apply color to sprite renderer if it exists
        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            spriteRenderer.color = color;
        }
    }

    /// <summary>
    /// GETCOLORFORTYPE - Get display color based on enemy type
    ///
    /// PSEUDOCODE:
    /// - Switch on enemy type:
    ///   - Red: return pure red color
    ///   - Purple: return purple color
    ///   - Green: return pure green color
    ///   - Default: return white
    ///
    /// Returns: Color struct for rendering
    /// </summary>
    private Color GetColorForType()
    {
        return enemyType switch
        {
            EnemyType.Red => Color.red,                                           // Pure red
            EnemyType.Purple => new Color(0.627f, 0.125f, 0.941f),             // Purple
            EnemyType.Green => Color.green,                                      // Pure green
            _ => Color.white                                                      // Default fallback
        };
    }
}
