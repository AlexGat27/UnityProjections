using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotholeSpawn : MonoBehaviour
{
    public Transform[] PotholePrefabs;
    public int PotholesCount;
    public Transform SpawnPlane;
    public Vector2 MinPotholeSize, MaxPotholeSize;

    private List<Transform> _potholes = new List<Transform>();

    private void Start()
    {
        SpawnPotholes(PotholesCount);
    }

    public void SpawnPotholes(int potholesCount)
    {
        for (int i = 0; i < potholesCount; i++)
        {
            SpawnPothole();
        }
    }

    public void ClearPotholes()
    {
        for (int i = _potholes.Count - 1; i >= 0; i--)
        {
            Destroy(_potholes[i].gameObject);
        }
        _potholes.Clear();
    }

    private void SpawnPothole(Transform potholePrefab = null)
    {
        Transform prefab = null;
        if (potholePrefab == null)
        {
            prefab = PotholePrefabs[Random.Range(0, PotholePrefabs.Length)];
        }
        else
        {
            prefab = potholePrefab;
        }

        Vector2 potholeSize = new Vector2(Random.Range(MinPotholeSize.x, MaxPotholeSize.x), Random.Range(MinPotholeSize.y, MaxPotholeSize.y));

        Transform pothole = Instantiate(prefab);

        pothole.localScale = new Vector3(potholeSize.x, 1, potholeSize.y);
        pothole.position = new Vector3(Random.Range(SpawnPlane.position.x - SpawnPlane.localScale.x / 2 + pothole.localScale.x / 2,
            SpawnPlane.position.x + SpawnPlane.localScale.x / 2 - pothole.localScale.x / 2),
            0,
            Random.Range(SpawnPlane.position.z - SpawnPlane.localScale.z / 2 + pothole.localScale.z / 2,
            SpawnPlane.position.z + SpawnPlane.localScale.z / 2 - pothole.localScale.z / 2));
        pothole.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        _potholes.Add(pothole);
    }
}
