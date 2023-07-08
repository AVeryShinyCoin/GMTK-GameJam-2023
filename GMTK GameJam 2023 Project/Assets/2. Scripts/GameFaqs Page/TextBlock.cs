using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextBlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool Interactable;
    public bool LineBreak;
    public bool NewCondition;
    public string DisplayText;
    public float Width;
    public int Index;
    public int InstructionBelongsToRole;
    [SerializeField] TextMeshProUGUI textUI;
    [SerializeField] float spaceBarWidth;

    public DataType dataType = new DataType();
    public enum DataType { None, Zones, Target, BossEnergy, BossHealth, DirectionFromTarget, Action};
    public int DataValue;
    public List<StackZone> DataZones;

    bool hoverOver;
    float fontSize;

    void Awake()
    {
        fontSize = textUI.fontSize;
        dataType = DataType.None;
        InstructionBelongsToRole = TextDisplay.Instance.CurrentRoleInstructions;
    }

    public void InitializeBlock(string text)
    {
        DisplayText = text;

        if (text == "LINEBREAK")
        {
            LineBreak = true;
            Width = 0;
            return;
        }

        if (text == "-")
        {
            NewCondition = true;
        }

        if (text.StartsWith("H@"))
        {
            DisplayText = DisplayText.Substring(2);
            textUI.fontSize = textUI.fontSize * 1.2f;
            TextDisplay.Instance.CurrentRoleInstructions++;
        }

        if (text.Contains("damagedealer"))
        {
            textUI.color = new Color(0.4f, 0.1f, 0.1f, 1f);
        }
        if (text.Contains("tank"))
        {
            textUI.color = new Color(0.1f, 0.1f, 0.4f, 1f);
        }
        if (text.Contains("healer"))
        {
            textUI.color = new Color(0.1f, 0.4f, 0.1f, 1f);
        }


        if (text.StartsWith("ZON@"))
        {
            if (text.Contains("@@")) MakeInteractable();
            dataType = DataType.Zones;

            if (text.Contains("near"))
            {
                DataZones.Add(GameController.Instance.FrontStackZone);
                DataZones.Add(GameController.Instance.BackStackZone);
                DataZones.Add(GameController.Instance.LeftStackZone);
                DataZones.Add(GameController.Instance.RightStackZone);
                DisplayText = "near";
            }

            if (text.Contains("sides"))
            {
                DataZones.Add(GameController.Instance.LeftStackZone);
                DataZones.Add(GameController.Instance.RightStackZone);
                DisplayText = "to the sides of";
            }

            if (text.Contains("away"))
            {
                DataZones.Add(GameController.Instance.OuterStackZone);
                DisplayText = "away from";
            }

            if (text.Contains("left"))
            {
                DataZones.Add(GameController.Instance.LeftStackZone);
                DisplayText = "to the left of";
            }

            if (text.Contains("right"))
            {
                DataZones.Add(GameController.Instance.LeftStackZone);
                DisplayText = "to the right of";
            }

            if (text.Contains("notfront"))
            {
                DataZones.Add(GameController.Instance.BackStackZone);
                DataZones.Add(GameController.Instance.LeftStackZone);
                DataZones.Add(GameController.Instance.RightStackZone);
                DisplayText = "away from the front of";
            } else if (text.Contains("front"))
            {
                DataZones.Add(GameController.Instance.FrontStackZone);
                DisplayText = "in front of";
            }

            if (text.Contains("behind"))
            {
                DataZones.Add(GameController.Instance.BackStackZone);
                DisplayText = "behind";
            }
        }

        if (text.StartsWith("TAR@"))
        {
            if (text.Contains("@@")) MakeInteractable();
            dataType = DataType.Target;

            if (text.Contains("dd"))
            {
                DataValue = 0;
                DisplayText = "damagedealers";
            }

            if (text.Contains("tank"))
            {
                DataValue = 1;
                DisplayText = "tanks";
            }

            if (text.Contains("heal"))
            {
                DataValue = 2;
                DisplayText = "healers";
            }

            if (text.Contains("fire"))
            {
                DataValue = 3;
                DisplayText = "fire";
            }
            if (text.Contains("boss"))
            {
                DataValue = 4;
                DisplayText = "boss";
            }
        }

        if (text.StartsWith("NRG@"))
        {
            int parseOffset = 0;
            if (text.Contains("@@"))
            {
                MakeInteractable();
                parseOffset = 1;
            }

            if (text.Length == 7 + parseOffset)
            {
                DataValue = 100;
            }
            else
            {
                DataValue = int.Parse(text.Substring(4 + parseOffset, 2));
            }

            dataType = DataType.BossEnergy;
            DisplayText = DataValue + "% boss energy";
        }

        if (text.StartsWith("HPS@"))
        {
            int parseOffset = 0;
            if (text.Contains("@@"))
            {
                MakeInteractable();
                parseOffset = 1;
            }
                
            if (text.Length == 7 + parseOffset)
            {
                DataValue = 100;
            }
            else
            {
                DataValue = int.Parse(text.Substring(4 + parseOffset, 2));
            }

            dataType = DataType.BossHealth;
            DisplayText = DataValue + "% boss health";
        }

        if (text.StartsWith("DFT@"))
        {
            if (text.Contains("@@")) MakeInteractable();
            dataType = DataType.DirectionFromTarget;
            if (text.Contains("stack"))
            {
                DataValue = 0;
                DisplayText = "stack on";
            }
            if (text.Contains("away"))
            {
                DataValue = 1;
                DisplayText = "move away from";
            }
        }

        if (text.StartsWith("ACT@"))
        {
            if (text.Contains("@@")) MakeInteractable();
            dataType = DataType.Action;
            if (text.Contains("heal"))
            {
                DataValue = 0;
                DisplayText = "heal";
            }
        }

        textUI.text = DisplayText;
        textUI.gameObject.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
        Width = textUI.gameObject.GetComponent<RectTransform>().sizeDelta.x + spaceBarWidth;
        InstructionBelongsToRole = TextDisplay.Instance.CurrentRoleInstructions;

        if (Interactable)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(Width, GetComponent<RectTransform>().sizeDelta.y);
        } 
    }

    void MakeInteractable()
    {
        Interactable = true;
        textUI.fontStyle = FontStyles.Underline;
        textUI.color = new Color(0.6f, 0.0f, 0.6f, 1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactable) return;
        hoverOver = true;
        textUI.fontSize = fontSize * 1.10f;
        SoundManager.Instance.PlayUniqueSound("ButtonHover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Interactable) return;
        hoverOver = false;
        textUI.fontSize = fontSize * 1f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Interactable) return;
        textUI.fontSize = fontSize * 1f;
        SoundManager.Instance.PlayUniqueSound("ButtonClick");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!Interactable) return;
        if (hoverOver)
        {
            textUI.fontSize = fontSize * 1.10f;
        }
        else
        {
            textUI.fontSize = fontSize * 1f;
        }
    }


}
