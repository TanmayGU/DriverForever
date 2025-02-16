using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

public class ARCarPlacement : MonoBehaviour
{
    public GameObject carPrefab;
    public GameObject roadPrefab;
    public ARTrackedImageManager imageManager;
    private ARSession arSession;
    public float carYOffset = 0.5f;

    private GameObject spawnedCar;
    private bool roadsInitialized = false;
    private HashSet<string> trackedImages = new HashSet<string>();

    public GameObject scanCanvas;
   

    private void Awake()
    {
        Debug.Log("ARCarPlacement:Awake() - Ensuring single instance.");
        if (FindObjectsOfType<ARCarPlacement>().Length > 1)
        {
            Debug.LogWarning("Multiple ARCarPlacement instances found! Destroying duplicate...");
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        Debug.Log("ARCarPlacement:OnEnable()");
        StartCoroutine(InitializeAR());
    }

    private IEnumerator InitializeAR()
    {
        Debug.Log("Initializing AR system...");
        arSession = FindObjectOfType<ARSession>();

        if (arSession == null)
        {
            Debug.LogWarning("ARSession not found. Waiting for initialization...");
            float waitTime = 0f;
            while (arSession == null && waitTime < 5f)
            {
                arSession = FindObjectOfType<ARSession>();
                yield return new WaitForSeconds(0.5f);
                waitTime += 0.5f;
            }
        }

        if (arSession == null)
        {
            Debug.LogError("ARSession still not found! AR may not work.");
            yield break;
        }

        Debug.Log("ARSession is now active.");
        yield return new WaitForSeconds(2f);

        Debug.Log("Resetting AR session before enabling tracking...");
        StartCoroutine(ResetARSessionCompletely());

        if (imageManager != null)
        {
            imageManager.trackedImagesChanged += OnTrackedImagesChanged;
            Debug.Log("Subscribed to ARTrackedImageManager.trackedImagesChanged.");
        }
    }

    private void OnDisable()
    {
        if (imageManager != null)
        {
            imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            Debug.Log("Unsubscribed from ARTrackedImageManager.trackedImagesChanged.");
        }
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            PlaceObjects(trackedImage);
        }
    }

    private void PlaceObjects(ARTrackedImage trackedImage)
    {
        if (roadsInitialized)
        {
            Debug.LogWarning("Placement blocked! roadsInitialized is true.");
            return;
        }

        Vector3 position = trackedImage.transform.position;

        // Force car to face forward (Z-axis) instead of using AR image rotation
        Quaternion fixedRotation = Quaternion.Euler(0, 0, 0);

        spawnedCar = Instantiate(carPrefab, position, fixedRotation);
        spawnedCar.transform.position += new Vector3(0, carYOffset, 0);

        RoadManager roadManager = FindObjectOfType<RoadManager>();
        if (roadManager != null)
        {
            float roadScaleFactor = (trackedImage.size.x) * 1.5f;
            roadManager.InitializeRoads(position, Quaternion.identity, roadScaleFactor);
        }

        roadsInitialized = true;
        scanCanvas.SetActive(false);
        GameManager.StartGame();
    }


    public IEnumerator ResetARSessionCompletely()
    {
        Debug.Log("Starting full AR session reset...");

        if (arSession == null)
        {
            Debug.LogWarning("ARSession not found. Trying again...");
            arSession = FindObjectOfType<ARSession>();

            if (arSession == null)
            {
                Debug.LogError("ERROR: ARSession still not found! Cannot reset.");
                yield break;
            }
        }

        while (arSession.subsystem != null && arSession.subsystem.running == false)
        {
            Debug.LogWarning("Waiting for ARSession to start...");
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("Resetting ARSession...");
        try
        {
            arSession.Reset();
        }
        catch (System.Exception e)
        {
            Debug.LogError("CRITICAL ERROR: Failed to reset ARSession! " + e.Message);
            yield break;
        }

        yield return new WaitForSeconds(1.5f);

        if (imageManager != null)
        {
            Debug.Log("Restarting ARTrackedImageManager...");
            imageManager.enabled = false;
            yield return new WaitForSeconds(1f);
            imageManager.enabled = true;
        }

        yield return new WaitForSeconds(1f);
        scanCanvas.SetActive(false); // Fix: Keep it hidden
        Debug.Log("ARSession fully reset.");
    }
}
