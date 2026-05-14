using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject prefab;
    public Vector3 offset;

    bool Once = false;

    private void Start()
    {
        if ( !Once)
        {
                    Vector3 spawnPos = transform.TransformPoint(offset);
        Instantiate(prefab, spawnPos, transform.rotation);
            Once = true;
        }

    }
}