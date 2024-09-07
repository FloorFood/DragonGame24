using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MineableObject: MonoBehaviour
{
    public ResourceType resourceType;

    SpriteRenderer spriteRenderer;
    TilemapRenderer tileMapRenderer;
    ShapeGenerator shapeGenerator;

    bool update = false;

    private void Awake()
    {
        shapeGenerator = GetComponent<ShapeGenerator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Texture2D copyTexture = new Texture2D(spriteRenderer.sprite.texture.width, spriteRenderer.sprite.texture.height);
        copyTexture.SetPixels(spriteRenderer.sprite.texture.GetPixels());
        copyTexture.Apply();
        Graphics.CopyTexture(copyTexture, spriteRenderer.sprite.texture);
        tileMapRenderer = GetComponentInChildren<TilemapRenderer>();
    }
    private void Start()
    {
        AdjustCollider();
    }
    private void Update()
    {
        if(update)
        {
            AdjustCollider();
        }
    }
    public bool MinePosition(Vector2 position)
    {
        if(spriteRenderer != null)
        {
            return MineSprite(position);
        }
        else if(tileMapRenderer != null)
        {
            return MineTileMap(position);
        }
        return false;
    }
    bool MineTileMap(Vector2 position)
    {
        return true;
    }
    public bool IsPositionMineable(ref Vector2 position)
    {
        var savedTexture = spriteRenderer.sprite.texture;
        Vector2Int pixelPosInt = WorldToPixel(position);

        List<Vector2Int> possiblePositions = new List<Vector2Int>();
        int reach = 5;
        for(int y = -reach; y <= reach; y++)
        {
            for(int x = -reach; x <= reach; x++)
            {
                Vector2Int candidate = new Vector2Int(pixelPosInt.x + x, pixelPosInt.y + y);
                if(candidate.y < savedTexture.height && candidate.y >= 0 &&
                    candidate.x < savedTexture.width && candidate.x >= 0 && (candidate - pixelPosInt).magnitude <= reach && 
                    savedTexture.GetPixel(candidate.x, candidate.y).a != 0)
                {
                    possiblePositions.Add(candidate);
                }
            }
        }
        if(possiblePositions.Count > 0)
        {
            position = PixelToWorld(possiblePositions[Random.Range(0, possiblePositions.Count)]);
            return true;
        }
        else
        {
            return false;
        }
    }
    Vector2Int WorldToPixel(Vector2 position)
    {
        float pixel2units = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.bounds.size.x;
        float temp1 = 1.0f / pixel2units;
        Vector2 startPos = transform.position - (spriteRenderer.sprite.bounds.size * 0.5f);
        Vector2 pixelPos = (position - startPos) / temp1;

        return new Vector2Int(Mathf.RoundToInt(pixelPos.x), Mathf.RoundToInt(pixelPos.y));
    }
    Vector2 PixelToWorld(Vector2Int position)
    {
        float pixel2units = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.bounds.size.x;
        float temp1 = 1.0f / pixel2units;
        Vector2 startPos = transform.position - (spriteRenderer.sprite.bounds.size * 0.5f);
        Vector2 worldPos = startPos + (new Vector2(position.x * temp1, position.y * temp1));

        return new Vector2(worldPos.x, worldPos.y);
    }
    bool MineSprite(Vector2 position)
    {
        var savedTexture = spriteRenderer.sprite.texture;
        Vector2Int pixelPosInt = WorldToPixel(position);
        savedTexture.SetPixel(pixelPosInt.x, pixelPosInt.y, Color.clear);
        savedTexture.Apply();

        update = true;

        return true;
    }
    void AdjustCollider()
    {
        var polygonCollider = GetComponent<PolygonCollider2D>();
        var sprite = spriteRenderer.sprite;

        shapeGenerator.input = sprite.texture;
        shapeGenerator.UpdateShape(sprite.texture);
        //shapeGenerator.GenerateOnThis();

       /* polygonCollider.pathCount = 0;
        polygonCollider.pathCount = sprite.GetPhysicsShapeCount();

        List<Vector2> path = new List<Vector2>();
        for (int i = 0; i < polygonCollider.pathCount; i++)
        {
            path.Clear();
            sprite.GetPhysicsShape(i, path);
            for (int j = 0; j < path.Count; j++)
            {
                path[j] -= path[j].normalized * 0.01f;
            }
            polygonCollider.SetPath(i, path.ToArray());
        }*/
    }
}
