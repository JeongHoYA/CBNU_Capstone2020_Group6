using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildMenu : MonoBehaviour
{
    public World world;
    public GameObject pauseMenu;

    public void FromBuildQuitMenuToMainMenu()
    {

    }

    public void FromPauseToGame()
    {
        pauseMenu.SetActive(false);
        world.inPause = false;
    }
}
