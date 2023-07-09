using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    RectTransform trans;
    float width;
    float height;
    [SerializeField] TextMeshProUGUI text;


    // Start is called before the first frame update
    void Awake()
    {
        trans = GetComponent<RectTransform>();
        width = trans.sizeDelta.x;
        height = trans.sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        float ratio = (float)((float)BossMechanics.Instance.BossHP / (float)BossMechanics.Instance.BossHPMax);
        text.text = (Mathf.FloorToInt((ratio * 100) + 0.5f) + "%");
        trans.sizeDelta = new Vector2(width * ratio, height);
    }
}
