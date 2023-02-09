using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoad : MonoBehaviour
{
    public GameObject loadScreen;
    public void Play()
    {
        loadScreen.SetActive(true);
        SceneManager.LoadScene("Gameplay");

    }
    public void Settings()
    {
        SceneManager.LoadScene("Settings");

    }
    public void Return()
    {
        SceneManager.LoadScene("MainMenu");

    }
}
