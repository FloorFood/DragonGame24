using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//[System.Serializable]
public class LogicPixel
{
    [HideInInspector]
    public PixelMatrix matrix;
    public int column, row;
    public bool empty;
    public bool hasEdges;

    [HideInInspector]
    public Shape shape;

    public Vector2 position;
    public List<Int2> unusedEdgePoints;

    public LogicPixel(int _column, int _row, bool _empty, PixelMatrix _matrix, Vector2 _worldOffset)
    {
        matrix = _matrix;
        column = _column;
        row = _row;
        empty = _empty;
        position = PixelToWorld(_worldOffset);
    }

    public LogicPixel(int _column, int _row, bool _empty, PixelMatrix _matrix)
    {
        matrix = _matrix;
        column = _column;
        row = _row;
        empty = _empty;
        position = PixelToWorld();
    }


    public bool CheckNeighBoursForShape()
    {
        bool hadToCreateNew = false;

        Shape neighbourShape;
        for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                if ((x == 0 || y == 0) && !(x == 0 && y == 0))
                {
                    if (matrix.CheckRangeAt(column + x, row + y))
                    {
                        neighbourShape = CheckShapeOf(matrix.pixels[column + x, row + y]);
                        CheckForMerge(neighbourShape);
                    }
                }
            }
        }

        if (shape == null)
        {
            shape = new Shape();
            if (!empty)
            {
                shape.volume.Add(this);
                shape.volumeCount++;
            }
            hadToCreateNew = true;
        }

        return hadToCreateNew;
    }

    public Shape CheckShapeOf(LogicPixel neighbour)
    {
        if (neighbour != null && neighbour.shape != null)
        {
            return neighbour.shape;
        }
        else
        {
            return null;
        }
    }

    public void CheckForMerge(Shape neighbourShape)
    {
        if (neighbourShape != null)
        {
            if (shape == null)
            {
                shape = neighbourShape;
                shape.volume.Add(this);
                shape.volumeCount++;
            }
            else if (shape != neighbourShape)
            {
                shape.volume.Add(this);
                shape.volumeCount++;
                shape.MergeIntoShape(neighbourShape);
            }
            //break;
        }
    }

    public Vector2 PixelToWorld(Vector2 offSet)
    {
        return new Vector2((column+0.5f) * matrix.pixelToUnit + offSet.x, (row + 0.5f) * matrix.pixelToUnit + offSet.y);
    }

    public Vector2 PixelToWorld()
    {
        return PixelToWorld( Vector2.zero);
    }

    public void SetEdgePoints()
    {
        if (empty)
            return;

        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if ((x == 0 || y == 0) && !(x == 0 && y == 0))
                {
                    if (matrix.CheckRangeAt(column + x, row + y))
                    {
                        //Debug.Log("[LogicPixel] "+"Checked " + (column + x) + ";" + (row + y));
                        if (matrix.pixels[column + x, row + y].empty)
                        {
                            if (unusedEdgePoints == null)
                                unusedEdgePoints = new List<Int2>();
                            unusedEdgePoints.Add(new Int2(x, y));
                        }
                    }
                    else
                    {
                        if (unusedEdgePoints == null)
                            unusedEdgePoints = new List<Int2>();
                        unusedEdgePoints.Add(new Int2(x, y));
                    }
                }
            }
        }
        if (unusedEdgePoints!=null && unusedEdgePoints.Count > 0) hasEdges = true;
        else hasEdges = false;

    }

    public LogicPixel GetNextEdgePixel(Int2 point, ref List<Vector2> edgePositions, out Int2 nextConnector)
    {
        //!travelrules!
        //we can not turn 180 degrees
        // -> if we have down we can only connect to down (same direction) left (90° clockwise) and right (90° counterclockwise)
        // we are checking diagonals as well
        // ->
        // we want to check clockwise
        LogicPixel nextPixel;
        Int2 currentConnector = point;
        nextConnector = Int2.zero;
        Vector2 nextConnectionPosition = Vector2.zero;
        while (unusedEdgePoints.Count > 0)
        {
            unusedEdgePoints.Remove(currentConnector);
            edgePositions.Add(position + currentConnector.ToVector() * 0.5f * matrix.pixelToUnit);
            if (CheckDirection(currentConnector, out nextPixel, ref nextConnector, ref nextConnectionPosition, ref edgePositions))
            {
                return nextPixel;
            }
            //connect to next CW90
            int circles = 0;
            while (unusedEdgePoints.Count > 0 && circles < 4)
            {
                //Debug.Log("[LogicPixel] "+"Turning (" + column + "," + row + ") CW (" + currentConnector.x + ";" + currentConnector.y + ") -> (" + currentConnector.TurnClockWise90().x + ";" + currentConnector.TurnClockWise90().y + ")");
                currentConnector = currentConnector.TurnClockWise90();
                if (unusedEdgePoints.Contains(currentConnector))
                {
                    break;
                }
                circles++;
            }
        }
        return null;
    }

    bool CheckDirection(Int2 point, out LogicPixel newPixel, ref Int2 nextConnector, ref Vector2 nextConnectionPosition, ref List<Vector2> edgePositions)
    {
        LogicPixel nextPixel;
        Int2 lookDirection = new Int2(-point.y, point.x);
        nextPixel = IsValidConnection(lookDirection, point, ref nextConnector, ref nextConnectionPosition);
        if (nextPixel != null && !edgePositions.Contains(nextConnectionPosition))
        {
            edgePositions.Add(nextConnectionPosition);
            newPixel = nextPixel;
            return true;
        }
        nextPixel = IsValidConnection(-lookDirection, point, ref nextConnector, ref nextConnectionPosition);
        if (nextPixel != null && !edgePositions.Contains(nextConnectionPosition))
        {
            edgePositions.Add(nextConnectionPosition);
            newPixel = nextPixel;
            return true;
        }
        nextPixel = IsValidConnection(lookDirection + point, point, ref nextConnector, ref nextConnectionPosition);
        if (nextPixel != null && !edgePositions.Contains(nextConnectionPosition))
        {
            edgePositions.Add(nextConnectionPosition);
            newPixel = nextPixel;
            return true;
        }
        nextPixel = IsValidConnection(-lookDirection + point, point, ref nextConnector, ref nextConnectionPosition);
        if (nextPixel != null && !edgePositions.Contains(nextConnectionPosition))
        {
            edgePositions.Add(nextConnectionPosition);
            newPixel = nextPixel;
            return true;
        }
        newPixel = null;
        return false;
    }

    public LogicPixel IsValidConnection(Int2 look, Int2 currentConnector, ref Int2 connector, ref Vector2 connectionPosition)
    {
        if (matrix.CheckRangeAt(column + look.x, row + look.y))
        {
            if (matrix.pixels[column + look.x, row + look.y].hasEdges)
            {
                LogicPixel check = matrix.pixels[column + look.x, row + look.y];
                connector = check.ConnectFrom(look, currentConnector);
                if (connector == Int2.zero)
                    return null;
                connectionPosition = connector.ToVector()*0.5f*matrix.pixelToUnit + check.position;
                return check;
            }
        }
        return null;
    }
    
    public Int2 ConnectFrom(Int2 direction, Int2 sendingConnector)
    {
        if (!direction.isDiagonal())
        {
            //straight connection always wants the same connector as sendingConnector
            foreach(Int2 conn in unusedEdgePoints)
            {
                if (conn == sendingConnector)
                    return conn;
            }
            return Int2.zero;
        }
        else
        {
            Int2 lookingFor = sendingConnector - direction;
            foreach (Int2 conn in unusedEdgePoints)
            {
                if (conn == lookingFor)
                    return conn;
            }
            return Int2.zero;
        }

    }

    public void DoForEveryNeighbour(Action toDo)
    {
        for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                if ((x == 0 || y == 0) && !(x == 0 && y == 0))
                {
                    if (matrix.CheckRangeAt(column + x, row + y))
                    {
                        toDo.Invoke();
                    }
                }
            }
        }
    }
}
