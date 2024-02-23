using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMainScript : MonoBehaviour
{
    /*// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    public GameObject Settings;
    public GameObject Menu;

    public void OnClickPlay()
    {
        SceneManager.LoadScene(sceneBuildIndex:1);
        
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
