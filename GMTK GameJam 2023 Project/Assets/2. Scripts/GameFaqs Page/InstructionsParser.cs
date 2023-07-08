using System.Collections.Generic;
using UnityEngine;

public class InstructionsParser : MonoBehaviour
{
    List<List<TextBlock>> rawConditions = new List<List<TextBlock>>();


    public void ParseInstructions(List<TextBlock> rawTextBlocks)
    {
        List<TextBlock> currConditionList = new List<TextBlock>();      

        foreach (TextBlock textBlock in rawTextBlocks)        
        {
            Debug.Log("SUCCESS!" + textBlock.DisplayText);
            if (textBlock.NewCondition == true && currConditionList.Count != 0)
            {
                List<TextBlock> newList = new List<TextBlock>();
                foreach (TextBlock _textBlock in currConditionList)
                {
                    newList.Add(_textBlock);
                }
                Debug.Log(newList.Count);
                rawConditions.Add(newList);
                currConditionList.Clear();
            }

            if (textBlock.dataType == TextBlock.DataType.None) continue; 
            currConditionList.Add(textBlock);
        }

        //add final condition list since it won't have a new condition starter
        List<TextBlock> _newList = new List<TextBlock>();
        foreach (TextBlock _textBlock in currConditionList)
        {
            _newList.Add(_textBlock);
        }
        Debug.Log(_newList.Count);
        rawConditions.Add(_newList);
        currConditionList.Clear();


        CookConditions();
    }

    private void CookConditions()
    {
        foreach (List<TextBlock> list in rawConditions)
        {
            if (list.Count == 0)
            {
                Debug.LogError("EMPTY CONDITION SENT TO CONDITION COOKER!");
                continue;
            }

            int role = list[0].InstructionBelongsToRole;
            int type = FindTypeOfCondition(list);

            if (type == 0)          // Basic Condition
            {
                List<StackZone> stackZones = FindStackZones(list);
                if (stackZones.Count == 0)
                {
                    Debug.LogError("EMPTY STACKZONES SENT TO BASIC CONDITION!");
                    continue;
                }
                BasicCondition newCondition = new BasicCondition(role, stackZones, -400);
                GameController.Instance.BasicConditions.Add(newCondition);
            }

            else if (type == 1)     // Energy Condition
            {
                List<StackZone> stackZones = FindStackZones(list);
                if (stackZones.Count == 0)
                {
                    Debug.LogError("EMPTY STACKZONES SENT TO ENERGY CONDITION!");
                    continue;
                }

                int value = FindDataValueOfType(list, TextBlock.DataType.BossEnergy);
                if (value == -1)
                {
                    Debug.LogError("EMPTY VALUE SENT TO ENERGY CONDITION!");
                    continue;
                }

                EnergyCondition newCondition = new EnergyCondition(role, stackZones, 1000, value);
                GameController.Instance.EnergyConditions.Add(newCondition);
            }

            else if (type == 2)     // Health Condition
            {
                List<StackZone> stackZones = FindStackZones(list);
                if (stackZones.Count == 0)
                {
                    Debug.LogError("EMPTY STACKZONES SENT TO ENERGY CONDITION!");
                    continue;
                }

                int value = FindDataValueOfType(list, TextBlock.DataType.BossHealth);
                if (value == -1)
                {
                    Debug.LogError("EMPTY VALUE SENT TO ENERGY CONDITION!");
                    continue;
                }

                HealthCondition newCondition = new HealthCondition(role, stackZones, 1000, value);
                GameController.Instance.HealthConditions.Add(newCondition);
            }
        }
    }

    int FindTypeOfCondition(List<TextBlock> list)
    {
        int type = 0;
        foreach (TextBlock textBlock in list)
        {
            if (textBlock.dataType == TextBlock.DataType.BossEnergy) type = 1;
            if (textBlock.dataType == TextBlock.DataType.BossHealth) type = 2;
        }
        return type;
    }

    List<StackZone> FindStackZones(List<TextBlock> list)
    {
        List<StackZone> stackZones = new List<StackZone>();

        foreach (TextBlock textBlock in list)
        {
            if (textBlock.dataType == TextBlock.DataType.Zones)
            {
                stackZones = textBlock.DataZones;
            }
        }
        return stackZones;
    }

    int FindDataValueOfType(List<TextBlock> list, TextBlock.DataType type)
    {
        int value = -1;
        foreach (TextBlock textBlock in list)
        {
            if (textBlock.dataType == type)
            {
                value = textBlock.DataValue;
            }
        }
        return value;
    }
}
