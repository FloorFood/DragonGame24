using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jobless : IProfession
{
    public ProfessionType Profession => ProfessionType.NONE;

    public Sprite Sprite { get; set; }

    public Jobless(Sprite sprite)
    {
        Sprite = sprite;
    }
    public void Perform(Creature self)
    {
        self.Roam();
    }
}
