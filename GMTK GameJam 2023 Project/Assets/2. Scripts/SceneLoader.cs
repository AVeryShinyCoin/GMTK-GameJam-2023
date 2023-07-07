using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Animator animator;
    public static SceneLoader Instance;
    int targetScene;
    public bool StartedTransition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            MenuManager.Instance.titleMenu.SetActive(true);
            SoundManager.Instance.PlaySound("BGMMusic", 1f);
        }
    }

    public void LoadScene(int scene)
    {
        if (!StartedTransition)
        {
            targetScene = scene;
            StartedTransition = true;
            animator.Play("Transition Start");
        } 
    }
    public void AnimationComplete()
    {
        SceneManager.LoadScene(targetScene);
    }
}
