using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BossMechanics : MonoBehaviour
{
    public static BossMechanics Instance;

    public StackZone FrontStackZone;
    public StackZone BackStackZone;
    public StackZone LeftStackZone;
    public StackZone RightStackZone;
    public StackZone OuterStackZone;

    [Space(20)]
    public int BossHP;
    public int BossHPMax;
    public int BossEnergy;
    public int BossMaxEnergy;

    [Space(20)]
    [SerializeField] BossGFX gfx;
    [SerializeField] LargeEffect fxBackSwipe;
    [SerializeField] LargeEffect fxFrontSwipe;
    [SerializeField] LargeEffect fxFrontSwipe2;
    [SerializeField] LargeEffect fxExplosion;
    [SerializeField] LargeEffect fxFirebreath;
    [SerializeField] LargeEffect fxBreathExplode;
    [SerializeField] GameObject fxBitePrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        BossHP = BossHPMax;
        BossEnergy = 0;
    }

    public void ChangeBossEnergy(int amount)
    {
        BossEnergy += amount;
        if (BossEnergy > 100)
        {
            BossEnergy = 0;
        }
        //update energy gfx
    }


    public void PerformBossAction()
    {
        if (BossHP <= 0) return;

        if (BossEnergy == 100)
        {
            FearAll();
            Invoke("TailSwipe", 1.5f);
        }
        else
        {
            FrontAttackBite();
            Invoke("TailSwipe", 0.75f);
        }

        if (BossEnergy == 30 || BossEnergy == 60)
        {
            Invoke("WideClawAttack", 1.5f);
        }

        if (BossEnergy == 50)
        {
            fxFirebreath.PlayAnimation();
            fxBreathExplode.PlayAnimation();
            Invoke("FireBreath", 1.5f);
        }

        GameController.Instance.CullDeadRaidersFromAllStacks();

        Invoke("ActionDone", 0f);
    }


    private void ActionDone()
    {
        Debug.Log("Boss action complete!");
    }


    private void FrontAttackBite()
    {
        if (FrontStackZone.UnitsInZone.Count > 0)
        {
            Raider tank = null;
            Raider target;
            foreach (GameObject unit in FrontStackZone.UnitsInZone)
            {
                if (unit.GetComponent<Raider>().Role == 0)
                {
                    tank = unit.GetComponent<Raider>();
                    break;
                }
            }
            if (tank != null)
            {
                target = tank;
            } else
            {
                int rnd = Random.Range(0, FrontStackZone.UnitsInZone.Count);
                target = FrontStackZone.UnitsInZone[rnd].GetComponent<Raider>();
            }

            target.TakeDamage(10);
            Instantiate(fxBitePrefab, new Vector2(target.transform.position.x +0.05f, target.transform.position.y + 0.2f), Quaternion.identity);
            //inset sound & animation
            
        }
        else
        {
            foreach (Raider raider in GameController.Instance.AllRaiders)
            {
                raider.TakeDamage(8);
            }
            //inset sound & animation
            fxExplosion.PlayAnimation();
        }
    }

    private void TailSwipe()
    {

        if (BackStackZone.UnitsInZone.Count > 0)
        {
            List<StackZone> list = new List<StackZone>();
            list.Add(BackStackZone);
            DamageAllInZones(list, 8);

            //inset sound & animation
            fxBackSwipe.PlayAnimation();
        }
    }

    private void WideClawAttack()
    {
        List<StackZone> list = new List<StackZone>();
        list.Add(FrontStackZone);
        list.Add(LeftStackZone);
        list.Add(BackStackZone);
        DamageAllInZones(list, 8);

        //inset sound & animation
        fxFrontSwipe.PlayAnimation();
        fxFrontSwipe2.PlayAnimation();
    }

    private void FireBreath()
    {
        if (FrontStackZone.UnitsInZone.Count > 0)
        {
            float units = FrontStackZone.UnitsInZone.Count;
            int damage = Mathf.FloorToInt((40 / units) + 0.5f);
            List<StackZone> list = new List<StackZone>();
            list.Add(FrontStackZone);
            DamageAllInZones(list, damage);
        }

        //inset sound & animation
        
    }

    private void FearAll()
    {
        foreach (Raider raider in GameController.Instance.AllRaiders)
        {
            raider.MoveToNewStackZone(GameController.Instance.StackZones[Random.Range(0, GameController.Instance.StackZones.Count)]);
        }

        //inset sound and animation
        fxExplosion.PlayAnimation();
    }


    public void BossTakeDamage(int damage)
    {
        BossHP -= damage;

        float ratio = ((float)BossHP / (float)BossHPMax);
        //GetComponent<SpriteRenderer>().color = new Color(0.5f + ratio / 2, 0.2f * ratio, 0.2f * ratio, 1f);

        if (BossHP <= 0)
        {
            Debug.Log("THE BOSS HAS BEEN SLAIN");
        }
    }

    public void DamageAllInZones(List<StackZone> stackZones, int damage)
    {
        foreach (StackZone stackZone in stackZones)
        {
            foreach (GameObject raider in stackZone.UnitsInZone)
            {
                raider.GetComponent<Raider>().TakeDamage(damage);
            }
        }
    }

    public void StunAllInZones(List<StackZone> stackZones)
    {
        foreach (StackZone stackZone in stackZones)
        {
            foreach (GameObject raider in stackZone.UnitsInZone)
            {
                raider.GetComponent<Raider>().Stunned = true;
            }
        }
    }

    public void FearAllInZones(List<StackZone> stackZones)
    {
        foreach (StackZone stackZone in stackZones)
        {
            
        }
    }

    public void PutFireInZones(List<StackZone> stackZones)
    {
        foreach (StackZone stackZone in stackZones)
        {
            
        }
    }
}
