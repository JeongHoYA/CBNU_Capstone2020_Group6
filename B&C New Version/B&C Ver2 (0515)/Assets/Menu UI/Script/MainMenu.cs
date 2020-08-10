using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void NewBuildMap()
    {
        SceneManager.LoadScene("Build Mode", LoadSceneMode.Single);
    }

    public void LoadBuildMap()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
