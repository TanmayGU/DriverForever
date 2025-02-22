using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    // List of heart UI images representing the player's health
    public List<Image> hearts;

    // Keeps track of the current number of hearts remaining
    private int currentHearts;

    // Reference to the UI canvas that displays hearts
    public GameObject heartCanvas;

    void Start()
    {
        // Initialize the current heart count based on the number of heart images
        currentHearts = hearts.Count;

        // Hide the heart UI at the beginning of the game
        heartCanvas.SetActive(false);
    }

    void Update()
    {
        // Ensure the heart UI is only shown when the game has started
        if (GameManager.gameStarted && !heartCanvas.activeSelf)
        {
            heartCanvas.SetActive(true);
            Debug.Log("HeartCanvas Activated!");
        }
    }

    // Reduces the number of hearts when the player takes damage
    public void LoseHeart()
    {
        Debug.Log($"Losing heart! Current hearts: {currentHearts}");

        // Ensure hearts can only be removed if at least one remains
        if (currentHearts > 0)
        {
            currentHearts--;

            // Disable the UI representation of the lost heart
            if (hearts[currentHearts] != null)
            {
                Debug.Log($"Disabling heart {currentHearts}");
                hearts[currentHearts].enabled = false;
            }
            else
            {
                Debug.LogError($"ERROR: hearts[{currentHearts}] is null! Check Inspector.");
            }

            // If no hearts remain, trigger game over
            if (currentHearts <= 0)
            {
                GameOver();
            }
        }
    }

    // Handles game over state when the player runs out of hearts
    private void GameOver()
    {
        Debug.Log("Game Over!");

        // Load the game over scene
        SceneManager.LoadScene("End");
    }
}
