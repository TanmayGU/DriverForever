using UnityEngine;

public class CameraController : MonoBehaviour
{
    // The target the camera should follow, set via the Unity Inspector
    [SerializeField]
    private Transform target;

    // The offset distance between the camera and the target
    private Vector3 offset;

    // Speed at which the camera follows the target
    public float followSpeed = 5f;

    void Start()
    {
        // Define the default vertical and depth offsets for the camera
        float baseOffsetY = 3.5f;
        float baseOffsetZ = -7f;

        // Initialize the offset vector with fixed X, adjustable Y and Z
        offset = new Vector3(0, baseOffsetY, baseOffsetZ);

        // Check if a target is assigned and log the result
        if (target != null)
        {
            Debug.Log($"CameraController initialized with target: {target.name}, Offset: {offset}");
        }
        else
        {
            Debug.LogWarning("CameraController: Target not set at Start().");
        }
    }

    void LateUpdate()
    {
        // Ensure the game has started and that the target exists before updating the camera
        if (!GameManager.gameStarted || target == null) return;

        // Calculate the target position by applying the offset to the target's current position
        Vector3 targetPosition = target.position + offset;

        // Smoothly interpolate the camera's position toward the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Ensure the camera always looks at the target
        transform.LookAt(target);
    }

    // Method to dynamically set a new target for the camera
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log($"Camera target set to: {target.name}");
    }
}
