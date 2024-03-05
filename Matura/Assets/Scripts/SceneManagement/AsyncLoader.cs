using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
public class AsyncLoader : MonoBehaviour
{
    //public static AsyncLoader Instance;

    [Header("Menu Screens")]
    [SerializeField] private GameObject loadingScreen;

    [Header("Slider")]
    [SerializeField] private Slider loadingSlider;

    public float postLoadDelay = 1.0f;

    /*
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    */
    public void LoadLevelButton(string levelToLoad) //Button
    {
        loadingScreen.SetActive(true);

        StartCoroutine(LoadLevelAsync(levelToLoad));
    }

    private IEnumerator LoadLevelAsync(string levelToLoad)
    {
        var loadingOperation = SceneManager.LoadSceneAsync(levelToLoad); //AsyncOperation
        loadingOperation.allowSceneActivation = false; 

        while (loadingOperation.progress < 0.9f)
        {
            loadingSlider.value = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            yield return null; 
        }

        loadingSlider.value = 1f; 
        yield return new WaitForSeconds(postLoadDelay);

        loadingOperation.allowSceneActivation = true;

        //If it would be singleton we set loadingScreen.SetActive(false); 
    }
}
