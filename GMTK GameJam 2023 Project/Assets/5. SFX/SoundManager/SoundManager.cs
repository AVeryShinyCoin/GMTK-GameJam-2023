using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Reflection;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] GameObject soundPlayerPrefab;

    [SerializeField] AudioMixerGroup musicMixer;
    [SerializeField] AudioMixerGroup sfxMixer;
    public Sound[] sounds;

    private bool delay;

    // TEST
    [SerializeField] float masterVolume;

    List<Sound> changeVolList = new List<Sound>();
    List<float> changeVolDuration = new List<float>();
    List<float> changeVolTimer = new List<float>();
    List<float> changeVolStartVolume = new List<float>();
    List<float> changeVolEndVolume = new List<float>();
    List<Sound> changeVolCulling = new List<Sound>();

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

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.volume * masterVolume;
            s.source.pitch = s.pitch;

            if (s.name.StartsWith("BGM"))
            {
                s.source.outputAudioMixerGroup = musicMixer;
            }
            else
            {
                s.source.outputAudioMixerGroup = sfxMixer;
            }
        }

    }

    public void PlaySound(string name)
    {
        PlaySound(name, 0f);
    }

    public void PlaySound(string name, float fadeInTime)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found!");
            return;
        }

        CullSoundFromChangeList(s);
        s.source.Play();

        if (fadeInTime > 0)
        {
            ChangeSoundVolume(s, 0f, 1f, fadeInTime);
        }
        else
        {
            s.source.volume = 1f;
        }
 
    }

    public void EndSound(string name)
    {
        EndSound(name, 0f);
    }

    public void EndSound(string name, float fadeOutTime)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found!");
            return;
        }

        CullSoundFromChangeList(s);
        if (fadeOutTime > 0)
        {
            ChangeSoundVolume(s, s.source.volume, 0f, fadeOutTime);
        }
        else
        {
            s.source.Stop();
        }
    }

    public void ChangeSoundVolume (string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found!");
            return;
        }
        ChangeSoundVolume(s, s.source.volume, volume, 0f);
    }

    public void ChangeSoundVolume(string name, float volume, float time)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found!");
            return;
        }
        ChangeSoundVolume(s, s.source.volume, volume, time);
    }

    private void ChangeSoundVolume(Sound s, float startVolume, float endVolume, float time)
    {
        if (endVolume < 0f || endVolume > 1f)
        {
            Debug.LogWarning("Invalid volume, must be between 0 and 1");
            return;
        }

        CullSoundFromChangeList(s);
        if (time > 0)
        {
            changeVolList.Add(s);
            changeVolDuration.Add(time);
            changeVolTimer.Add(0f);
            changeVolStartVolume.Add(startVolume);
            changeVolEndVolume.Add(endVolume);
        }
        else
        {
            s.source.volume = s.volume * endVolume;
        }
    }

    private void FixedUpdate()
    {
        IncrementVolumes();
    }

    private void IncrementVolumes()
    {
        if (changeVolList.Count > 0)
        {
            foreach (Sound s in changeVolList)
            {
                int i = changeVolList.IndexOf(s);

                float difference = changeVolEndVolume[i] - changeVolStartVolume[i];
                changeVolTimer[i] += Time.deltaTime;
                float progress = changeVolTimer[i] / changeVolDuration[i];

                float volume = changeVolStartVolume[i] + (difference * progress);
                
                if (volume < 0) volume = 0;
                if (volume > 1) volume = 1;
                s.source.volume = s.volume * volume;

                if (changeVolTimer[i] >= changeVolDuration[i])
                {
                    changeVolCulling.Add(s);
                }
            }

            if (changeVolCulling.Count > 0)
            {
                foreach (Sound s in changeVolCulling)
                {
                    if (s.source.volume == 0) s.source.Stop();
                    CullSoundFromChangeList(s);
                }
                changeVolCulling.Clear();
            }
        }
    }

    void CullSoundFromChangeList(Sound s)
    {
        if (changeVolList.Contains(s))
        {
            int i = changeVolList.IndexOf(s);
            changeVolList.RemoveAt(i);
            changeVolDuration.RemoveAt(i);
            changeVolTimer.RemoveAt(i);
            changeVolEndVolume.RemoveAt(i);
            changeVolStartVolume.RemoveAt(i);
        }   
    }


    public void PlayUniqueSound(string name)
    {
        PlayUniqueSound(name, 1f, 1f, 1f);
    }

    public void PlayUniqueSound(string name, float volume)
    {
        PlayUniqueSound(name, volume, 1f, 1f);
    }

    public void PlayUniqueSound(string name, float volume, float minPitch, float maxPitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found!");
            return;
        }

        GameObject gob = Instantiate(soundPlayerPrefab, Vector3.zero, Quaternion.identity);
        gob.transform.parent = transform;
        AudioSource gobSource = gob.GetComponent<AudioSource>();
        gobSource.clip = s.clip;
        gobSource.loop = s.loop;
        gobSource.volume = s.volume * volume * masterVolume;
        gobSource.pitch = s.pitch * UnityEngine.Random.Range(minPitch, maxPitch);

        if (s.name.StartsWith("BGM"))
        {
            gobSource.outputAudioMixerGroup = musicMixer;
        }
        else
        {
            gobSource.outputAudioMixerGroup = sfxMixer;
        }

        gobSource.Play();

        Destroy(gob, s.clip.length * gobSource.pitch);
    }



    //public void PlaySoundRandomPitch(string name, float minPitch, float maxPitch)
    //{
    //    Sound s = Array.Find(sounds, sound => sound.name == name);
    //    if (s == null)
    //    {
    //        Debug.LogWarning("Sound " + name + " not found!");
    //        return;
    //    }

    //    GameObject gob = Instantiate(soundPlayerPrefab, Vector3.zero, Quaternion.identity);

    //    UniqueAudioSource script = gob.GetComponent<UniqueAudioSource>();
    //    script.audioName = name;
    //    script.audioList = currentlyPlaying;
    //    currentlyPlaying.Add(name);

    //    AudioSource source = gob.GetComponent<AudioSource>();
    //    source.clip = s.clip;
    //    source.loop = s.loop;
    //    source.volume = s.volume * masterVolume;
    //    source.pitch = s.pitch * UnityEngine.Random.Range(minPitch, maxPitch); ;
    //    source.Play();

    //    Destroy(gob, s.clip.length / Math.Abs(s.source.pitch));

    //}

    //public void PlayUniqueSound(string name)
    //{
    //    // this ensures that a maximum number of the same sound can be played at any one time
    //    int maxInstances = 4;
    //    int instances = 0;
    //    for (int i = 0; i < maxInstances; i++)
    //    {
    //        if (currentlyPlaying.Contains(name))
    //        {
    //            currentlyPlaying.Remove(name);
    //            instances++;
    //        } else
    //        {
    //            break;
    //        }
    //    }
    //    for (int i = 0; i < instances; i++)
    //    {
    //        currentlyPlaying.Add(name);
    //    }
    //    if (instances >= maxInstances) return;


    //    Sound s = Array.Find(sounds, sound => sound.name == name);
    //    if (s == null)
    //    {
    //        Debug.LogWarning("Sound " + name + " not found!");
    //        return;
    //    }

    //    GameObject gob = Instantiate(soundPlayerPrefab, Vector3.zero, Quaternion.identity);

    //    UniqueAudioSource script = gob.GetComponent<UniqueAudioSource>();
    //    script.audioName = name;
    //    script.audioList = currentlyPlaying;
    //    currentlyPlaying.Add(name);

    //    AudioSource source = gob.GetComponent<AudioSource>();
    //    source.clip = s.clip;
    //    source.loop = s.loop;
    //    source.volume = s.volume * masterVolume;
    //    source.pitch = s.pitch;
    //    source.Play();

    //    Destroy(gob, s.clip.length);
    //}

    //private void SoundDelay()
    //{
    //    delay = false;
    //}

}
