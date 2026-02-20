using UnityEngine;

/// <summary>
/// GRIDHELPER - Utility class for grid-to-world coordinate conversion
///
/// This static class provides helper functions for working with the 5x5 game grid.
/// It manages the conversion between:
/// - Grid coordinates: (0-4, 0-4) logical game positions
/// - World coordinates: (-2 to 3, -2 to 3) visual positions in the game world
///
/// The grid is centered at the origin with 1-unit cells:
/// Grid (0,0) = World (-2, -2)
/// Grid (2,2) = World (0, 0)
/// Grid (4,4) = World (2, 2)
///
/// This allows entities to be positioned logically on the 5x5 grid
/// while still having proper world coordinates for rendering.
/// </summary>
public static class GridHelper
{
    // ============ CONSTANTS ============
    /// <summary>
    /// Width of the game grid (in cells)
    /// </summary>
    public const int GRID_WIDTH = 5;

    /// <summary>
    /// Height of the game grid (in cells)
    /// </summary>
    public const int GRID_HEIGHT = 5;

    /// <summary>
    /// Size of each grid cell in world units
    /// 1.0 = 1 unit per cell (matches default Unity scale)
    /// </summary>
    public const float CELL_SIZE = 1f;

    /// <summary>
    /// Offset from world origin (0,0) to grid (0,0)
    /// This centers the 5x5 grid around the origin
    /// Grid (0,0) maps to world (-2, -2, 0)
    /// </summary>
    private static readonly Vector3 GRID_OFFSET = new Vector3(-2f, -2f, 0f);

    /// <summary>
    /// GRIDTOWORLD - Convert grid coordinates to world position
    ///
    /// Transforms logical grid positions into visual world positions.
    /// Used when rendering entities and calculating visual positions.
    ///
    /// PSEUDOCODE:
    /// - Apply grid offset to move from origin (0,0,0) to grid offset
    /// - Add grid X coordinate * cell size + half cell size (center of cell)
    /// - Add grid Y coordinate * cell size + half cell size (center of cell)
    /// - Keep Z at 0 (2D game)
    /// - Return combined world position
    ///
    /// EXAMPLE:
    /// - GridToWorld((0,0)) = (-2, -2, 0) - top-left corner
    /// - GridToWorld((2,2)) = (0, 0, 0) - center
    /// - GridToWorld((4,4)) = (2, 2, 0) - bottom-right corner
    ///
    /// Parameters:
    /// - gridPos: Grid coordinates (0-4, 0-4)
    ///
    /// Returns: World coordinates (Vector3)
    /// </summary>
    public static Vector3 GridToWorld(Vector2Int gridPos)
    {
        // Apply offset + scale grid coordinates to world space
        // Adding CELL_SIZE * 0.5f centers the position within the cell
        return GRID_OFFSET + new Vector3(gridPos.x * CELL_SIZE + CELL_SIZE * 0.5f,
                                         gridPos.y * CELL_SIZE + CELL_SIZE * 0.5f,
                                         0f);
    }

    /// <summary>
    /// WORLDTOGRID - Convert world position to grid coordinates
    ///
    /// Transforms visual world positions into logical grid positions.
    /// Used when processing mouse clicks or converting world positions to grid.
    ///
    /// PSEUDOCODE:
    /// - Subtract grid offset from world position (reverse offset)
    /// - Divide by cell size to convert world units to cell counts
    /// - Use FloorToInt to get integer cell indices
    /// - Return grid coordinates
    ///
    /// EXAMPLE:
    /// - WorldToGrid((-2, -2, 0)) = (0, 0) - top-left
    /// - WorldToGrid((0, 0, 0)) = (2, 2) - center
    /// - WorldToGrid((2, 2, 0)) = (4, 4) - bottom-right
    ///
    /// Parameters:
    /// - worldPos: World position (Vector3)
    ///
    /// Returns: Grid coordinates (Vector2Int)
    /// </summary>
    public static Vector2Int WorldToGrid(Vector3 worldPos)
    {
        // Reverse the offset to get relative position
        Vector3 relativePos = worldPos - GRID_OFFSET;

        // Convert world units to grid cells
        return new Vector2Int(Mathf.FloorToInt(relativePos.x / CELL_SIZE),
                             Mathf.FloorToInt(relativePos.y / CELL_SIZE));
    }

    /// <summary>
    /// ISVALIDGRIDPOSITION - Check if coordinates are within grid bounds
    ///
    /// PSEUDOCODE:
    /// - If X is less than 0 or greater/equal to grid width: return false
    /// - If Y is less than 0 or greater/equal to grid height: return false
    /// - Otherwise: return true
    ///
    /// EXAMPLES:
    /// - IsValidGridPosition((2, 2)) = true (in bounds)
    /// - IsValidGridPosition((5, 2)) = false (X too large)
    /// - IsValidGridPosition((-1, 2)) = false (X too small)
    ///
    /// Parameters:
    /// - gridPos: Position to validate
    ///
    /// Returns: true if valid, false if out of bounds
    /// </summary>
    public static bool IsValidGridPosition(Vector2Int gridPos)
    {
        // Check both X and Y are within [0, GRID_WIDTH) and [0, GRID_HEIGHT)
        return gridPos.x >= 0 && gridPos.x < GRID_WIDTH &&
               gridPos.y >= 0 && gridPos.y < GRID_HEIGHT;
    }

    /// <summary>
    /// GETADJACENTPOSITIONS - Get all neighboring grid positions
    ///
    /// Returns positions of all cells that touch the given cell.
    /// Can include diagonal neighbors or only cardinal neighbors.
    ///
    /// PSEUDOCODE:
    /// - Create array for adjacent positions (4 or 8 size depending on diagonals)
    /// - Add cardinal directions (up, down, left, right):
    ///   - For each cardinal direction:
    ///     - Calculate position in that direction
    ///     - If position is in bounds: add to array
    /// - If diagonals included:
    ///   - Add diagonal directions (up-left, up-right, down-left, down-right):
    ///     - For each diagonal direction:
    ///       - Calculate position in that direction
    ///       - If position is in bounds: add to array
    /// - Resize array to actual number of valid neighbors
    /// - Return array of neighbor positions
    ///
    /// EXAMPLE with diagonals=true at (2,2):
    /// - Returns: [(2,3), (2,1), (1,2), (3,2), (1,3), (3,3), (1,1), (3,1)]
    /// - 8 positions total (all valid, center is not on edge)
    ///
    /// EXAMPLE with diagonals=true at (0,0):
    /// - Returns: [(0,1), (1,0), (1,1)]
    /// - Only 3 positions (others out of bounds)
    ///
    /// Parameters:
    /// - gridPos: Center position
    /// - includeDiagonals: If true, include 8 neighbors; if false, only 4 cardinal
    ///
    /// Returns: Array of valid adjacent positions
    /// </summary>
    public static Vector2Int[] GetAdjacentPositions(Vector2Int gridPos, bool includeDiagonals = true)
    {
        // Allocate array for neighbors (4 cardinal or 8 with diagonals)
        Vector2Int[] adjacent = includeDiagonals
            ? new Vector2Int[8]      // 8 neighbors including diagonals
            : new Vector2Int[4];     // 4 neighbors cardinal only

        // Track how many neighbors we've actually added
        int index = 0;

        // ===== CARDINAL DIRECTIONS (4 directions) =====
        // These are: up, down, left, right
        Vector2Int[] cardinalDirs = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        // Check each cardinal direction
        foreach (var dir in cardinalDirs)
        {
            // Calculate position in this direction
            Vector2Int pos = gridPos + dir;

            // Only add if in bounds
            if (IsValidGridPosition(pos))
            {
                adjacent[index++] = pos;
            }
        }

        // ===== DIAGONAL DIRECTIONS (4 more if requested) =====
        if (includeDiagonals)
        {
            // These are: up-left, up-right, down-left, down-right
            Vector2Int[] diagonalDirs = new Vector2Int[]
            {
                new Vector2Int(1, 1),      // up-right
                new Vector2Int(-1, 1),     // up-left
                new Vector2Int(1, -1),     // down-right
                new Vector2Int(-1, -1)     // down-left
            };

            // Check each diagonal direction
            foreach (var dir in diagonalDirs)
            {
                // Calculate position in this direction
                Vector2Int pos = gridPos + dir;

                // Only add if in bounds
                if (IsValidGridPosition(pos))
                {
                    adjacent[index++] = pos;
                }
            }
        }

        // Resize array to only include valid neighbors
        // (discards empty slots from out-of-bounds checks)
        System.Array.Resize(ref adjacent, index);

        return adjacent;
    }

    /// <summary>
    /// MANHATTANDISTANCE - Calculate shortest grid distance between two positions
    ///
    /// Manhattan distance (also called taxicab distance) is the number of moves
    /// needed to reach one position from another, moving only horizontally or vertically
    /// (like navigating city blocks - no diagonal shortcuts).
    ///
    /// Formula: |x1 - x2| + |y1 - y2|
    ///
    /// PSEUDOCODE:
    /// - Calculate absolute difference in X coordinates
    /// - Calculate absolute difference in Y coordinates
    /// - Return sum of both differences
    ///
    /// EXAMPLES:
    /// - Distance((0,0) to (2,2)) = |0-2| + |0-2| = 2 + 2 = 4
    /// - Distance((1,1) to (3,1)) = |1-3| + |1-1| = 2 + 0 = 2
    /// - Distance((0,0) to (0,0)) = |0-0| + |0-0| = 0 + 0 = 0
    ///
    /// Uses in game:
    /// - Enemy pathfinding (move toward player)
    /// - Avoidance behavior (prefer moves away from goal)
    /// - Proximity checks (is player adjacent?)
    ///
    /// Parameters:
    /// - pos1: First position
    /// - pos2: Second position
    ///
    /// Returns: Distance in grid moves (integer)
    /// </summary>
    public static int ManhattanDistance(Vector2Int pos1, Vector2Int pos2)
    {
        // Calculate horizontal and vertical distance
        // Absolute value ignores direction (distance is always positive)
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
    }
}
