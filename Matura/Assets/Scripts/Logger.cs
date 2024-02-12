using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("MessageLogger/Logger")]
public class Logger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private bool _showLogs;

    [SerializeField]
    private string _prefix;

    [SerializeField]
    private Color _prefixColor;

    private string _hexColor;

    private void OnValidate()
    {
        _hexColor = "#" + ColorUtility.ToHtmlStringRGBA(_prefixColor);
    }

    public void Log(object message, Object sender)
    {
        if (!_showLogs) return; 

        Debug.Log($"<color={_prefixColor}> {_prefix}: {message}", sender);
    }
}
