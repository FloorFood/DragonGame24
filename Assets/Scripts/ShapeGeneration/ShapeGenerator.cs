using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ShapeGenerator : MonoBehaviour
{
    public UnityEngine.UI.RawImage alternativeSource;
    public Texture2D input;
    public float size = 100f;
    public bool useReduction = true;
    [Range(0f,1f)]
    public float normalThreshold = 0.975f;
    public float destructiveSmoothing = 3f;
    public bool generateOnStart = false;
    public bool showSprite = false;
    public bool generateMesh = false;
    public bool createNewSharedMaterial = false;
    public bool addRigidbody = false;
    public Material shapeMaterial;
    public bool trigger;

    public PixelMatrix matrix;

    List<PolygonCollider2D> cashedColliders = new List<PolygonCollider2D>();

    private void Start()
    {
        if(generateOnStart && input)
            GenerateShape(null,Vector2.zero);
    }

    public void GenerateFromRawImage()
    {
        GenerateShape(alternativeSource.mainTexture as Texture2D,new Vector2(-0.5f*alternativeSource.mainTexture.width/100f,-0.5f*alternativeSource.mainTexture.height/100f));
    }

    [ContextMenu("Generate")]
    public void GenerateShape()
    {
        GenerateShape(null,Vector2.zero);
    }

    [ContextMenu("Generate regular polygoncollider")]
    public void GenerateOnThis()
    {
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        if (rend == null || rend.sprite == null)
            return;

        if (!rend.sprite.texture.isReadable)
        {
            Debug.LogWarning("[ShapeGenerator] Texture needs read/write enabled in import settings.", this.gameObject);
            return;
        }
        //new Vector2( -(rend.sprite.pivot.x/rend.sprite.texture.width), -(rend.sprite.pivot.y/rend.sprite.texture.height))
        matrix = new PixelMatrix(rend.sprite.texture, -rend.sprite.pivot/rend.sprite.pixelsPerUnit, rend.sprite.pixelsPerUnit);
        if (useReduction)
        {
            foreach (var shape in matrix.shapes)
            {
                //Debug.Log("[ShapeGenerator] "+"------------------------------------------------------");
                //shape.ReduceEdges(reductionThreshold);
                if (destructiveSmoothing > 0f)
                {
                    shape.DestructiveSmoothEdges(destructiveSmoothing * matrix.pixelToUnit);
                }
                shape.ReduceEdges(normalThreshold);
            }
        }

        Debug.Log("[ShapeGenerator] "+rend.sprite.pivot);

        GeneratePolygonCollider();
        matrix = null;
    }

    public void GenerateShape(Texture2D tex, Vector2 _offset)
    {
        if (tex == null)
            tex = input;
        matrix = new PixelMatrix(tex, _offset, size);

        if (useReduction)
        {
            foreach (var shape in matrix.shapes)
            {
                //Debug.Log("[ShapeGenerator] "+"------------------------------------------------------");
                //shape.ReduceEdges(reductionThreshold);
                if (destructiveSmoothing > 0f)
                {
                    shape.DestructiveSmoothEdges(destructiveSmoothing*matrix.pixelToUnit);
                }
                shape.ReduceEdges(normalThreshold);
            }
        }

        GeneratePolygon2D(generateMesh,(1/(float)matrix.width)*matrix.pixelPerUnit, (1 / (float)matrix.height) * matrix.pixelPerUnit,tex,addRigidbody,_offset);


        if (showSprite)
            ShowSprite(tex);
        else
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Destroy(renderer);
            }
        }
    }
    public void UpdateShape(Texture2D tex)
    {
        matrix = new PixelMatrix(tex, Vector2.zero, size);

        if (useReduction)
        {
            foreach (var shape in matrix.shapes)
            {
                //Debug.Log("[ShapeGenerator] "+"------------------------------------------------------");
                //shape.ReduceEdges(reductionThreshold);
                if (destructiveSmoothing > 0f)
                {
                    shape.DestructiveSmoothEdges(destructiveSmoothing * matrix.pixelToUnit);
                }
                shape.ReduceEdges(normalThreshold);
            }
        }

        UpdatePolygon2D(generateMesh, (1 / (float)matrix.width) * matrix.pixelPerUnit, (1 / (float)matrix.height) * matrix.pixelPerUnit, tex, addRigidbody, Vector2.zero);
    }

    void ShowSprite(Texture2D tex)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = gameObject.AddComponent<SpriteRenderer>();
        }
        //renderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 0f), matrix.pixelPerUnit, 0, SpriteMeshType.FullRect);
    }

    void GeneratePolygonCollider()
    {
        PolygonCollider2D poly = gameObject.AddComponent<PolygonCollider2D>();
        int j = 0;
        foreach (Shape s in matrix.shapes)
        {
            if (s.edges.Count > 0)
            {
                for (int i = 0; i < s.edges.Count; i++)
                {
                    poly.SetPath(j, s.edges[i].vertices.ToArray());
                    j++;
                }
            }
        }
    }

    void GeneratePolygon2D(bool addGraphic = false, float uMultiplier = 1f, float vMultiplier = 1f, Texture2D tex = null, bool _addPhysics = false, Vector2 _offset = default(Vector2))
    {
        foreach(Shape s in matrix.shapes)
        {
            GameObject clone = new GameObject();
            clone.name = "CollisionObject";
            clone.transform.SetParent(transform);
            clone.transform.localPosition = new Vector2(-(input.width / 200f), -(input.height / 200f));
            var polygonColl = clone.AddComponent<PolygonCollider2D>();
            polygonColl.isTrigger = trigger;
            cashedColliders.Add(polygonColl);
            polygonColl.pathCount = s.edges.Count;
            if (s.edges.Count>0)
            {
                for(int i=0; i < s.edges.Count;i++)
                {
                    if (s.edges[i].vertices.Count >= 3) // At least a triangle
                    {
                        polygonColl.SetPath(i, s.edges[i].vertices.ToArray());
                    }
                }
            }
                

            if (addGraphic)
            {
                MeshFilter filter = clone.AddComponent<MeshFilter>();

                Mesh newMesh = GeneratePolygonMesh(s.GetLargestEdge().vertices.ToArray(),uMultiplier,vMultiplier,_offset);

                filter.mesh = newMesh;
                
                MeshRenderer renderer = clone.AddComponent<MeshRenderer>();
                if (createNewSharedMaterial)
                {
                    
                }
                else
                {
                    renderer.material = shapeMaterial;
                }
                if (tex != null)
                {
                    if (createNewSharedMaterial)
                    {
                        renderer.sharedMaterial.mainTexture = tex;
                    }
                    else
                    {
                        Texture2D cloneTex = new Texture2D(tex.width, tex.height);
                        cloneTex.SetPixels(tex.GetPixels());
                        cloneTex.Apply();
                        renderer.material.mainTexture = cloneTex;
                    }
                }
            }
            if (_addPhysics)
            {
                clone.AddComponent<Rigidbody2D>();
            }
        }
    }
    void UpdatePolygon2D(bool addGraphic = false, float uMultiplier = 1f, float vMultiplier = 1f, Texture2D tex = null, bool _addPhysics = false, Vector2 _offset = default(Vector2))
    {
        for(int i = 0; i < cashedColliders.Count; i++)
        {
            Destroy(cashedColliders[i].gameObject);
        }
        cashedColliders.Clear();
        for(int i = 0; i < matrix.shapes.Length; i++)
        {
            Shape s = matrix.shapes[i];
            GameObject clone = new GameObject();
            clone.name = "CollisionObject";
            clone.transform.SetParent(transform);
            clone.transform.localPosition = new Vector2(-(input.width / 200f), -(input.height / 200f));
            var polygonColl = clone.AddComponent<PolygonCollider2D>();
            polygonColl.isTrigger = trigger;
            cashedColliders.Add(polygonColl);
            polygonColl.pathCount = s.edges.Count;
            if (s.edges.Count > 0)
            {
                for (int j = 0; j < s.edges.Count; j++)
                {
                    if (s.edges[j].vertices.Count >= 3) // At least a triangle
                    {
                        polygonColl.SetPath(j, s.edges[j].vertices.ToArray());
                    }
                }
            }
        }
    }

    public static Mesh GeneratePolygonMesh(Vector2[] verts, float uMultiplier, float vMultiplier, Vector2 _offset = default)
    {
        Triangulator tri = new Triangulator(verts);
        int[] tris = tri.Triangulate();

        Mesh newMesh = new Mesh();
        newMesh.vertices = System.Array.ConvertAll<Vector2, Vector3>(verts, v => v);

        /*//----------inverse normals--------
        newMesh.normals = new Vector3[verts.Length];
        for (int i = 0; i < newMesh.normals.Length; i++)
        {
            newMesh.normals[i] = Vector3.back;
        }

        for (int i = 0; i < tris.Length; i += 3)
        {
            int temp = tris[i + 0];
            tris[i + 0] = tris[i + 1];
            tris[i + 1] = temp;
        }
        //------------------------------------*/

        Vector2[] uvs = new Vector2[newMesh.vertices.Length];
        //Debug.Log("[ShapeGenerator] "+newMesh.vertices.Length);
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2((verts[i].x + _offset.x) * uMultiplier, (verts[i].y + _offset.y) * vMultiplier);
            //Debug.Log("[ShapeGenerator] "+"UV for " + newMesh.vertices[i] + " is " + uvs[i]);

        }

        newMesh.uv = uvs;
        newMesh.triangles = tris;

        /*newMesh.RecalculateNormals();
        for (int i = 0; i < newMesh.normals.Length; i++)
        {
            //Debug.Log("[ShapeGenerator] "+newMesh.normals[i]);
            newMesh.normals[i] = Vector3.back;
        }*/

        return newMesh;
    }



    private void OnDrawGizmosSelected()
    {
        if (matrix != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            if (matrix.shapes?.Length > 0)
            {
                foreach(var s in matrix.shapes)
                    if (s.edges.Count > 0)
                    {
                        foreach(var e in s.edges)
                            for(int i=1; i <e.vertices.Count;i++)
                            {
                                Gizmos.DrawLine(e.vertices[i - 1], e.vertices[i]);
                            }
                    }
            }
        }
    }


}
