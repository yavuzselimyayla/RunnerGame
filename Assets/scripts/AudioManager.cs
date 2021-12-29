using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class AudioManager : MonoBehaviour {

    //array of sounds
    [Header("Sounds")]
    public Sound[] sounds;

    //singleton instance
    public static AudioManager instance;

    //gamemanager instance
    protected GameManager gameManager;

    //name of the background music to play
    public String backgroundMusicName;

    //called before start
    private void Awake()
    {
        //Find the game manager
        gameManager = FindObjectOfType<GameManager>();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //persist across scenes
        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.loop = sound.loop;
            sound.source.playOnAwake = false;
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
        }
    }

    //play backgroud music (based on whats playing etc)
    //Needs refactoring to make pretty
    public void PlayBackgroundMusic()
    {

        String menuMusicName = "menuMusic";
        String gameMusicName = "gameMusic";
        Sound menuMusic = FindSoundByName(menuMusicName);
        Sound gameMusic = FindSoundByName(gameMusicName);

        backgroundMusicName = gameManager.IsPlayerInMenu() ? menuMusicName : gameMusicName;

        //want to play menu music
        if (backgroundMusicName == menuMusicName)
        {
           if(menuMusic.source.isPlaying != true)
            {
                Play(menuMusicName);
                Stop(gameMusicName);
            }
        }

        //Want to play game music
        if(backgroundMusicName == gameMusicName)
        {
        
            if (gameMusic.source.isPlaying != true)
            {
                Play(gameMusicName);
                Stop(menuMusicName);
            }
        }
       
       
    }

    //Find the sound with the name
    public Sound FindSoundByName(String name)
    {
        Sound sound = null;
        foreach (Sound s in sounds)
        {
           // Debug.LogError("Sound Name: " + s.name);
            if (s.name == name)
            {
                sound = s;
                break;
            }
        }

        return sound; 
    }

    /// <summary>
    /// Stops a currently playing clip from playing (if it's playing)
    /// </summary>
    /// <param name="name"></param>
    public void Stop(string name)
    {
        Sound foundSound = FindSoundByName(name);
        if(foundSound != null)
        {
            if (foundSound.source.isPlaying)
            {
                foundSound.source.Stop();
            }
            else
            {
                Debug.LogWarning("Sound " + name + " was asked to stop, but it was not playing");
            }
        }
        else
        {
            Debug.LogError("No sound found called: " + name);
        }
    }


    /// <summary>
    /// Play a clip by it's given name
    /// </summary>
    /// <param name="name"></param>
    public void Play(string name)
    {

        Sound foundSound = FindSoundByName(name);

       //Play sound
        if (foundSound.source != null)
        {
            foundSound.source.Play();
        }
        else
        {
            Debug.LogError("No sound found called: " + name);
        }
       

    }


    /// <summary>
    /// Adjust a clip to change it's volume immediately 
    /// </summary>
    /// <param name="name">Name of the clip to adjust volumne</param>
    /// <param name="volume">Volume from 0.0f to 1.0f</param>
    public void AdjustClipVolume(string name, float volume = 1.0f)
    {
        Sound targetSound = null;
        foreach(Sound sound in sounds)
        {
            Debug.Log(sound.name);
           
            if(sound.name == name)
            {
                targetSound = sound;
            }
        }

        if(targetSound != null)
        {
            Debug.Log("Changing sound " + name  + " to be volume " + volume);
            targetSound.source.volume = volume;
        }
        else
        {
            Debug.LogError("The sound " + name  + " could not be found and its sound adjusted"); 
        }
    }

    /// <summary>
    /// Adjust a sound clip from current volume to target over an interval
    /// </summary>
    /// <param name="name"></param>
    /// <param name="volume"></param>
    /// <param name="time"></param>
    public void AdjustClipOverTime(string name, float volume, float time)
    {

    }


}
