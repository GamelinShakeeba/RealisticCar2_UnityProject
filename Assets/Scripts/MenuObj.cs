using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuObj : MonoBehaviour
{
    public GameObject Controls;
    public void PlayLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void ControlsOpen()
    {
        Controls.SetActive(true);
    }

    public void ControlsCloses()
    {
        Controls.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
