using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //update energy gfx
    }


    public void BossTakeDamage(int damage)
    {
        BossHP -= damage;

        float ratio = ((float)BossHP / (float)BossHPMax);
        GetComponent<SpriteRenderer>().color = new Color(0.5f + ratio / 2, 0.2f * ratio, 0.2f * ratio, 1f);

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
        GameController.Instance.CullDeadRaidersFromAllStacks();
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
