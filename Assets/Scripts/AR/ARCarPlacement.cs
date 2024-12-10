using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARCarPlacement : MonoBehaviour
{
    public GameObject carPrefab; // Assign your car prefab in the Inspector
    private GameObject spawnedCar; // The car object that will be placed in AR
    private bool gameStarted = false; // Track if the game has started or not

    private ARRaycastManager raycastManager; // AR raycasting manager
    private List<ARRaycastHit> hits = new List<ARRaycastHit>(); // List to store raycast hits
    private ARPlaneManager planeManager; // AR Plane manager to detect planes

    public GameObject groundMessage; // A message or UI element that tells the user to tap on the ground

    void Start()
    {
        // Find the AR Raycast Manager and AR Plane Manager in the scene
        raycastManager = FindObjectOfType<ARRaycastManager>();
        planeManager = FindObjectOfType<ARPlaneManager>();

        // Disable the ground message initially
        if (groundMessage != null)
        {
            groundMessage.SetActive(true); // Show message to tap on the detected plane
        }
    }

    void Update()
    {
        // If the game hasn't started yet
        if (!gameStarted)
        {
            // Check if there's any touch input on the screen
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Vector2 touchPosition = Input.GetTouch(0).position;

                // Perform raycast to detect AR planes
                if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinBounds))
                {
                    // Get the position and rotation of the detected plane hit
                    Pose hitPose = hits[0].pose;

                    // If the car isn't spawned, instantiate it at the touch position
                    if (spawnedCar == null)
                    {
                        spawnedCar = Instantiate(carPrefab, hitPose.position, hitPose.rotation);
                    }
                    else
                    {
                        // If the car is already placed, move it to the new position
                        spawnedCar.transform.position = hitPose.position;
                    }

                    // Hide the message and start the game
                    if (groundMessage != null)
                    {
                        groundMessage.SetActive(false);
                    }

                    // Set gameStarted to true to indicate the game has started
                    gameStarted = true;
                }
            }
        }
        else
        {
            // Handle the gameplay logic after the game has started (e.g., car movement)
        }
    }
}
