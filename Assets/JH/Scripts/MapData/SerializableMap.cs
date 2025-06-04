using System;
using UnityEngine;

public enum TileType
{
    None = 0,
    Wall = 1,
    Spike = 2,
    BouncePad = 3,
    Turrnt = 4,
}

[Serializable]
public class SerializableMap
{
    public int width;
    public int height;
    public int[] data;

    public SerializableMap(TileType[,] grid)
    {
        width = grid.GetLength(0);
        height = grid.GetLength(1);
        data = new int[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                data[y * width + x] = (int)grid[x, y];
            }
        }
    }

    public TileType[,] To2DArray()
    {
        TileType[,] result = new TileType[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                result[x, y] = (TileType)data[y * width + x];
            }
        }
        return result;
    }
}
