using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRoad : MonoBehaviour
{
    public GameObject roadPrefab; // ��·�ε�Ԥ����
    public int numberOfRoads = 5; // ��ʼ��ʱ��·�ε�����
    public float roadLength = 50f; // ÿ����·�εĳ���
    public Transform player; // ��Ҷ���
    public float maxHeightChange = 5f; // ����߶ȱ仯��Χ
    public float maxWidthChange = 2f; // �����ȱ仯��Χ
    public float maxCurveAngle = 30f; // ��������Ƕȷ�Χ

    private Queue<GameObject> roads; // �洢��ǰ��·�εĶ���
    private float spawnZ = 0f; // ��һ����·�����ɵ�λ��

    void Start()
    {
        roads = new Queue<GameObject>();
        for (int i = 0; i < numberOfRoads; i++)
        {
            SpawnRoad();
        }
    }

    void Update()
    {
        // �ж�����Ƿ��ѽӽ����һ����·��
        if (player.position.z > spawnZ - numberOfRoads * roadLength)
        {
            SpawnRoad();
        }
    }

    // �����µĵ�·��
    void SpawnRoad()
    {
        GameObject road = Instantiate(roadPrefab);
        road.transform.position = new Vector3(0, Random.Range(-maxHeightChange, maxHeightChange), spawnZ);

        // ���������·�εĿ��
        road.transform.localScale = new Vector3(Random.Range(1f, 1f + maxWidthChange), 1f, 1f);

        // �����ת��·����ʵ������Ч��
        float curveAngle = Random.Range(-maxCurveAngle, maxCurveAngle);
        road.transform.rotation = Quaternion.Euler(0f, curveAngle, 0f);

        spawnZ += roadLength; // ��������λ��
        roads.Enqueue(road); // �������ɵĵ�·����ӵ�������

        // ɾ����������Զ�ĵ�·�Σ������ɼ��Ĳ��֣�
        if (roads.Count > numberOfRoads)
        {
            GameObject oldRoad = roads.Dequeue();
            Destroy(oldRoad);
        }
    }
}

