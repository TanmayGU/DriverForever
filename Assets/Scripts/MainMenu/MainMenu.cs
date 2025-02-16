using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        GameManager.ResetGame(); // Reset the game state
        GameManager.ResetARSession(); // Reset the AR session
        SceneManager.LoadSceneAsync(1); // Load the game scene
    }



    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit.");
    }

}
