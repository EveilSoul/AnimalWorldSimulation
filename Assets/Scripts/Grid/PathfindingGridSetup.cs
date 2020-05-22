using UnityEngine;

public class PathfindingGridSetup : MonoBehaviour {

    public static PathfindingGridSetup Instance { private set; get; }

    public Grid<GridNode> pathfindingGrid;

    public LayerMask unwalkableMask;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        pathfindingGrid = new Grid<GridNode>(200, 200, 1f, new Vector3(-100, 0, -100), unwalkableMask, (Grid<GridNode> grid, int x, int y) => new GridNode(grid, x, y));
        CreateGrid();
    }

    void CreateGrid()
    {
        var originPosition = pathfindingGrid.GetOriginPosition();
        var width = pathfindingGrid.GetWidth();
        var height = pathfindingGrid.GetHeight();
        var cellSize = pathfindingGrid.GetCellSize();
        Vector3 worldBottomLeft = originPosition - Vector3.right * width / 2 - Vector3.forward * height / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * cellSize + cellSize / 2) + Vector3.forward * (y * cellSize + cellSize / 2);
                bool walkable = !(Physics.CheckSphere(worldPoint, cellSize / 2, pathfindingGrid.unwalkableMask));

                pathfindingGrid.GetGridObject(x, y).SetIsWalkable(walkable);
            }
        }
    }

}
