using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    private InputActions input = null;
    [SerializeField] GameObject RaiderPrefab;

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
    public bool RaidStarted;
    public bool Tutorial = true;
    [SerializeField] GameObject middleFrame;
    [SerializeField] GameObject raidProgessText;
    [SerializeField] GameObject bossFrame;

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
    }
    
    public void StartRaid()
    {
        SoundManager.Instance.PlaySound("BGMMusic", 1f);
        if (Tutorial)
        {
            BossMechanics.Instance.BossHP = 300;
        }

        bossFrame.SetActive(true);
        Tutorial = false;
        raidProgessText.SetActive(true);
        TextDisplay.Instance.CookTextBlocks();

        Debug.Log("start");
        Vector2 spawnPoint = new Vector2(0, -4.5f);

        for (int i = 0; i < 2; i++)
        {
            GameObject gob = Instantiate(RaiderPrefab, spawnPoint, Quaternion.identity);
            AllRaiders.Add(gob.GetComponent<Raider>());
            gob.GetComponent<Raider>().Role = 0;
        }
        for (int i = 0; i < 9; i++)
        {
            GameObject gob = Instantiate(RaiderPrefab, spawnPoint, Quaternion.identity);
            AllRaiders.Add(gob.GetComponent<Raider>());
            gob.GetComponent<Raider>().Role = 1;
        }
        for (int i = 0; i < 4; i++)
        {
            GameObject gob = Instantiate(RaiderPrefab, spawnPoint, Quaternion.identity);
            AllRaiders.Add(gob.GetComponent<Raider>());
            gob.GetComponent<Raider>().Role = 2;
        }


        foreach (Raider raider in AllRaiders)
        {
            raider.CurrentStackZone = null;
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

            raider.UpdateLowestCostStackZone();
            raider.MoveToNewStackZone(raider.LowestCostStackZone);
            raider.Ready = false;
            BossMechanics.Instance.Ready = true;
            RaidStarted = true;
        }
    }

    void NewTurn()
    {
        if (!BossMechanics.Instance.Ready) return;
        foreach (Raider raider in AllRaiders)
        {
            if (!raider.Ready) return;
        }

        BossMechanics.Instance.ChangeBossEnergy(10);
        Debug.Log("New turn! Energy: " + BossMechanics.Instance.BossEnergy +
            " Health: " + (float)((float)BossMechanics.Instance.BossHP / (float)BossMechanics.Instance.BossHPMax) * 100);

        foreach (Raider raider in AllRaiders)
        {
            raider.Ready = false;
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

        BossMechanics.Instance.Ready = false;
        BossMechanics.Instance.Invoke("PerformBossAction", 2.5f);
    }

    void Update()
    {
        if (RaidStarted)
        {
            NewTurn();
        }

        if (input.PlayerController.Enter.WasPressedThisFrame())
        {
            StartRaid();
        }

    }

    public void BossDefeated()
    {
        if (!RaidStarted) return;
        RaidStarted = false;
        SoundManager.Instance.PlaySound("BossDefeated");
        Invoke("TransitionDefeat", 3.0f);
        SoundManager.Instance.EndSound("BGMMusic", 3.0f);
    }

    void TransitionDefeat()
    {
        ResetWorld();
        middleFrame.SetActive(true);
        middleFrame.GetComponent<UICenterFrameController>().ShowDefeatScreen();
        CameraManager.Instance.PanToDeskScreen();
    }

    public void RaidWiped()
    {
        if (!RaidStarted) return;
        if (KilledRaiders.Count < 15) return;
        RaidStarted = false;
        SoundManager.Instance.PlaySound("Victory");
        Invoke("TransitionVictory", 3.0f);
        SoundManager.Instance.EndSound("BGMMusic", 3.0f);
    }

    void TransitionVictory()
    {
        ResetWorld();
        middleFrame.SetActive(true);
        middleFrame.GetComponent<UICenterFrameController>().ShowVictoryScreen();
        CameraManager.Instance.PanToDeskScreen();
    }



    void ResetWorld()
    {
        raidProgessText.SetActive(false);
        bossFrame.SetActive(false);

        foreach (Raider raider in AllRaiders)
        {
            Destroy(raider.gameObject);
        }
        AllRaiders.Clear();
        foreach (GameObject raider in KilledRaiders)
        {
            Destroy(raider.gameObject);
        }
        KilledRaiders.Clear();
        BossMechanics.Instance.BossHP = BossMechanics.Instance.BossHPMax;
        BossMechanics.Instance.BossEnergy = 0;
        foreach (StackZone stackZone in StackZones)
        {
            stackZone.UnitsInZone.Clear();
        }
        TextDisplay.Instance.InitializePage();
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
            if (AllRaiders.Contains(raider.GetComponent<Raider>()))
            {
                RemoveRaiderFromAllStacks(raider);
                AllRaiders.Remove(raider.GetComponent<Raider>());
            } 
        }
    }

}
