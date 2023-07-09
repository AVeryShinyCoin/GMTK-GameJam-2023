using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class EnergyCondition
{
    int Role;
    public List<StackZone> AffectedZones = new List<StackZone>();
    public int CostChange;
    public int[] EnergyTargets;

    public EnergyCondition(int role, List<StackZone> affectedZones, int costChange, int[] energyTargets)
    {
        Role = role;
        AffectedZones = affectedZones;
        CostChange = -costChange;
        EnergyTargets = energyTargets;
    }

    public void ApplyConditionCosts(Raider raider)
    {
        for (int i = 0; i < EnergyTargets.Length; i++)
        {
            if (BossMechanics.Instance.BossEnergy != EnergyTargets[i]) continue;

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
