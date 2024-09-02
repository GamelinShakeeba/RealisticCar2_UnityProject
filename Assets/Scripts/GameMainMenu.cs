using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMainMenu : MonoBehaviour
{
    public void GoToGameMenu()
    {
        SceneManager.LoadScene(0);
    }
}
