using UnityEngine;

[System.Serializable]
public class Sound{
    [Tooltip("Name of the sound. Each name has to be different between each other.")]
    public string name;

    public AudioClip clip;

    [System.Serializable] public enum Type {Music, SFX}
    [Space]
    [Tooltip("Is it part of the music or the SFX?")] public Type type;

    [Space]
    [Tooltip("Default volume of the sound.")] [Range(0f, 1f)] public float volume = 1;
    [Tooltip("Max random volume variation of the sound.")] [Range(0f, 1f)] public float volumeVariance;
    [Space]
    [Tooltip("Default pitch of the sound.")] [Range(.1f, 3f)] public float pitch = 1;
    [Tooltip("Max random pitch variation of the sound.")] [Range(0f, 1f)] public float pitchVariance;

    public bool loop;

    [HideInInspector] public AudioSource source;
}

