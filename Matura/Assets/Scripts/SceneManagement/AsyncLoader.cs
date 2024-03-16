using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
public class AsyncLoader : MonoBehaviour
{
    public static AsyncLoader Instance;

    [Header("Menu Screens")]
    [SerializeField] private GameObject loadingScreen;

    [Header("Slider")]
    [SerializeField] private Slider loadingSlider;

    public float postLoadDelay = 2.0f;

    
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
    
    public void LoadLevel(string levelToLoad) //Button
    {
        loadingScreen.SetActive(true);

        StartCoroutine(LoadLevelAsync(levelToLoad));
    }

    private IEnumerator LoadLevelAsync(string levelToLoad)
    {
        if (levelToLoad == "Menu")
        {
            DataPersistanceManager.Instance.SaveGame();
            yield return new WaitForSeconds(postLoadDelay);
            yield return null; 
        }
        var loadingOperation = SceneManager.LoadSceneAsync(levelToLoad); //AsyncOperation
        loadingOperation.allowSceneActivation = false; 

        while (loadingOperation.progress < 0.9f)
        {
            loadingSlider.value = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            yield return null; 
        }

        loadingSlider.value = 1f;

        loadingOperation.allowSceneActivation = true;

        yield return new WaitForSeconds(postLoadDelay);

        DataPersistanceManager.Instance.LoadGame();

        loadingScreen.SetActive(false); // Because it's a singleton, we set it to false
    }

}
