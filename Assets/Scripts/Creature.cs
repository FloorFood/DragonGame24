using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class Creature : MonoBehaviour
{
    [SerializeField] SpriteRenderer body;
    [SerializeField] float speed;

    Rigidbody2D rigidBody;
    IProfession profession;
    Action<Vector2> onDeath;

    Timer walkTimer;
    Timer chillTimer;
    public bool walking;
    int directionToWalk;
    float detectionRadius = 2.2f;

    private void Awake()
    {
        profession = null;
        walkTimer = new Timer(2, () => { walking = false; }, 3);
        chillTimer = new Timer(1, () => { walking = true; ChooseDirection(); }, 1.4f);
        rigidBody = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        profession?.Perform(this);
        Move();
    }
    public void Move()
    {
        if(walking)
        {
            rigidBody.AddForce(new Vector2(directionToWalk * speed * Time.deltaTime, 0), ForceMode2D.Impulse);
        }
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
    public bool DetectResource(ResourceType type,out GameObject target, out Vector2 position)
    {
        position = Vector2.zero;
        target = null;
        var hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        var hit = hits.FirstOrDefault(h => { return h.transform.parent.GetComponent<MineableObject>() && h.transform.parent.GetComponent<MineableObject>().resourceType == type; });
        if (hit != null)
        {
            position = hit.ClosestPoint(transform.position);
            MineableObject mineable = hit.transform.parent.GetComponent<MineableObject>();
            if (mineable.IsPositionMineable(ref position))
            {
                target = hit.gameObject;
                return true;
            }
        }

        return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
