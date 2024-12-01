using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRoad : MonoBehaviour
{
    public GameObject roadPrefab; // 道路段的预制体
    public int numberOfRoads = 5; // 初始化时道路段的数量
    public float roadLength = 50f; // 每个道路段的长度
    public Transform player; // 玩家对象
    public float maxHeightChange = 5f; // 随机高度变化范围
    public float maxWidthChange = 2f; // 随机宽度变化范围
    public float maxCurveAngle = 30f; // 随机弯曲角度范围

    private Queue<GameObject> roads; // 存储当前道路段的队列
    private float spawnZ = 0f; // 下一个道路段生成的位置

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
        // 判断玩家是否已接近最后一个道路段
        if (player.position.z > spawnZ - numberOfRoads * roadLength)
        {
            SpawnRoad();
        }
    }

    // 生成新的道路段
    void SpawnRoad()
    {
        GameObject road = Instantiate(roadPrefab);
        road.transform.position = new Vector3(0, Random.Range(-maxHeightChange, maxHeightChange), spawnZ);

        // 随机调整道路段的宽度
        road.transform.localScale = new Vector3(Random.Range(1f, 1f + maxWidthChange), 1f, 1f);

        // 随机旋转道路段以实现弯曲效果
        float curveAngle = Random.Range(-maxCurveAngle, maxCurveAngle);
        road.transform.rotation = Quaternion.Euler(0f, curveAngle, 0f);

        spawnZ += roadLength; // 更新生成位置
        roads.Enqueue(road); // 将新生成的道路段添加到队列中

        // 删除队列中最远的道路段（即不可见的部分）
        if (roads.Count > numberOfRoads)
        {
            GameObject oldRoad = roads.Dequeue();
            Destroy(oldRoad);
        }
    }
}

