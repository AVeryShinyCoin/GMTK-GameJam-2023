using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsParser : MonoBehaviour
{
    List<List<TextBlock>> rawConditions = new List<List<TextBlock>>();


    public void ParseInstructions(List<TextBlock> rawTextBlocks)
    {
        rawConditions.Clear();

        List<TextBlock> currConditionList = new List<TextBlock>();      

        foreach (TextBlock textBlock in rawTextBlocks)        
        {
            if (textBlock.NewCondition == true && currConditionList.Count != 0)
            {
                List<TextBlock> newList = new List<TextBlock>();
                foreach (TextBlock _textBlock in currConditionList)
                {
                    newList.Add(_textBlock);
                }
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
            int negMod = (ContainsNegativeModifier(list));

            if (type == 0)          // Basic Condition
            {
                List<StackZone> stackZones = FindStackZones(list);
                if (stackZones.Count == 0)
                {
                    Debug.LogError("EMPTY STACKZONES SENT TO BASIC CONDITION!");
                    continue;
                }

                int cost;
                if (role == 0)
                {
                    cost = 2000;
                }
                else
                {
                    cost = 20;
                }

                BasicCondition newCondition = new BasicCondition(role, stackZones, cost * negMod);
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

                int[] values = FindPercentageValues(list);
                if (values.Length == 0)
                {
                    Debug.LogError("EMPTY VALUES SENT TO ENERGY CONDITION!");
                    continue;
                }

                EnergyCondition newCondition = new EnergyCondition(role, stackZones, 1000 * negMod, values);
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

                int[] values = FindPercentageValues(list);
                if (values.Length == 0)
                {
                    Debug.LogError("EMPTY VALUES SENT TO ENERGY CONDITION!");
                    continue;
                }

                HealthCondition newCondition = new HealthCondition(role, stackZones, 1000 * negMod, values);
                GameController.Instance.HealthConditions.Add(newCondition);
            }
        }
    }

    bool CheckIfSkip(List<TextBlock> list)
    {
        foreach (TextBlock textBlock in list)
        {
            if (textBlock.dataType == TextBlock.DataType.SkipCondition)
            {
                return true;
            }
        }
        return false;
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


    int[] FindPercentageValues(List<TextBlock> list)
    {
        List<int> valuesList = new List<int>();
        foreach (TextBlock textBlock in list)
        {
            if (textBlock.dataType == TextBlock.DataType.Percent)
            {
                valuesList.Add(textBlock.DataValue);
            }
        }

        return (valuesList.ToArray());
    }

    int ContainsNegativeModifier(List<TextBlock> list)
    {
        foreach (TextBlock textBlock in list)
        {
            if (textBlock.dataType == TextBlock.DataType.NegativeModifier)
            {
                return -1;
            }
        }
        return 1;
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
