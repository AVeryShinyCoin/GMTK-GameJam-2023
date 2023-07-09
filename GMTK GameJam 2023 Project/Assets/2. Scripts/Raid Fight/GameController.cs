using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    private InputActions input = null;

    public List<StackZone> StackZones = new List<StackZone>();
    [HideInInspector] public StackZone FrontStackZone;
    [HideInInspector] public StackZone BackStackZone;
    [HideInInspector] public StackZone LeftStackZone;
    [HideInInspector] public StackZone RightStackZone;
    [HideInInspector] public StackZone OuterStackZone;

    public List<Raider> AllRaiders = new List<Raider>();

    public List<GameObject> KilledRaiders = new List<GameObject>();

    [Space(20)]
    public List<BasicCondition> BasicConditions = new List<BasicCondition>();
    public List<EnergyCondition> EnergyConditions = new List<EnergyCondition>();
    public List<HealthCondition> HealthConditions = new List<HealthCondition>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        input = new InputActions();

        FrontStackZone = StackZones[0];
        BackStackZone = StackZones[1];
        LeftStackZone = StackZones[2];
        RightStackZone = StackZones[3];
        OuterStackZone = StackZones[4];
    }
    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        //SoundManager.Instance.PlaySound("BGMMusic", 1f);
    }

    void NewTurn()
    {
        BossMechanics.Instance.ChangeBossEnergy(10);
        Debug.Log("New turn! Energy: " + BossMechanics.Instance.BossEnergy +
            " Health: " + (float)((float)BossMechanics.Instance.BossHP / (float)BossMechanics.Instance.BossHPMax) * 100);

        foreach (Raider raider in AllRaiders)
        {
            raider.ZoneCosts.Clear();
            
            foreach (StackZone stackZone in StackZones) // baseline 500 cost to all zones
            {
                raider.AddCostToZone(stackZone, 500);
            }

            foreach (BasicCondition condition in BasicConditions)
            {
                condition.ApplyConditionCosts(raider);
            }
            foreach (EnergyCondition condition in EnergyConditions)
            {
                condition.ApplyConditionCosts(raider);
            }
            foreach (HealthCondition condition in HealthConditions)
            {
                condition.ApplyConditionCosts(raider);
            }

            raider.Invoke("PerformAction", Random.Range(0f, 1.5f));
        }
    }

    void Update()
    {
        
        if (input.PlayerController.R.WasPressedThisFrame())
        {
            foreach (Raider raider in AllRaiders)
            {
                raider.MoveToNewStackZone(StackZones[Random.Range(0, StackZones.Count)]);
            }
        }

        if (input.PlayerController.Space.WasPressedThisFrame())
        {
            NewTurn();
        }

        if (input.PlayerController.Enter.WasPressedThisFrame())
        {
            TextDisplay.Instance.CookTextBlocks();
            Debug.Log("BASIC CONDITIONS " + BasicConditions.Count);
            Debug.Log("ENERGY CONDITIONS " + EnergyConditions.Count);
            Debug.Log("HEALTH CONDITIONS " + HealthConditions.Count);
        }

    }


    public void RemoveRaiderFromAllStacks(GameObject raider)
    {
        foreach (StackZone stackZone in StackZones)
        {
            if (stackZone.UnitsInZone.Contains(raider)) stackZone.UnitsInZone.Remove(raider);
        }
    }

    public void CullDeadRaidersFromAllStacks()
    {
        foreach (GameObject raider in KilledRaiders)
        {
            RemoveRaiderFromAllStacks(raider);
            AllRaiders.Remove(raider.GetComponent<Raider>());
            Destroy(raider);
        }
        KilledRaiders.Clear();
    }

}
