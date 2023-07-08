using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Raider : MonoBehaviour
{
    public int Role; // 0 = damage dealer, 1 = tank, 2 = healer

    [Space(20)]
    [SerializeField] int DDHitPoints;
    [SerializeField] int TankHitPoints;
    [SerializeField] int HealerHitPoints;
    [SerializeField] int DDamageDealt;
    [SerializeField] int TankDamageDealt;
    [SerializeField] int HealerHealthRestore;

    [Space(20)]
    public int MaxHitPoints;
    public int HitPoints;
    public StackZone CurrentStackZone;
    public bool Stunned;

    [SerializeField] float speed;
    bool moving;
    Vector2 tarDestination;

    [Space(20)]
    public Dictionary<StackZone, int> ZoneCosts = new Dictionary<StackZone, int>();

    private void Awake()
    {
        if (Role == 0)
        {
            MaxHitPoints = DDHitPoints;
            transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "D";
        } else if (Role == 1)
        {
            MaxHitPoints = TankHitPoints;
            transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "T";
        }
        else if (Role == 2)
        {
            MaxHitPoints = HealerHitPoints;
            transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "H";
        }

        HitPoints = MaxHitPoints;
    }

    private void Start()
    {
        GameController.Instance.AllRaiders.Add(this);
    }

    public void PerformAction()
    {
        if (Stunned) // if raider is stunned, do nothing
        {
            Stunned = false;
            return;
        }

        // condition to check if unit needs to move
        if (Role == 0) 
        {
            if (CurrentStackZone != GameController.Instance.LeftStackZone &&
                CurrentStackZone != GameController.Instance.RightStackZone &&
                CurrentStackZone != GameController.Instance.BackStackZone )
            {
                MoveToNewStackZone(GameController.Instance.StackZones[Random.Range(1, 4)]);
                return;
            }
        }
        else if (Role == 1)
        {
            if (CurrentStackZone != GameController.Instance.FrontStackZone)
            {
                MoveToNewStackZone(GameController.Instance.FrontStackZone);
                return;
            }
        }
        else if (Role == 2)
        {
            if (CurrentStackZone != GameController.Instance.OuterStackZone)
            {
                MoveToNewStackZone(GameController.Instance.OuterStackZone);
                return;
            }
        }


        if (Role == 0)
        {
            BossMechanics.Instance.BossTakeDamage(DDamageDealt);
        } 
        else if (Role == 1)
        {
            BossMechanics.Instance.BossTakeDamage(TankDamageDealt);
            // Add tank bool;
        }
        else if (Role == 2)
        {
            List<Raider> woundedRaiders = new List<Raider>();
            Raider tank = null;
            foreach (Raider raider in GameController.Instance.AllRaiders)
            {
                if (raider.HitPoints < raider.MaxHitPoints)
                {
                    woundedRaiders.Add(raider);
                    if (raider.Role == 1) tank = raider;
                }
            }

            if (tank != null)
            {
                tank.RestoreHealth(HealerHealthRestore);
            }
            else if (woundedRaiders.Count > 0)
            {
                woundedRaiders[Random.Range(0, woundedRaiders.Count)].RestoreHealth(HealerHealthRestore);
                //List<Order> SortedList = objListOrder.OrderBy(o => o.OrderDate).ToList();
            }
        }
    }

    public void AddCostToZone(StackZone stackZone, int cost)
    {
        if (ZoneCosts.ContainsKey(stackZone))
        {
            ZoneCosts[stackZone] += cost;
        }
        else
        {
            ZoneCosts.Add(stackZone, cost);
        }
    }

    public StackZone LowestCostStackZone()
    {
        StackZone preferedStackZone = CurrentStackZone;

        int lowestCost = 0;
        if (ZoneCosts.ContainsKey(CurrentStackZone))
        {
            lowestCost = ZoneCosts[CurrentStackZone];
        }

        foreach (KeyValuePair<StackZone, int> entry in ZoneCosts)
        {
            if (ZoneCosts[entry.Key] < lowestCost)
            {
                lowestCost = ZoneCosts[entry.Key];
                preferedStackZone = entry.Key;
            }

        }

        return preferedStackZone;
    }



    public void MoveToNewStackZone(StackZone stackZone)
    {
        GameController.Instance.RemoveRaiderFromAllStacks(this.gameObject);
        CurrentStackZone = stackZone;
        CurrentStackZone.UnitsInZone.Add(this.gameObject);

        tarDestination = CurrentStackZone.RandomPointInZone();
        moving = true;
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            Vector2 currPos = transform.position;
            Vector2 moveDirection = (tarDestination - currPos).normalized;

            transform.position = currPos + moveDirection * speed * Time.deltaTime;

            if (Vector2.Distance(transform.position, tarDestination) <= 0.1)
            {
                moving = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        HitPoints -= damage;

        float ratio = ((float)HitPoints / (float)MaxHitPoints);
        GetComponent<SpriteRenderer>().color = new Color(0.5f + ratio/2, ratio, ratio, 1f);

        if (HitPoints <= 0)
        {
            GameController.Instance.KilledRaiders.Add(this.gameObject);
        }
    }

    public void RestoreHealth(int amount)
    {
        HitPoints += amount;
        if (HitPoints > MaxHitPoints) HitPoints = MaxHitPoints;

        float ratio = ((float)HitPoints / (float)MaxHitPoints);
        GetComponent<SpriteRenderer>().color = new Color(0.5f + ratio / 2, ratio, ratio, 1f);

    }

}
