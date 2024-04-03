using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudio : MonoBehaviour
{
    [SerializeField] private AudioClip _touchSound;

    public void OnStartGameClick()
    {
        if (SoundManager.SoundEnabled)
        {
            SoundManager.Instance.PlaySound(_touchSound, transform, 0.105f);
        }
    }
    public void OnRespawnClick()
    {
        if (SoundManager.SoundEnabled)
        {
            SoundManager.Instance.PlaySound(_touchSound, transform, 0.105f);
        }
    }

    public void OnExitClick()
    {
        if (SoundManager.SoundEnabled)
        {
            SoundManager.Instance.PlaySound(_touchSound, transform, 0.105f);
        }
    }
    public void OnSettingsClick()
    {
        if (SoundManager.SoundEnabled)
        {
            SoundManager.Instance.PlaySound(_touchSound, transform, 0.105f);
        }
    }
    public void OnOpenSettingsClick()
    {
        if (SoundManager.SoundEnabled)
        {
            SoundManager.Instance.PlaySound(_touchSound, transform, 0.105f);
        }
    }
    public void OnCloseSettingsClick()
    {
        if (SoundManager.SoundEnabled)
        {
            SoundManager.Instance.PlaySound(_touchSound, transform, 0.105f);
        }
    }

    public void OnMuteClick()
    {
        if (SoundManager.SoundEnabled)
        {
            // _touchSound.Play();
            SoundManager.Instance.DisableSounds(); 
        }
        else
        {
            // We only want to play the sound when we enable it
            SoundManager.Instance.PlaySound(_touchSound, transform, 0.105f);
            SoundManager.Instance.EnableSounds(); 
        }
    }
}
