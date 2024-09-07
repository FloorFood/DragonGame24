using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHut : MonoBehaviour
{
    [SerializeField] Vector2Int spawnRate;

    CreatureManager creatureManager;
    Timer spawnTimer;
    public void Start()
    {
        creatureManager = CreatureManager.Instance;
        creatureManager.SpawnNewCreature(transform.position);
        spawnTimer = new Timer(spawnRate.x, () => { creatureManager.SpawnNewCreature(transform.position); }, spawnRate.y);
    }

    private void Update()
    {
        spawnTimer.Tick();
    }
}