using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Ticker : MonoBehaviour
{
    public static float TickTime = .2f;

    private float _tickerTimer; 

    public delegate void TickAction(); 
    public static event TickAction OnTickAction;

    private void Update()
    {
        _tickerTimer += Time.deltaTime; 

        if (_tickerTimer >= TickTime)
        {
            _tickerTimer = 0;
            TickEvent(); 
        }
    }

    private void TickEvent()
    {
        OnTickAction?.Invoke(); 
    }

}
