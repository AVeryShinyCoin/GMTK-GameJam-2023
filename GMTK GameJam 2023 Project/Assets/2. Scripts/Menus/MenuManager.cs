using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    private InputActions input = null;

    public GameObject titleMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject viewControls;
    [SerializeField] GameObject credits;

    [Space(20)]
    [SerializeField] MenuButton titlePlay;
    [SerializeField] MenuButton titleSettings;
    [SerializeField] MenuButton titleQuit;
    [SerializeField] MenuButton titleCredits;
    [SerializeField] MenuButton settingsViewControls;
    [SerializeField] MenuButton settingsFullscreen;
    [SerializeField] MenuButton settingsBack;
    [SerializeField] MenuButton pausedResume;
    [SerializeField] MenuButton pausedSettings;
    [SerializeField] MenuButton pausedQuit;
    [SerializeField] MenuButton screenControlsBack;
    [SerializeField] MenuButton screenCreditsBack;

    [Space(20)]
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    private float musicVolume = 1;
    private float sfxVolume = 1;

    [HideInInspector] public bool IntroTextScrollDone;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        input = new InputActions();
    }

    void SetMusicVolume(float value)
    {
        musicVolume = value;
        Debug.Log(value);
        mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }
    void SetSFXVolume(float value)
    {
        sfxVolume = value;
        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }
    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    void Start()
    {
        //titlePlay.action = () => TitleStartGame();
        //titleSettings.action = () => TitleEnterSettings();
        //titleQuit.action = () => TitleQuit();
        //titleCredits.action = () => TitleCredits();
        //settingsViewControls.action = () => SettingsViewControls();
        settingsFullscreen.action = () => SettingsFullscreen();
        settingsBack.action = () => SettingsBack();
        pausedResume.action = () => PausedResume();
        pausedSettings.action = () => PausedSettings();
        pausedQuit.action = () => PausedQuit();
        //screenControlsBack.action = () => ScreenControlsBack();
        //screenCreditsBack.action = () => ScreenCreditsBack();
    }

    public void TitleStartGame()
    {
        Debug.Log("Succes");
        titleMenu.SetActive(false);
        SceneLoader.Instance.LoadScene(0);
        //UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void TitleEnterSettings()
    {
        titleMenu.SetActive(false);
        settingsMenu.SetActive(true);
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
    }

    public void TitleQuit()
    {
        Application.Quit();
    }

    public void TitleCredits()
    {
        titleMenu.SetActive(false);
        credits.SetActive(true);
    }

    public void SettingsViewControls()
    {
        settingsMenu.SetActive(false);
        viewControls.SetActive(true);
    }

    public void SettingsFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void SettingsBack()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void PausedResume()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void PausedSettings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
    }

    public void PausedQuit()
    {
        Application.Quit();
    }

    public void ScreenControlsBack()
    {
        viewControls.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void ScreenCreditsBack()
    {
        credits.SetActive(false);
        titleMenu.SetActive(true);
    }


    private void Update()
    {
        // Pause game
        if (input.PlayerController.Escape.WasPressedThisFrame())
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }


    }
}
