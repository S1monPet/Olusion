using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour, ISettingsData
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private AudioSource _ambientSound;

    public static bool SoundEnabled = true; 

    public void LoadSettingsData(SettingsData data) 
    {
        SoundEnabled = data.SoundEnabled; 
    }

    public void SaveSettingsData(ref SettingsData data)
    {
        data.SoundEnabled = SoundEnabled;
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Check if we can play and play
            UpdateSoundState(); 
        }  
        else
        {
            Destroy(gameObject);
        }

    }
    public void EnableSounds()
    {
        if (!SoundEnabled)
        {
            SoundEnabled = true;
            UpdateSoundState();
        }
    }

    public void DisableSounds()
    {
        if (SoundEnabled)
        {
            SoundEnabled = false;
            UpdateSoundState();
        }
    }

    public void PauseSound()
    {
        if (SoundEnabled)
        {
            _ambientSound.Pause(); 
        }
    }

    public void UnPauseSound()
    {
        if (SoundEnabled)
        {
            _ambientSound.Play(); 
        }
    }

    private void UpdateSoundState()
    {
        if (SoundEnabled)
        {
            // Checking if it's not already playing
            if (!_ambientSound.isPlaying)
            {
                _ambientSound.Play(); 
            }
        }
        else
        {
            _ambientSound.Stop();
        }
    }

    public void PlaySound(AudioClip audioClip, Transform spawnTransform, float volume) 
    {
        if (!SoundEnabled)
            return; 

        AudioSource audioSource = Instantiate(_audioSource, spawnTransform.position, Quaternion.identity); 

        audioSource.clip = audioClip;   

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    // Random sound effects
    public void PlayRandomSound(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        if (!SoundEnabled)
            return;

        int random = Random.Range(0, audioClip.Length);

        AudioSource audioSource = Instantiate(_audioSource, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip[random];

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
}
