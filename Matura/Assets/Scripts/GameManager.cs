using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class GameManager : MonoBehaviour
{
    private float fps;
    public TextMeshProUGUI FPSText;

    private void GetFPS()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        FPSText.text = fps.ToString();
    }

    private void Start()
    {
        InvokeRepeating("GetFPS", 1, 1);
    }

    private void Update()
    {
        //Will be for keeping trees, when they despawn to respawn, enemies over time, etc.

    }
}
