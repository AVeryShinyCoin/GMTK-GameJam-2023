using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class BasicCondition
{
    int Role;
    public List<StackZone> PreferedZones = new List<StackZone>();
    public int CostChange;

    public BasicCondition(int role, List<StackZone> preferedZones, int costChange)
    {
        Role = role;
        PreferedZones = preferedZones;
        CostChange = costChange;
    }

    public void ApplyConditionCosts(Raider raider)
    {
        if (raider.Role != Role) return;

        foreach (StackZone stackZone in PreferedZones)
        {
            raider.AddCostToZone(stackZone, CostChange);
        }
    }
}
