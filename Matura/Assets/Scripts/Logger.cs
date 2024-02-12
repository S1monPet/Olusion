using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("MessageLogger/Logger")]
public class Logger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private bool _showLogs; 

    public void Log(object message, Object sender)
    {
        if (_showLogs)
            Debug.Log(message, sender);
    }
}
