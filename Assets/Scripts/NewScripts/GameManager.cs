using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject car;  // Reference to the car
    private bool gameRunning = false;  // Flag to check if the game is running

    // Function to start the game
    public void StartGame()
    {
        if (!gameRunning)
        {
            car.GetComponent<CarController>().enabled = true;  // Enable car movement
            gameRunning = true;  // Set the gameRunning flag to true
        }
    }
}