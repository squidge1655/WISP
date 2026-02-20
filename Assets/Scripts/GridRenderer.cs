using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    [SerializeField] private Material gridMaterial;
    [SerializeField] private Color gridLineColor = Color.white;
    [SerializeField] private float gridLineWidth = 0.05f;

    private void OnDrawGizmos()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        Gizmos.color = gridLineColor;

        // Draw vertical lines
        for (int x = 0; x <= GridHelper.GRID_WIDTH; x++)
        {
            Vector3 start = GridHelper.GridToWorld(new Vector2Int(x, 0)) + Vector3.back;
            Vector3 end = GridHelper.GridToWorld(new Vector2Int(x, GridHelper.GRID_HEIGHT - 1)) + Vector3.back;

            start.x -= GridHelper.CELL_SIZE * 0.5f;
            end.x -= GridHelper.CELL_SIZE * 0.5f;

            Gizmos.DrawLine(start, end);
        }

        // Draw horizontal lines
        for (int y = 0; y <= GridHelper.GRID_HEIGHT; y++)
        {
            Vector3 start = GridHelper.GridToWorld(new Vector2Int(0, y)) + Vector3.back;
            Vector3 end = GridHelper.GridToWorld(new Vector2Int(GridHelper.GRID_WIDTH - 1, y)) + Vector3.back;

            start.y -= GridHelper.CELL_SIZE * 0.5f;
            end.y -= GridHelper.CELL_SIZE * 0.5f;

            Gizmos.DrawLine(start, end);
        }
    }

    public void HighlightCell(Vector2Int gridPos, Color color)
    {
        // Placeholder for visual feedback
        Debug.Log($"Highlighting cell at {gridPos} with color {color}");
    }

    public void ClearHighlights()
    {
        // Placeholder for clearing highlights
    }
}
