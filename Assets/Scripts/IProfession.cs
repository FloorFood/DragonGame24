using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProfession
{
    public ProfessionType Profession {get;}
    public Sprite Sprite { get; set; }
    public void Perform(Creature self);
}
