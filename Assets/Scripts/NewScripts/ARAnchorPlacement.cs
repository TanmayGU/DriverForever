using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARAnchorPlacement : MonoBehaviour
{
    public GameObject anchorPrefab;  // Prefab to instantiate as anchor
    public GameObject gameManager;  // Reference to the GameManager
    private ARRaycastManager raycastManager;  // AR Raycast Manager to handle raycasting
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();  // List to store raycast hits
    private bool gameStarted = false;  // Flag to check if the game has started

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();  // Get the ARRaycastManager component
    }

    void Update()
    {
        // Check if the game hasn't started and there is a touch input
        if (!gameStarted && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // Perform a raycast to detect planes
                if (raycastManager.Raycast(touch.position, hits, TrackableType.Planes))
                {
                    var hitPose = hits[0].pose;
                    // Instantiate the anchor prefab at the hit pose
                    Instantiate(anchorPrefab, hitPose.position, hitPose.rotation);

                    // Notify GameManager to start the game
                    gameManager.GetComponent<GameManager>().StartGame();

                    gameStarted = true;  // Set the gameStarted flag to true
                }
            }
        }
    }
}
