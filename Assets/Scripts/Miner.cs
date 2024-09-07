using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : IProfession
{
    public Sprite Sprite { get; set; }
    public ProfessionType Profession { get => ProfessionType.MINER; }

    Vector2? positionToMine;
    MineableObject tileset;

    int rocksCarried;
    int carryingCapacity = 10;

    public Miner(Sprite sprite)
    {
        Sprite = sprite;
        positionToMine = null;
        rocksCarried = 0;
    }
    public void Perform(Creature self)
    {
        if(rocksCarried >= carryingCapacity) { return; }
        if(positionToMine == null)
        {
            if(self.DetectResource(ResourceType.ROCK, out GameObject target, out Vector2 position))
            {
                positionToMine = position;
                tileset = target.transform.parent.GetComponent<MineableObject>();
            }
            else
            {
                self.Roam();
            }
        }
        else if(MineResource())
        {
            rocksCarried++;
            positionToMine = null;
        }
        //Look for rocks to mine
    }
    bool MineResource()
    {
        return tileset.MinePosition(positionToMine.Value);
    }
}
