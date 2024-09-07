using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : IProfession
{
    public Sprite Sprite { get; set; }
    public ProfessionType Profession { get => ProfessionType.MINER; }

    public Miner(Sprite sprite)
    {
        Sprite = sprite;
    }
    public void Perform(Creature self)
    {
        //Look for rocks to mine
    }
}
