using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditGuideButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    Color defColor;
    Color highlightColor;
    TextMeshProUGUI textUI;
    bool hoverOver;
    float fontSize;

    void Awake()
    {
        defColor = GetComponent<Image>().color;
        highlightColor = new Color(0.9f, 0.9f, 0.75f);
        textUI = GetComponentInChildren<TextMeshProUGUI>();
        fontSize = textUI.fontSize;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverOver = true;
        textUI.fontSize = fontSize * 1.05f;
        GetComponent<Image>().color = highlightColor;
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
            GetComponent<Image>().color = highlightColor;

            CameraManager.Instance.PanToEditScreen();
        }
        else
        {
            textUI.fontSize = fontSize * 1f;
            GetComponent<Image>().color = defColor;
        }
    }
}
