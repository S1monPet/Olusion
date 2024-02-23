using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSettingsUI : MonoBehaviour
{

    public GameObject Settings;
    public GameObject SettingsOpen;

    public void onClickExit()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }
    
    public void onClickSettingsOpen()
    {
        Settings.SetActive(true);
        SettingsOpen.SetActive(false);
    }

    public void onClickSettingsClose()
    {
        Settings.SetActive(false);
        SettingsOpen.SetActive(true);
    }
}
