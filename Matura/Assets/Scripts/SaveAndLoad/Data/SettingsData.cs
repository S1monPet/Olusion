using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class SettingsData
{
    // Settings
    public bool SoundEnabled;

    public SettingsData()
    {
        this.SoundEnabled = true;
    }
}
