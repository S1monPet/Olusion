using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeathUI : MonoBehaviour
{
    public GameObject canvas; //For disabling canvas
    public CanvasGroup deathScreenUIGroup;

    public SurvivalSceneManager survivalSceneManager;
    public float fadeDuration = 3f;

    public Image enemyImage;
    public TextMeshProUGUI enemyNameText; 

    public void ChangeScreen(Sprite enemySprite, string enemyName)
    {
        canvas.SetActive(false); //Hide playing UI

        survivalSceneManager.UpdateSurvivedTimer();
        SetEnemySprite(enemySprite);
        SetEnemyNameText(enemyName);

        StartCoroutine(DeathScreenFadeIn()); 
    }

    private void SetEnemySprite(Sprite enemySprite)
    {
        if (enemySprite == null) return; //I don't know what killed you

        enemyImage.sprite = enemySprite;
        //enemyImage.SetNativeSize(); //For setting native size of image
    }

    private void SetEnemyNameText(string enemyName)
    {
        if (enemyName == null) return;
        enemyNameText.text = enemyName; 
    }

    private IEnumerator DeathScreenFadeIn()
    {
        float elapsedTime = 0f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            deathScreenUIGroup.alpha = elapsedTime / fadeDuration;
            yield return null; 
        }

        deathScreenUIGroup.alpha = 1f;
    }
}
