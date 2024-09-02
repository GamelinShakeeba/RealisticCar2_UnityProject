using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyCar : MonoBehaviour
{
    public GameObject theCar;
    public GameObject gameOverText;
    public void CarDestroy()
    {
        StartCoroutine(OpenMenuLevel());
        Destroy(theCar.gameObject);
    }

    IEnumerator OpenMenuLevel() 
    {
        yield return new WaitForSeconds(2f);
        gameOverText.SetActive(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
    }
}
