using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefab;
    public float spawnTime = 1;
    private float timer = 0;
    void Update()
    {
        if (timer > spawnTime)
        {
            int rand = Random.Range(0, obstaclePrefab.Length);
            GameObject obs = Instantiate(obstaclePrefab[rand]);
            obs.transform.position = transform.position + new Vector3(0, 0, 25);
            Destroy(obs, 15);
            timer = 0;
        }
        timer += Time.deltaTime;
    }
}
