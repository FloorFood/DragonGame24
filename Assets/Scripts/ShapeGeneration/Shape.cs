using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Shape
{
    public List<LogicPixel> volume = new List<LogicPixel>();
    public int volumeCount;

    public List<Edge> edges = new List<Edge>();

    public void MergeIntoShape(Shape otherShape)
    {
        //.................volúme pixel
        otherShape.volume.AddRange(volume);
        otherShape.volumeCount += volume.Count;
        foreach (LogicPixel pix in volume)
        {
            pix.shape = otherShape;
        }
        //if (GenerateShapes.useCreationLog)
        //    Debug.Log("[Shape] "+"Transfered " + volume.Count + " pixel to otherShape");
        volume.Clear();
        volumeCount = 0;
    }

    public void SetupEdges()
    {
        List<LogicPixel> edgePixel = new List<LogicPixel>();
        foreach(LogicPixel pix in volume)
        {
            pix.SetEdgePoints();
            if (pix.unusedEdgePoints!=null && pix.unusedEdgePoints.Count > 0)
            {
                edgePixel.Add(pix);
            }
        }
        //Debug.Log("[Shape] "+"Has " + edgePixel.Count + " pixels in edge");

        TravelEdge(edgePixel);
    }

    public void TravelEdge(List<LogicPixel> edgePixel)
    {
        Edge newEdge = new Edge();
        LogicPixel startPixel = edgePixel[0];
        LogicPixel thisPixel = startPixel;
        Int2 connector = startPixel.unusedEdgePoints[0];
        newEdge.vertices.Add(thisPixel.position + 0.5f * connector.ToVector() * thisPixel.matrix.pixelToUnit);
        while (edgePixel.Count > 0 )//&& thisPixel != startPixel)
        {
            //Debug.Log("[Shape] "+edgePixel.Count + " edgepixel left");
            newEdge.pixel.Add(thisPixel);
            
            edgePixel.Remove(thisPixel);
            if (thisPixel == null)
                break;
            thisPixel = thisPixel.GetNextEdgePixel(connector,ref newEdge.vertices,out connector);
            if (thisPixel == null)
            {
                //Debug.Log("[Shape] "+"Found no further edge-connection");
                //Debug.Log("[Shape] "+"No case for multiple edges in one shape yet");
                //break;
                if (edgePixel.Count > 1 && startPixel.unusedEdgePoints.Count > 0)
                {
                    edges.Add(newEdge);
                    newEdge = new Edge();
                    startPixel = edgePixel[1];
                    thisPixel = startPixel;
                    connector = startPixel.unusedEdgePoints[0];
                    newEdge.vertices.Add(thisPixel.position + 0.5f * connector.ToVector() * thisPixel.matrix.pixelToUnit);
                }
            }
            //Debug.Log("[Shape] "+"Next edgepixel: (" + thisPixel.column + ";" + thisPixel.row + ")");
        }
        //newEdge.vertices.Add(newEdge.vertices[0]);
        edges.Add(newEdge);
    }

    public void DestructiveSmoothEdges(float smoothingThreshold)
    {
        foreach (var edge in edges)
        {
            //Debug.Log("[Shape] "+"Edges before destructive smoothing: " + edge.vertices.Count);
            var edgePositions = edge.vertices;
            int i = 1;
            while (i < edgePositions.Count - 1)
            {
                if ((edgePositions[i] - edgePositions[i - 1]).magnitude < smoothingThreshold)
                {
                    edgePositions.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            //Debug.Log("[Shape] "+"Edges after destructive smoothing should be: " + edgePositions.Count);
            edge.vertices = edgePositions;
            //Debug.Log("[Shape] "+"Edges after destructive smoothing are: " + edge.vertices.Count);
        }
    }

    public void ReduceEdges(float normalThreshold = 0.95f)
    {

        foreach (var edge in edges)
        {
            //Debug.Log("[Shape] "+"Edges before normalReduction: " + edge.vertices.Count);
            var edgePositions = edge.vertices;
            int i = 1;
            while (i < edgePositions.Count - 1)
            {
                Vector2 normalLeft = GetNormal(edgePositions[i - 1], edgePositions[i]);
                Vector2 normalRight = GetNormal(edgePositions[i], edgePositions[i + 1]);

                if (Vector2.Dot(normalLeft, normalRight) >= normalThreshold)
                {
                    //if (GenerateShapes.useCreationLog) Debug.Log("[Shape] "+"Removed " + i + " normalvalue was " + Vector2.Dot(normalLeft, normalRight));
                    edgePositions.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            //Debug.Log("[Shape] "+"Edges after normalReduction should be: " + edgePositions.Count);
            edge.vertices = edgePositions;
            //Debug.Log("[Shape] "+"Edges after normalReduction are: " + edge.vertices.Count);
        }
    }

    public Vector2 GetNormal(Vector2 point1, Vector2 point2, bool direction = true)
    {
        float dx = point2.x - point1.x;
        float dy = point2.y - point1.y;

        if (direction)
            return (new Vector2(-dy, dx).normalized); // or dy,-dx
        else
            return (new Vector2(dy, -dx).normalized);
    }

    public Edge GetLargestEdge()
    {
        float maxX = float.NegativeInfinity;
        Edge maxEdge=null;
        foreach(var e in edges)
        {
            foreach(var vert in e.vertices)
            {
                if (vert.x > maxX)
                {
                    maxX = vert.x;
                    maxEdge = e;
                }
            }
        }
        return maxEdge;
    }

}

[System.Serializable]
public class Edge
{
    public List<LogicPixel> pixel = new List<LogicPixel>();
    public List<Vector2> vertices = new List<Vector2>();
    //public int count;
}