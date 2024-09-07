using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PixelMatrix
{
    public LogicPixel[,] pixels;
    public int width, height;
    public float pixelPerUnit;
    public float pixelToUnit;

    int logicPixelCount, volumePixelCount;

    public Shape[] shapes;

    

    public PixelMatrix(Texture2D _tex, Vector2 _worldOffset, float _pixelPerUnit = 100f)
    {
        width = _tex.width;
        height = _tex.height;
        pixelPerUnit = _pixelPerUnit;
        pixelToUnit = 1 / pixelPerUnit;
        pixels = new LogicPixel[width, height];

        SetupPixelAndShapes(_tex, _worldOffset);
        foreach(Shape s in shapes)
        {
            s.SetupEdges();
        }
    }
    public bool IsHole(Texture2D tex, int column, int row)
    {
        // Check if the current pixel is empty but is surrounded by filled pixels
        List<bool> filledNeighbor = new List<bool>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(column + x >= 0 && column + x < tex.width &&
                    row + y >= 0 && row + y < tex.height)
                {
                    if (tex.GetPixel(column + x, row + y).a < 0.1f)
                    {
                        filledNeighbor.Add(true);
                    }
                }
            }
        }
        return filledNeighbor.Count < 4; // Not a hole, or on the edge
    }

    void SetupPixelAndShapes(Texture2D _tex, Vector2 _worldOffset)
    {
        List<Shape> newShapes = new List<Shape>();
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                LogicPixel pixel = new LogicPixel(column, row, _tex.GetPixel(column, row).a < 0.1f, this, _worldOffset);
                if(IsHole(_tex, column, row))
                {
                    pixel.empty = false;
                }
                pixels[column, row] = pixel;

                logicPixelCount++;

                if (!pixel.empty)
                {
                    volumePixelCount++;
                    if (pixel.CheckNeighBoursForShape())
                    {
                        newShapes.Add(pixel.shape);
                    }
                }
            }
        }
        List<Shape> actualShapes = new List<Shape>();
        foreach (Shape s in newShapes)
        {
            if (s.volume.Count > 10)
            {
                actualShapes.Add(s);
            }
        }
        shapes = actualShapes.ToArray();
    }

    public bool CheckRangeAt(int _column, int _row)
    {
        return (_column < width && _column >= 0) && (_row < height && _row >= 0);
    }

    public bool CheckXRange(int _column)
    {
        return (_column + 1) < width && (_column - 1) >= 0;
    }

    public bool CheckYRange(int _row)
    {
        return (_row + 1) < height && (_row - 1) >= 0;
    }

}