using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class EnergyCondition
{
    int Role;
    public List<StackZone> AffectedZones = new List<StackZone>();
    public int CostChange;
    public int EnergyTarget;

    public EnergyCondition(int role, List<StackZone> affectedZones, int costChange, int energyTarget)
    {
        Role = role;
        AffectedZones = affectedZones;
        CostChange = costChange;
        EnergyTarget = energyTarget;
    }

    public void ApplyConditionCosts(Raider raider)
    {

        if (raider.Role != Role) return;
        if (BossMechanics.Instance.BossEnergy != EnergyTarget) return;

        foreach (StackZone stackZone in AffectedZones)
        {
            raider.AddCostToZone(stackZone, CostChange);
        }
    }
}
