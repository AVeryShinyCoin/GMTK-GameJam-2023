using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class HealthCondition
{
    int Role;
    public List<StackZone> AffectedZones = new List<StackZone>();
    public int CostChange;
    public int HealthTarget;

    public HealthCondition(int role, List<StackZone> affectedZones, int costChange, int healthTarget)
    {
        Role = role;
        AffectedZones = affectedZones;
        CostChange = costChange;
        HealthTarget = healthTarget;
    }

    public void ApplyConditionCosts(Raider raider)
    {
        if (raider.Role != Role) return;
        if (BossMechanics.Instance.BossHP > HealthTarget) return;

        foreach (StackZone stackZone in AffectedZones)
        {
            raider.AddCostToZone(stackZone, CostChange);
        }
    }
}
