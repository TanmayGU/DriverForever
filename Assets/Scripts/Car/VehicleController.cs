using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public float moveSpeed = 2f; // �ƶ��ٶ�
    public float turnSpeed = 50f; // ת���ٶ�
    private Vector3 targetPosition; // Ŀ��λ��
    private bool isMoving = false; // �Ƿ������ƶ�

    void Update()
    {
        // ��ⴥ����Ļ
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // �������߼������λ��
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    targetPosition = hit.point; // ����Ŀ��λ��
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
        // ���㷽��
        Vector3 direction = (targetPosition - transform.position).normalized;

        // ƽ����ת����
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);

        // �ƶ�����
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // ֹͣ����
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
        }
    }
}

