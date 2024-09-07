using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreatureManager : MonoBehaviour
{
    [SerializeField] Creature creaturePrefab;
    [SerializeField] ProfessionConfig professionConfig;
    [SerializeField] Creature currentlyHoveredCreature;
    [SerializeField] Creature currentlySelectedCreature;
    [SerializeField] GameObject dragon;

    List<IProfession> professions = new List<IProfession>();
    List<Creature> activeCreatures = new List<Creature>();

    private Transform dragging = null;
    private Vector3 offset;
    static CreatureManager instance;
    public static CreatureManager Instance
    {
        get => instance; private set => instance = value;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        if(professionConfig.GetEntry(ProfessionType.NONE, out var entry))
        {
            professions.Add(new Jobless(entry.sprite));
        }
        if (professionConfig.GetEntry(ProfessionType.MINER, out entry))
        {
            professions.Add(new Miner(entry.sprite));
        }
        if (professionConfig.GetEntry(ProfessionType.LUMBERJACK, out entry))
        {
            professions.Add(new Lumberjack(entry.sprite));
        }
        if (professionConfig.GetEntry(ProfessionType.BRIDGEMAN, out entry))
        {
            professions.Add(new Bridgeman(entry.sprite));
        }
    }
    private void Update()
    {
        HoverCreature();
        /** BoxCollider2D drag_coll = dragon.GetComponent<BoxCollider2D>();
        if (currentlySelectedCreature != null && currentlySelectedCreature.GetComponent<CircleCollider2D>().bounds.Intersects(drag_coll.bounds))
        {
            dragging = null;
            currentlySelectedCreature.GetComponent<GameObject>().SetActive(false);
        }**/
    }
    public void HoverCreature()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hits = Physics2D.OverlapCircleAll(mousePosition, 0.2f);
        if (hits.Length > 0 && hits[0].TryGetComponent(out Creature creature))
        {
            currentlyHoveredCreature = creature;
            if (Input.GetMouseButtonDown(0))
            {
                currentlySelectedCreature = creature;
                dragging = currentlySelectedCreature.transform;
                // And record the offset.
                offset = dragging.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0)) 
            {
                // Stop dragging.
                dragging = null;
            }

            if (dragging != null) 
            {
                // Move object, taking into account original offset.
                dragging.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
                currentlySelectedCreature.GetComponent<Animator>().Play("Base Layer.Creature_Walk");
            }
        }
        else
        {
            currentlyHoveredCreature = null;
        }
    }

    public void SpawnNewCreature()
    {
        if(activeCreatures.Count < 200)
        {
            var creature = Instantiate(creaturePrefab, transform);
            creature.SpawnIn(OnCreatureDeath);
            creature.SetProfession(professions.FirstOrDefault(p => p.Profession == ProfessionType.NONE));
            activeCreatures.Add(creature);
        }
    }
    public void AssignProfession(ProfessionType type)
    {
        currentlySelectedCreature.SetProfession(professions.FirstOrDefault(p => p.Profession == type));
    }
    public void OnCreatureDeath(Vector2 position)
    {

    }
}
