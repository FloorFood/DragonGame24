using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridgeman : IProfession
{
    public Sprite Sprite { get; set; }
    public ProfessionType Profession { get => ProfessionType.BRIDGEMAN; }
    public Bridgeman(Sprite sprite)
    {
        Sprite = sprite;
    }
    public void Perform(Creature self)
    {
        //Fetch wood and build bridge
    }
}
