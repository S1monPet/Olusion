using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNight : MonoBehaviour
{
    [Header("Enviroment")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Gradient fogGradient; 
    [SerializeField] private Gradient ambientLight; 
    [SerializeField] private Gradient directionalLightGradient; 
    

    [Header("Variables")]
    [SerializeField] private float dayDurationInSeconds = 60f;
    [SerializeField] private float rotationSpeed = 1f;
    private float currentTime = 0f;

    private void Update()
    {
        UpdateTime();
        UpdateDayNightCycle(); 
    }

    private void UpdateTime()
    {
        currentTime += Time.deltaTime / dayDurationInSeconds;
        currentTime = Mathf.Repeat(currentTime, 1f); //If current time goes above 1f it sets it to 1f
    }

    private void UpdateDayNightCycle()
    {
        float sunPosition = Mathf.Repeat(currentTime + 0.25f, 1f);
        directionalLight.transform.rotation = Quaternion.Euler(sunPosition * 360f, 0f, 0f);

        RenderSettings.fogColor = fogGradient.Evaluate(-currentTime);
        RenderSettings.ambientLight = ambientLight.Evaluate(currentTime);

        directionalLight.color = directionalLightGradient.Evaluate(currentTime);
    }
}
