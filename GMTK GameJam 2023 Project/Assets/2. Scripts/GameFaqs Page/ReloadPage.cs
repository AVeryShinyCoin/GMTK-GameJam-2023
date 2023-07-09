using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReloadPage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    Color defColor;
    TextMeshProUGUI textUI;
    bool hoverOver;
    float fontSize;

    void Awake()
    {
        defColor = GetComponent<Image>().color;
        textUI = GetComponentInChildren<TextMeshProUGUI>();
        fontSize = textUI.fontSize;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverOver = true;
        textUI.fontSize = fontSize * 1.05f;
        GetComponent<Image>().color = new Color(0.9f, 0.75f, 0.75f, 1f);
        SoundManager.Instance.PlayUniqueSound("ButtonHover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverOver = false;
        textUI.fontSize = fontSize * 1f;
        GetComponent<Image>().color = defColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        textUI.fontSize = fontSize * 1f;
        GetComponent<Image>().color = defColor;
        SoundManager.Instance.PlayUniqueSound("ButtonClick");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (hoverOver)
        {
            textUI.fontSize = fontSize * 1.05f;
            GetComponent<Image>().color = new Color(0.9f, 0.75f, 0.75f, 1f);

            TextDisplay.Instance.ResetSwaps();
        }
        else
        {
            textUI.fontSize = fontSize * 1f;
            GetComponent<Image>().color = defColor;
        }
    }
}
