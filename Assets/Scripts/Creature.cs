using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class Creature : MonoBehaviour
{
    [SerializeField] SpriteRenderer body;

    IProfession profession;
    Action<Vector2> onDeath;

    Timer walkTimer;
    Timer chillTimer;
    public bool walking;
    int directionToWalk;

    private void Awake()
    {
        profession = null;
        walkTimer = new Timer(2, () => { walking = false; }, 3);
        chillTimer = new Timer(1, () => { walking = true; ChooseDirection(); }, 1.4f);
    }
    private void Update()
    {
        profession?.Perform(this);
    }
    public void SpawnIn(Action<Vector2> OnDeath)
    {
        onDeath = OnDeath;
    }
    public void SetProfession(IProfession profession)
    {
        this.profession = profession;
    }
    public void Die()
    {
        onDeath(transform.position);
    }
    public void ChooseDirection()
    {
        int[] directions = { -1, 1 };
        directionToWalk = directions[Random.Range(0, 2)];
    }
    public void Roam()
    {
        if (walking)
        {
            walkTimer.Tick();
        }
        else
        {
            chillTimer.Tick();
        }
    }
}
