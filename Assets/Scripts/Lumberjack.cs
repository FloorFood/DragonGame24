using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lumberjack : IProfession
{
    public Sprite Sprite { get; set; }
    public ProfessionType Profession { get => ProfessionType.LUMBERJACK; }
    public Lumberjack(Sprite sprite)
    {
        Sprite = sprite;
    }
    public void Perform(Creature self)
    {
        //Look for wood to cut
    }
}
