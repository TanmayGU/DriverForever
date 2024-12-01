using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public float moveSpeed = 2f; // 移动速度
    public float turnSpeed = 50f; // 转向速度
    private Vector3 targetPosition; // 目标位置
    private bool isMoving = false; // 是否正在移动

    void Update()
    {
        // 检测触摸屏幕
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // 发射射线检测点击的位置
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    targetPosition = hit.point; // 设置目标位置
                    isMoving = true;
                }
            }
        }

        if (isMoving)
        {
            MoveVehicle();
        }
    }

    void MoveVehicle()
    {
        // 计算方向
        Vector3 direction = (targetPosition - transform.position).normalized;

        // 平滑旋转车辆
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);

        // 移动车辆
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // 停止条件
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
        }
    }
}

