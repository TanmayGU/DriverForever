// CameraController.cs
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private Vector3 offset;
    public float followSpeed = 5f;

    void Start()
    {
        float baseOffsetY = 3.5f;
        float baseOffsetZ = -7f;

        offset = new Vector3(0, baseOffsetY, baseOffsetZ);

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
        if (!GameManager.gameStarted || target == null) return;

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        transform.LookAt(target);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log($"Camera target set to: {target.name}");
    }
}
