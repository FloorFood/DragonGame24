using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreatureManager : MonoBehaviour
{
    [SerializeField] Creature creaturePrefab;
    [SerializeField] ProfessionConfig professionConfig;
    [SerializeField] Creature currentlyHoveredCreature;
    [SerializeField] Creature currentlySelectedCreature;
    [SerializeField] List<Button> professionButtons;

    List<IProfession> professions = new List<IProfession>();
    List<Creature> activeCreatures = new List<Creature>();

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
            professionButtons[1].onClick.AddListener(() => { AssignProfession(ProfessionType.MINER); });
        }
        if (professionConfig.GetEntry(ProfessionType.LUMBERJACK, out entry))
        {
            professions.Add(new Lumberjack(entry.sprite));
            professionButtons[0].onClick.AddListener(() => { AssignProfession(ProfessionType.LUMBERJACK); });
        }
        if (professionConfig.GetEntry(ProfessionType.BRIDGEMAN, out entry))
        {
            professions.Add(new Bridgeman(entry.sprite));
            professionButtons[2].onClick.AddListener(() => { AssignProfession(ProfessionType.BRIDGEMAN); });
        }
    }
    private void Update()
    {
        HoverCreature();
    }
    public void HoverCreature()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hits = Physics2D.OverlapCircleAll(mousePosition, 0.2f);
        if (hits.Length > 0 && hits[0].TryGetComponent(out Creature creature))
        {
            currentlyHoveredCreature = creature;
        }
        else
        {
            currentlyHoveredCreature = null;
        }
    }

    public void SpawnNewCreature(Vector2 position)
    {
        if(activeCreatures.Count < 200)
        {
            var creature = Instantiate(creaturePrefab, position, Quaternion.identity, transform);
            creature.SpawnIn(OnCreatureDeath);
            creature.SetProfession(professions.FirstOrDefault(p => p.Profession == ProfessionType.LUMBERJACK));
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
