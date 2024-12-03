using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausMenu : MonoBehaviour
{
    [SerializeField] GameObject pausMenu;
    public void Pause() { 
        pausMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Home() {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;

    }

    public void Resume() {
        pausMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;

    }

}
