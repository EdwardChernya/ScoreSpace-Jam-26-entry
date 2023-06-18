using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject menuObject;
    public GameObject guideObject;
    
    public void StartGame()
    {
        SceneManager.LoadScene("TestScene");
    }
    public void OpenGuide()
    {
        menuObject.SetActive(false);
        guideObject.SetActive(true);
    }
    public void CloseGuide()
    {
        menuObject.SetActive(true);
        guideObject.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
