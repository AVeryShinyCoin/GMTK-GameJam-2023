using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public bool loop;
    public float volume = 1f;
    public float pitch = 1f;

    [HideInInspector]
    public AudioSource source;
}
