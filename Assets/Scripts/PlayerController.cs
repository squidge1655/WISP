using UnityEngine;

/// <summary>
/// PLAYERCONTROLLER - Controls the player character (Will-o'-the-Wisp)
///
/// This script handles:
/// - Reading player input (keyboard controls)
/// - Validating moves (checking bounds, obstacles, enemies)
/// - Moving the player to new positions
/// - Animating movement smoothly
/// - Triggering the game turn sequence after each move
///
/// CONTROL SCHEME:
/// Arrow Keys or WASD - Cardinal movement (up, down, left, right)
/// QEZC - Diagonal movement (Q=up-left, E=up-right, Z=down-left, C=down-right)
/// R - Reset current level
///
/// MOVEMENT FLOW:
/// 1. Player presses movement key
/// 2. HandleInput() reads the input
/// 3. AttemptMove() validates the move:
///    - Check if position is within 5x5 grid
///    - Check if an enemy blocks the position
///    - If valid: move player and trigger GameManager.ProcessTurn()
///    - If invalid: do nothing (illegal move)
/// 4. SetPosition() updates position and starts animation
/// 5. AnimateMoveCoroutine() smoothly transitions player from old to new position
/// 6. GameManager.ProcessTurn() executes entire turn sequence (enemy AI, collisions, etc.)
/// </summary>
public class PlayerController : MonoBehaviour
{
    // ============ SERIALIZED FIELDS (Configurable in Inspector) ============
    /// <summary>
    /// Grid position of the player (0-4 for both X and Y on 5x5 grid)
    /// </summary>
    [SerializeField] private Vector2Int position = Vector2Int.zero;

    /// <summary>
    /// Duration of movement animation in seconds
    /// Affects how smooth/fast the movement appears
    /// 0.1 = snappy, 0.5 = slow and smooth
    /// </summary>
    [SerializeField] private float moveAnimationSpeed = 0.1f;

    // ============ PRIVATE FIELDS ============
    /// <summary>
    /// Target world position for the animation to move toward
    /// Updated when player moves to convert grid coordinates to world coordinates
    /// </summary>
    private Vector3 targetWorldPosition;

    /// <summary>
    /// Flag: true if animation is currently playing
    /// </summary>
    private bool isMoving = false;

    /// <summary>
    /// Flag: false while animation is playing, true when ready for input
    /// Prevents player from moving multiple times before animation completes
    /// </summary>
    private bool canMove = true;

    // ============ PUBLIC PROPERTIES ============
    /// <summary>
    /// Read-only access to player's current grid position
    /// Used by other scripts to check player location
    /// </summary>
    public Vector2Int Position => position;

    /// <summary>
    /// START - Called on first frame when script is enabled
    ///
    /// PSEUDOCODE:
    /// - Convert grid position to world position
    /// - Teleport player to that world position (no animation on startup)
    /// </summary>
    private void Start()
    {
        // Convert grid coordinates (0-4) to world coordinates
        targetWorldPosition = GridHelper.GridToWorld(position);
        // Place player at target position immediately
        transform.position = targetWorldPosition;
    }

    /// <summary>
    /// UPDATE - Called every frame
    ///
    /// PSEUDOCODE:
    /// - If player cannot move currently: exit early
    /// - If game is not in Playing state: exit early
    /// - Otherwise: read and process player input
    ///
    /// Uses early exit pattern to skip processing when conditions aren't met
    /// </summary>
    private void Update()
    {
        // Exit if player is currently moving or locked
        if (!canMove || GameManager.Instance.CurrentGameState != GameManager.GameState.Playing)
            return;

        // Read input and attempt to move
        HandleInput();
    }

    /// <summary>
    /// HANDLEINPUT - Read keyboard input and attempt moves
    ///
    /// Checks for pressed keys and converts them to movement directions.
    /// Supports two control schemes:
    /// 1. Arrow Keys + WASD for cardinal directions
    /// 2. QEZC for diagonal movement
    ///
    /// PSEUDOCODE:
    /// - Initialize move direction to zero (no movement)
    /// - Check arrow keys and WASD:
    ///   - If Up arrow or W pressed: set direction to up
    ///   - Else if Down arrow or S pressed: set direction to down
    ///   - Else if Left arrow or A pressed: set direction to left
    ///   - Else if Right arrow or D pressed: set direction to right
    /// - Check diagonal keys:
    ///   - If Q pressed: set direction to up-left
    ///   - Else if E pressed: set direction to up-right
    ///   - Else if Z pressed: set direction to down-left
    ///   - Else if C pressed: set direction to down-right
    /// - If direction is not zero:
    ///   - Attempt to move in that direction
    /// - Check for reset key:
    ///   - If R pressed: reset level via GameManager
    ///
    /// Note: Uses GetKeyDown (triggers once when pressed), not GetKey (continuous)
    /// </summary>
    private void HandleInput()
    {
        // Start with no movement
        Vector2Int moveDirection = Vector2Int.zero;

        // ===== CARDINAL DIRECTIONS (UP/DOWN/LEFT/RIGHT) =====
        // These use if-else to ensure only ONE direction is processed per frame
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            moveDirection = Vector2Int.up;            // Direction: up
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            moveDirection = Vector2Int.down;          // Direction: down
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            moveDirection = Vector2Int.left;          // Direction: left
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            moveDirection = Vector2Int.right;         // Direction: right

        // ===== DIAGONAL DIRECTIONS (IF NO CARDINAL KEY PRESSED) =====
        // Uses else-if so diagonals don't override cardinal directions
        if (Input.GetKeyDown(KeyCode.Q))
            moveDirection = new Vector2Int(-1, 1);    // Direction: up-left
        else if (Input.GetKeyDown(KeyCode.E))
            moveDirection = new Vector2Int(1, 1);     // Direction: up-right
        else if (Input.GetKeyDown(KeyCode.Z))
            moveDirection = new Vector2Int(-1, -1);   // Direction: down-left
        else if (Input.GetKeyDown(KeyCode.C))
            moveDirection = new Vector2Int(1, -1);    // Direction: down-right

        // ===== ATTEMPT MOVE =====
        // Only try to move if a direction was pressed
        if (moveDirection != Vector2Int.zero)
        {
            AttemptMove(moveDirection);
        }

        // ===== RESET LEVEL =====
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Tell GameManager to reset the level
            GameManager.Instance.ResetLevel();
        }
    }

    /// <summary>
    /// ATTEMPTMOVE - Try to move player in a direction, with validation
    ///
    /// This is the core move logic. It validates the move before executing it.
    ///
    /// PSEUDOCODE:
    /// - Calculate new position by adding direction to current position
    /// - Check if new position is valid (within 5x5 grid):
    ///   - If not valid: return false (illegal move)
    /// - Check if enemy occupies new position:
    ///   - If yes: return false (blocked by enemy)
    /// - Move is valid:
    ///   - Update player position (and start animation)
    ///   - Lock input (set canMove to false)
    ///   - Trigger entire game turn via GameManager.ProcessTurn():
    ///     - This executes enemy movement, collision checks, win/lose conditions
    ///   - Unlock input (set canMove to true)
    ///   - Return true (move successful)
    ///
    /// Parameters:
    /// - direction: Direction to move (e.g., Vector2Int.up for north)
    ///
    /// Returns: true if move succeeded, false if blocked/illegal
    /// </summary>
    public bool AttemptMove(Vector2Int direction)
    {
        // Calculate destination by adding direction to current position
        // Example: if at (1,1) and pressing right, new position is (2,1)
        Vector2Int newPosition = position + direction;

        // ===== BOUNDS CHECK =====
        // Verify new position is within 5x5 grid
        if (!GameManager.Instance.IsValidPosition(newPosition))
        {
            // Out of bounds - move blocked
            return false;
        }

        // ===== COLLISION CHECK =====
        // Check if an active enemy occupies this position
        if (GameManager.Instance.IsEnemyAtPosition(newPosition))
        {
            // Enemy blocks - move blocked
            return false;
        }

        // ===== MOVE IS VALID =====
        // Update position and trigger animation
        SetPosition(newPosition);

        // Lock input while animation plays and enemies move
        canMove = false;

        // Execute entire turn sequence:
        // 1. Decrease mud trap timers
        // 2. Activate dormant enemies adjacent to player
        // 3. Move all active enemies toward player
        // 4. Resolve collisions (enemy merging)
        // 5. Eliminate enemies on goal
        // 6. Check for defeat (player caught)
        // 7. Check for victory (all enemies eliminated)
        GameManager.Instance.ProcessTurn();

        // Unlock input for next move
        canMove = true;

        // Return success
        return true;
    }

    /// <summary>
    /// SETPOSITION - Update player position and trigger movement animation
    ///
    /// PSEUDOCODE:
    /// - Update grid position to new position
    /// - Convert grid position to world position
    /// - Start animation coroutine that smoothly moves player over time
    ///
    /// Parameters:
    /// - newPosition: New grid position to move to
    /// </summary>
    public void SetPosition(Vector2Int newPosition)
    {
        // Update the grid position
        position = newPosition;

        // Convert grid coordinates to world coordinates
        // Example: grid (0,0) might be world (-2, -2, 0) depending on GridHelper.GridOffset
        targetWorldPosition = GridHelper.GridToWorld(newPosition);

        // Start smooth movement animation toward target
        AnimateToPosition();
    }

    /// <summary>
    /// ANIMATETOPOSITION - Prepare and start the movement animation
    ///
    /// PSEUDOCODE:
    /// - Stop any currently running animations (prevent overlapping)
    /// - Start new animation coroutine that smoothly moves player
    /// </summary>
    private void AnimateToPosition()
    {
        // Cancel any animation that might be running
        // This prevents overlapping animations if player moves multiple times quickly
        StopAllCoroutines();

        // Start new coroutine that animates movement over time
        StartCoroutine(AnimateMoveCoroutine());
    }

    /// <summary>
    /// ANIMATEMOVECOROUTINE - Smoothly animate player movement over time
    ///
    /// This coroutine runs over multiple frames, gradually moving the player
    /// from current position to target position.
    ///
    /// PSEUDOCODE:
    /// - Mark isMoving as true
    /// - Record starting world position
    /// - Initialize elapsed time to 0
    /// - While elapsed time is less than animation duration:
    ///   - Add frame time to elapsed time
    ///   - Calculate interpolation progress (0 to 1) based on elapsed time
    ///   - Interpolate (blend) between start and target position
    ///   - Move player GameObject to interpolated position
    ///   - Wait for next frame
    /// - After loop: snap player to exact target position (avoid floating point drift)
    /// - Mark isMoving as false
    ///
    /// Animation Example (0.1 second animation):
    /// - Frame 0: elapsed=0.0, t=0.0, position=start
    /// - Frame 1: elapsed=0.016, t=0.16, position=16% between start and target
    /// - Frame 2: elapsed=0.032, t=0.32, position=32% between start and target
    /// - ...continues...
    /// - Final: elapsed=0.1, t=1.0, position=target
    ///
    /// Returns: (coroutine, no explicit return value)
    /// </summary>
    private System.Collections.IEnumerator AnimateMoveCoroutine()
    {
        // Flag that we're currently animating
        isMoving = true;

        // Store the position player is moving FROM
        Vector3 startPosition = transform.position;

        // Time elapsed since animation started (in seconds)
        float elapsed = 0f;

        // ===== ANIMATION LOOP =====
        // Continue looping until animation duration has elapsed
        while (elapsed < moveAnimationSpeed)
        {
            // Increment elapsed time by time since last frame
            elapsed += Time.deltaTime;

            // Calculate animation progress from 0 to 1
            // Example: if elapsed=0.05 and duration=0.1, then t=0.5 (halfway)
            float t = Mathf.Clamp01(elapsed / moveAnimationSpeed);

            // Smoothly interpolate (blend) between start and target positions
            // t=0 returns startPosition, t=1 returns targetWorldPosition, 0<t<1 returns in-between
            transform.position = Vector3.Lerp(startPosition, targetWorldPosition, t);

            // Pause and resume next frame
            yield return null;
        }

        // After animation completes, snap to exact target (avoid floating point errors)
        transform.position = targetWorldPosition;

        // Flag that animation is complete
        isMoving = false;
    }

    /// <summary>
    /// RESETTOSTART - Return player to starting position and reset animation state
    ///
    /// Called by LevelManager when level is reset (player presses R or chooses Reset)
    ///
    /// PSEUDOCODE:
    /// - Stop any currently running animation
    /// - Reset grid position to start position
    /// - Calculate world position for start position
    /// - Teleport player to start position immediately (no animation)
    /// - Reset animation flags (not moving, can accept input)
    ///
    /// Parameters:
    /// - startPosition: Grid position where player should spawn
    /// </summary>
    public void ResetToStart(Vector2Int startPosition)
    {
        // Stop animation coroutine if running
        StopAllCoroutines();

        // Reset to starting grid position
        position = startPosition;

        // Convert grid position to world position
        targetWorldPosition = GridHelper.GridToWorld(startPosition);

        // Teleport player immediately (no animation)
        transform.position = targetWorldPosition;

        // Reset animation state
        isMoving = false;

        // Allow player to move again
        canMove = true;
    }
}
