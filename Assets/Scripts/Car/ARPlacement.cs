using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class ARPlacement : MonoBehaviour
{
    public GameObject roadPrefab; // Reference to the initial road prefab
    private ARRaycastManager raycastManager;
    private GameObject placedObject;

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && placedObject == null)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();

                // Perform a raycast to detect surfaces
                if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    // Place the road prefab at the detected position
                    placedObject = Instantiate(roadPrefab, hitPose.position, hitPose.rotation);

                    // Initialize the RoadManager
                    RoadManager roadManager = placedObject.GetComponentInChildren<RoadManager>();
                    if (roadManager != null)
                    {
                        roadManager.zSpawn = hitPose.position.z;
                        roadManager.startPosition = hitPose.position;
                    }
                }
            }
        }
    }
}
