using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject prefab;
    public Vector3 offset;

    bool Once = false;
    public bool DELAY;

    private void Start()
    {
        if ( !Once && !DELAY)
        {
                    SpawnIn();
            Once = true;
        }

    }
    public void SpawnIn()
    {
        Vector3 spawnPos = transform.TransformPoint(offset);
        Instantiate(prefab, spawnPos, transform.rotation);
    }
}