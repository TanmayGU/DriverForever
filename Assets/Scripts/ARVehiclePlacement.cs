using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ARVehiclePlacement : MonoBehaviour
{
    public GameObject vehiclePrefab; // 车辆预制体
    private GameObject placedVehicle; // 已放置的车辆实例
    private ARRaycastManager raycastManager;

    void Start()
    {
        // 获取 AR Raycast Manager 组件
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        // 如果用户有触摸输入
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // 发射射线检测点击位置
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    // 获取射线命中的平面位置
                    Pose hitPose = hits[0].pose;

                    if (placedVehicle == null)
                    {
                        // 如果车辆未放置，实例化车辆
                        placedVehicle = Instantiate(vehiclePrefab, hitPose.position, hitPose.rotation);
                    }
                    else
                    {
                        // 如果车辆已放置，更新车辆位置
                        placedVehicle.transform.position = hitPose.position;
                    }
                }
            }
        }
    }
}

