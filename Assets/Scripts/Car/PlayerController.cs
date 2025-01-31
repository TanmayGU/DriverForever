using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 targetScale;
    public float squeezeSpeed = 5f;

    private AudioSource audioSource;
    private const int sampleRate = 48000;
    private const int sampleSize = 1024;
    private float[] audioSamples = new float[sampleSize];
    public float volumeThreshold = 0.02f;

    public float laneFactor = 0.88f; // Ideal lane factor for a base road width
    private float laneDistance;
    private int desiredLane = 1; // Default to center lane
    private RoadManager roadManager;

    public float roadSpeed = 10f; // Speed at which roads move
    private float baseRoadWidth = 0.3428473f; // Reference road width for laneFactor calculation

    private int hitCount = 0; // Counter for wall collisions
    public int maxHits = 5; // Maximum number of hits before game over

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        InitializeMicrophone();

        roadManager = FindObjectOfType<RoadManager>();
        if (roadManager != null)
        {
            float dynamicRoadWidth = GetDynamicRoadWidth();
            laneDistance = laneFactor * (dynamicRoadWidth / baseRoadWidth);
            Debug.Log($"Dynamic lane distance calculated: {laneDistance} (Dynamic road width: {dynamicRoadWidth})");
        }
        else
        {
            Debug.LogWarning("RoadManager not found! Using default lane distance.");
            laneDistance = 3f; // Default fallback
        }
    }

    void Update()
    {
        AnalyzeAudio();

        if (!GameManager.gameStarted) return;

        HandleTouchInput();

        // Move roads
        roadManager.UpdateRoads(roadSpeed);

        // Adjust position based on the desired lane
        float targetX = (desiredLane - 1) * laneDistance; // Center lane is 1
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, 10f * Time.deltaTime);
    }

    private void InitializeMicrophone()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.mute = true;

        if (Microphone.devices.Length > 0)
        {
            string selectedMic = Microphone.devices[0];
            audioSource.clip = Microphone.Start(selectedMic, true, 1, sampleRate);

            if (Microphone.IsRecording(selectedMic))
            {
                Debug.Log($"Microphone '{selectedMic}' is recording.");
                while (!(Microphone.GetPosition(selectedMic) > 0)) { }
                audioSource.Play();
            }
            else
            {
                Debug.LogError($"Failed to start recording with Microphone: {selectedMic}");
            }
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    private void AnalyzeAudio()
    {
        if (audioSource.clip == null)
        {
            Debug.Log("AudioSource clip is null. Resetting scale.");
            ResetScale();
            return;
        }

        audioSource.clip.GetData(audioSamples, 0);
        float volume = GetVolume(audioSamples);

        float silenceThreshold = 0.0003f;
        float lowVolumeThreshold = 0.1f;
        float highVolumeThreshold = 0.3f;

        if (volume < silenceThreshold)
        {
            targetScale = originalScale;
            Debug.Log("Silence detected: Keeping original shape.");
        }
        else if (volume < lowVolumeThreshold)
        {
            targetScale = new Vector3(originalScale.x * 1.5f, originalScale.y * 0.5f, originalScale.z);
            Debug.Log("Low Volume detected: Shrinking to wide and short form.");
        }
        else if (volume > highVolumeThreshold)
        {
            targetScale = new Vector3(originalScale.x * 0.5f, originalScale.y * 1.5f, originalScale.z);
            Debug.Log("High Volume detected: Stretching to tall and thin form.");
        }
        else
        {
            targetScale = originalScale;
            Debug.Log("Normal Volume: Keeping original shape.");
        }

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * squeezeSpeed);
    }

    private float GetVolume(float[] samples)
    {
        float sum = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }

        return Mathf.Sqrt(sum / samples.Length);
    }

    private void ResetScale()
    {
        targetScale = originalScale;
    }

    private float GetDynamicRoadWidth()
    {
        foreach (GameObject road in roadManager.RoadSegments)
        {
            return road.transform.localScale.x; // Dynamic width of the road prefab
        }

        Debug.LogWarning("No road segments found. Using base road width.");
        return baseRoadWidth; // Fallback to the base reference width
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;

            if (touchPosition.x < Screen.width / 2) MoveLeft();
            else MoveRight();
        }
    }

    public void MoveLeft()
    {
        if (desiredLane > 0) desiredLane--;
        Debug.Log($"Moved to lane {desiredLane}");
    }

    public void MoveRight()
    {
        if (desiredLane < 2) desiredLane++;
        Debug.Log($"Moved to lane {desiredLane}");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is tagged as "Wall"
        if (collision.gameObject.CompareTag("Wall"))
        {
            hitCount++; // Increment the hit counter
            Debug.Log("Wall hit count: " + hitCount);

            if (hitCount >= maxHits)
            {
                GameOver(); // Trigger the game-over logic
            }
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        SceneManager.LoadScene("End"); // Load a game-over scene
    }
}
