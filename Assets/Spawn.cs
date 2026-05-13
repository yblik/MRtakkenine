using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject prefab;      // assign in Inspector
    public Vector3 offset;         // spawn offset from this object

    private void Awake()
    {
        Vector3 spawnPos = transform.position + offset;
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
