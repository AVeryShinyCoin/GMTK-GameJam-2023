using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    public StackZone LowestCostStackZone;
    public bool Stunned;

    [SerializeField] float speed;
    [SerializeField] RaiderGFX gfx;
    public bool moving;
    public Vector2 tarDestination;
    [SerializeField] GameObject fxHealPrefab;
    public bool Ready;

    [Space(20)]
    public Dictionary<StackZone, int> ZoneCosts = new Dictionary<StackZone, int>();

    private void Start()
    {
        if (Role == 0)
        {
            MaxHitPoints = TankHitPoints;
        }
        else if (Role == 1)
        {
            MaxHitPoints = DDHitPoints;
        }
        else if (Role == 2)
        {
            MaxHitPoints = HealerHitPoints;
        }

        HitPoints = MaxHitPoints;
    }

    public void PerformAction()
    {
        if (Stunned) // if raider is stunned, do nothing
        {
            Stunned = false;
            return;
        }

        // condition to check if unit needs to move
        UpdateLowestCostStackZone();

        if (LowestCostStackZone != CurrentStackZone)
        {
            MoveToNewStackZone(LowestCostStackZone);
            return;
        }


        
        Ready = true;

        if (Role == 0)
        {
            if (CurrentStackZone == GameController.Instance.FrontStackZone ||
                CurrentStackZone == GameController.Instance.BackStackZone ||
                CurrentStackZone == GameController.Instance.LeftStackZone ||
                CurrentStackZone == GameController.Instance.RightStackZone)
            {
                gfx.UseAbility();
                BossMechanics.Instance.BossTakeDamage(TankDamageDealt);
                PlayAbilitySound("HitMedium");
            }
            // Add tank bool;
        }
        else if (Role == 1)
        {
            if (CurrentStackZone == GameController.Instance.FrontStackZone ||
                CurrentStackZone == GameController.Instance.BackStackZone ||
                CurrentStackZone == GameController.Instance.LeftStackZone ||
                CurrentStackZone == GameController.Instance.RightStackZone)
            {
                gfx.UseAbility();
                BossMechanics.Instance.BossTakeDamage(DDamageDealt);
                PlayAbilitySound("HitLight");
            }
            
        }
        else if (Role == 2)
        {
            gfx.UseAbility();
            PlayAbilitySound("Heal");

            List<Raider> woundedRaiders = new List<Raider>();
            List<Raider> woundedTanks = new List<Raider>();
            Raider tank = null;
            foreach (Raider raider in GameController.Instance.AllRaiders)
            {
                if (raider.HitPoints < raider.MaxHitPoints)
                {
                    woundedRaiders.Add(raider);
                    if (raider.Role == 1) woundedTanks.Add(raider);
                }
            }

            if (woundedTanks.Count > 0)
            {
                Raider mostDamTank = null;
                int mostDamTankValue = 0;
                foreach (Raider raider in woundedTanks)
                {
                    int hurt = raider.MaxHitPoints - raider.HitPoints;
                    if (hurt > mostDamTankValue)
                    {
                        mostDamTankValue = hurt;
                        mostDamTank = raider;
                    }
                }

                tank = mostDamTank;
            }

            if (tank != null)
            {
                tank.RestoreHealth(HealerHealthRestore);
            }
            else if (woundedRaiders.Count > 0)
            {
                Raider mostDamaged = null;
                int mostDamage = 0;
                foreach (Raider raider in woundedRaiders)
                {
                    int hurt = raider.MaxHitPoints - raider.HitPoints;
                    if (hurt > mostDamage)
                    {
                        mostDamage = hurt;
                        mostDamaged = raider;
                    }
                }
                mostDamaged.RestoreHealth(HealerHealthRestore);
                
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

    public void UpdateLowestCostStackZone()
    {
        StackZone preferedStackZone = null;
        int lowestCost = 100000;
        if (CurrentStackZone != null)
        {
            preferedStackZone = CurrentStackZone;
            if (ZoneCosts.ContainsKey(CurrentStackZone))
            {
                lowestCost = ZoneCosts[CurrentStackZone];
            }

        }

        List<StackZone> lowestCosts = new List<StackZone>();

        foreach (KeyValuePair<StackZone, int> entry in ZoneCosts)
        {

            if (ZoneCosts[entry.Key] == lowestCost)
            {
                lowestCosts.Add(entry.Key);
            }
            if (ZoneCosts[entry.Key] < lowestCost)
            {
                lowestCost = ZoneCosts[entry.Key];
                lowestCosts.Clear();
                lowestCosts.Add(entry.Key);
            }
        }

        if (!lowestCosts.Contains(CurrentStackZone))
        {
            LowestCostStackZone = lowestCosts[Random.Range(0, lowestCosts.Count)];
        }
        else
        {
            LowestCostStackZone = CurrentStackZone;
        }
        
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
                Ready = true;
                gfx.ArrivedAtDestination();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (Role == 0)
        {
            damage /= 2;
        }

        HitPoints -= damage;
        if (HitPoints <= 0)
        {
            GameController.Instance.KilledRaiders.Add(this.gameObject);
            gfx.Die();
            if (moving) moving = false;
            GameController.Instance.RaidWiped();
        }
    }

    public void RestoreHealth(int amount)
    {
        Instantiate(fxHealPrefab, transform.position, Quaternion.identity);
        HitPoints += amount;
        if (HitPoints > MaxHitPoints) HitPoints = MaxHitPoints;
    }

    void PlayAbilitySound(string sound)
    {
        SoundManager.Instance.PlayUniqueSound(sound, 1f, 0.8f, 1.1f);
    }

}
