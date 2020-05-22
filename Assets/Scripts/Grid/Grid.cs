using System;
using UnityEngine;
using Unity.Mathematics;

public class Grid<TGridObject> {

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    public LayerMask unwalkableMask { get; private set; }

    public Grid(int width, int height, float cellSize, Vector3 originPosition, LayerMask unwalkableMask, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.unwalkableMask = unwalkableMask;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetCellSize() {
        return cellSize;
    }

    public Vector3 GetOriginPosition()
    {
        return originPosition;
    }

    public float3 GetWorldPosition(int x, int y) {
        return new float3(x, 0, y) * cellSize + (float3)originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void GetXY(float3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - (float3)originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - (float3)originPosition).z / cellSize);
    }

    public void SetGridObject(int x, int y, TGridObject value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
        }
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            return gridArray[x, y];
        } else {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

}
