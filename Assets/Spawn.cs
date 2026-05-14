using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject prefab;      // assign in Inspector
    public Vector3 offset;         // spawn offset from this object

    bool Once = false;
    public bool DELAY;

    private void Start()
    {
        if ( !Once && DELAY)
        {
                    Vector3 spawnPos = transform.TransformPoint(offset);
        Instantiate(prefab, spawnPos, transform.rotation);
            Once = true;
        }

    }
    public void SpawnIn()
    {
        if (!Once)
        {
            Vector3 spawnPos = transform.TransformPoint(offset);
            Instantiate(prefab, spawnPos, transform.rotation);
            Once = true;
        }
    }
}