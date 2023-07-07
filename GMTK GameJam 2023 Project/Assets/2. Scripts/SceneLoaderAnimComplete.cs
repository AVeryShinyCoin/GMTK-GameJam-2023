using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderAnimComplete : MonoBehaviour
{
    [SerializeField] SceneLoader script;
    public void AnimationComplete()
    {
        script.AnimationComplete();
    }
}
