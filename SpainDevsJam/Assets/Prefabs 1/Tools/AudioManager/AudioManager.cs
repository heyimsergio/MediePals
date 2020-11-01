using UnityEngine;
using System;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour{

    [Tooltip("The music mixer.")]
    [SerializeField] AudioMixerGroup mainMixer = default;
    [Tooltip("The SFX mixer.")]
    [SerializeField] AudioMixerGroup sfxMixer = default;

    [Space]
    [SerializeField] Sound[] sounds = default;

    public static AudioManager instance;

    // Start is called before the first frame update
    void Awake(){
        if (instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();

            if(s.type == Sound.Type.Music)
                s.source.outputAudioMixerGroup = mainMixer;
            else
                s.source.outputAudioMixerGroup = sfxMixer;

            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
        }
    }

    #region Play
    /// <summary> Play a sound with a certain name setted on the inspector. </summary>
    public void Play(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null){
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.Play();
    }

    /// <summary> Play a sound with a certain name setted on the inspector.
    /// Set the randomPitch to true to make the sound start with a random pitch variation setted on the inspector. </summary>
    public void Play(string name, bool randomPitch){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null){
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (randomPitch){
            s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
        }else{
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
        s.source.PlayOneShot(s.clip);
    }

    /// <summary> Play a sound with a certain name setted on the inspector.
    /// Make the sound start with a certain delay. </summary>
    public void Play(string name, float delay){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null){
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.PlayDelayed(delay);
    }

    /// <summary> Play a sound with a certain name setted on the inspector.
    /// Play it with a randomPitch and delay. </summary>
    public void Play(string name, float delay, bool randomPitch){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null){
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (randomPitch){
            s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
        }else{
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
        s.source.PlayDelayed(delay);
    }
    #endregion

    #region PlayOneShot
    /// <summary> Play a sound with a certain name without overlapping. </summary>
    public void PlayOneShot(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null){
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.PlayOneShot(s.clip);
    }

    /// <summary> Play a sound with a certain name without overlapping. 
    /// Set the randomPitch to true to make the sound start with a random pitch variation setted on the inspector. </summary>
    public void PlayOneShot(string name, bool randomPitch){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null){
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (randomPitch){
            s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
        }else{
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
        s.source.PlayOneShot(s.clip);
    }
    #endregion

    /// <summary> Pause a sound with a certain name setted on the inspector. </summary>
    public void Pause(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null){
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Pause();
    }

    /// <summary> Unpause a sound with a certain name setted on the inspector. </summary>
    public void UnPause(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null){
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.UnPause();
    }

    /// <summary> Stop a sound with a certain name setted on the inspector. </summary>
    public void Stop(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null){
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }

    public void StopAll(){
        foreach(Sound s in sounds){
            if(s.source != null){
                s.source.Stop();
            }
        }
    }

    AudioSource GetSource(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return null;
        }

        return s.source;
    }

    public IEnumerator FadeOut(string name, float FadeTime)
    {
        AudioSource audioSource = GetSource(name);
        if (audioSource != null && audioSource.isPlaying)
        {
            float startVolume = audioSource.volume;

            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = startVolume;
        }
    }

    public IEnumerator FadeIn(string name, float FadeTime)
    {
        AudioSource audioSource = GetSource(name);
        if (audioSource != null && !audioSource.isPlaying)
        {
            float volume = audioSource.volume;
            audioSource.volume = 0;
            audioSource.Play();

            while (audioSource.volume < volume)
            {
                audioSource.volume += Time.deltaTime / FadeTime;

                yield return null;
            }
        }

    }
}
