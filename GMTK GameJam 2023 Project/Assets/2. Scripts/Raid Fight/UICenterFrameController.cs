using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICenterFrameController : MonoBehaviour
{
    [SerializeField] GameObject dragonUIdefeat;
    [SerializeField] GameObject dragonUIvictory;


    // Start is called before the first frame update
    void OnDisable()
    {
        dragonUIdefeat.SetActive(false);
        dragonUIvictory.SetActive(false);
    }

    public void ShowDefeatScreen()
    {
        dragonUIdefeat.SetActive(true);
    }
    public void ShowVictoryScreen()
    {
        dragonUIvictory.SetActive(true);
    }
}
