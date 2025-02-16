using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    public List<Image> hearts; // Drag your UI heart images into this list in the Unity Inspector
    private int currentHearts;
    public GameObject heartCanvas; // ✅ Drag the HeartCanvas here in the Inspector

    void Start()
    {
        currentHearts = hearts.Count; // Set starting hearts
        heartCanvas.SetActive(false); // ✅ Hide canvas initially
    }

    void Update()
    {
        if (GameManager.gameStarted && !heartCanvas.activeSelf)
        {
            heartCanvas.SetActive(true); // ✅ Show hearts when game starts
        }
    }

    public void LoseHeart()
    {
        if (currentHearts > 0)
        {
            currentHearts--;
            hearts[currentHearts].enabled = false; // Hide a heart

            if (currentHearts <= 0)
            {
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        SceneManager.LoadScene("End");
    }
}
