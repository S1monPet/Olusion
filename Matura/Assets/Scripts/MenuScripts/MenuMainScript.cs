using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMainScript : MonoBehaviour
{

    public GameObject Settings;
    public GameObject Menu;

    public void OnClickPlay()
    {
        //SceneManager.LoadScene(sceneBuildIndex: 1);
        
    }

    public void OnClickSettingsOpen()
    {
        Settings.SetActive(true);
        Menu.SetActive(false);
        
    }

    public void OnClickSettingsClose()
    {
        Settings.SetActive(false);
        Menu.SetActive(true);
        
    }

    public void OnClickCogOpen()
    {

    }

    public void OnClickCogClose()
    {

    }

    public void OnClickSoundOpen()
    {

    }

    public void OnClickSoundClose()
    {

    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
