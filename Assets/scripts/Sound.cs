using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound  {

    [Tooltip("Internal name used to play the clip")]
    public string name;

    [Tooltip("Select the audio clip that will be played")]
    public AudioClip clip;

    [Range(0.0f, 1.0f)]
    [Tooltip("The volume from 0 - 100%")]
    public float volume = 1f;

    [Range(0.1f, 3.0f)]
    public float pitch;

    [Tooltip("if this should loop or not")]
    public bool loop = false;

    //assigned in awake method in audio manager
    [HideInInspector]
    public AudioSource source; 
}
