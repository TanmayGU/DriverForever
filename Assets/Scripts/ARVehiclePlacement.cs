using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ARVehiclePlacement : MonoBehaviour
{
    public GameObject vehiclePrefab; // ����Ԥ����
    private GameObject placedVehicle; // �ѷ��õĳ���ʵ��
    private ARRaycastManager raycastManager;

    void Start()
    {
        // ��ȡ AR Raycast Manager ���
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        // ����û��д�������
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // �������߼����λ��
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    // ��ȡ�������е�ƽ��λ��
                    Pose hitPose = hits[0].pose;

                    if (placedVehicle == null)
                    {
                        // �������δ���ã�ʵ��������
                        placedVehicle = Instantiate(vehiclePrefab, hitPose.position, hitPose.rotation);
                    }
                    else
                    {
                        // ��������ѷ��ã����³���λ��
                        placedVehicle.transform.position = hitPose.position;
                    }
                }
            }
        }
    }
}

