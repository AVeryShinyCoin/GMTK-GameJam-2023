using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Action action;
    bool hoverOver;
    Image image;

    TextMeshProUGUI text;
    float textSize;



    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        textSize = text.fontSize;
    }

    private void OnEnable()
    {
        hoverOver = false;
        text.color = new Color(0f, 0f, 0f, 1f);
        text.fontSize = textSize * 1.0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverOver = true;
        text.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        text.fontSize = textSize * 1.2f;
        SoundManager.Instance.PlayUniqueSound("ButtonHover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverOver = false;
        text.color = new Color(0f, 0f, 0f, 1f);
        text.fontSize = textSize * 1.0f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        text.color = new Color(0f, 0f, 0f, 1f);
        text.fontSize = textSize * 1.0f;
        SoundManager.Instance.PlayUniqueSound("ButtonClick");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (hoverOver)
        {
            action();
            text.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            text.fontSize = textSize * 1.2f;
        }
        else
        {
            text.color = new Color(0f, 0f, 0f, 1f);
            text.fontSize = textSize * 1.0f;
        }
    }
}
