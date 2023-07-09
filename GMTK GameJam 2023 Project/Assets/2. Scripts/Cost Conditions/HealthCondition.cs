using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class HealthCondition
{
    int Role;
    public List<StackZone> AffectedZones = new List<StackZone>();
    public int CostChange;
    public int[] HealthTargets;

    public HealthCondition(int role, List<StackZone> affectedZones, int costChange, int[] healthTargets)
    {
        Role = role;
        AffectedZones = affectedZones;
        CostChange = -costChange;
        HealthTargets = healthTargets;
    }

    public void ApplyConditionCosts(Raider raider)
    {
        for (int i = 0; i < HealthTargets.Length; i++)
        {
            if (((float)((float)BossMechanics.Instance.BossHP / (float)BossMechanics.Instance.BossHPMax) * 100) > HealthTargets[i]) return;

            // role -1 applies to everyone, but only at half effect
            if (Role == -1)
            {
                foreach (StackZone stackZone in AffectedZones)
                {
                    raider.AddCostToZone(stackZone, CostChange / 2);
                }
                return;
            }

            if (raider.Role != Role) return;
            foreach (StackZone stackZone in AffectedZones)
            {
                raider.AddCostToZone(stackZone, CostChange);
            }
        }
    }
}
