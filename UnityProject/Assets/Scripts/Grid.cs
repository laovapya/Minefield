using System;
using System.Collections.Generic;
using UnityEngine;


public class Grid<TGridObject>
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    public int width { get; private set; }
    public int height { get; private set; }
    public float cellSize { get; private set; }
    public Vector2 originPosition { get; private set; }
    private TGridObject[,] array;

    public Grid(int width, int height, float cellSize, Vector2 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        array = new TGridObject[width, height];
    }

    public Vector2 GetCellCenter(Vector2 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return new Vector2(x, y) * cellSize + new Vector2(cellSize, cellSize) / 2 + originPosition;
    }
    public Vector2 GetCellCenter(int x, int y)
    {
        return new Vector2(x, y) * cellSize + new Vector2(cellSize, cellSize) / 2 + originPosition;
    }
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(x, y) * cellSize + originPosition;
    }

    public void GetXY(Vector2 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }
    public void GetClampedXY(Vector2 worldPosition, out int x, out int y)
    {
        GetXY(worldPosition, out x, out y);
        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);
    }

    public Vector2 GetClampedPosition(Vector2 worldPosition)
    {
        float x = Mathf.Clamp(worldPosition.x, originPosition.x, originPosition.x + width * cellSize);
        float y = Mathf.Clamp(worldPosition.y, originPosition.y, originPosition.y + height * cellSize);
        return new Vector2(x, y);
    }

    public void SetValue(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            array[x, y] = value;
            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
        }
    }

    public void SetValue(Vector2 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }
    public TGridObject GetValue(Vector2 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }
    public TGridObject GetValue(int x, int y)
    {
        if (IsInBounds(x, y))
        {
            return array[x, y];
        }
        else
        {
            return default(TGridObject);
        }
    }
    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }






}
