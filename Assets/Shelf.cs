using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shelf : MonoBehaviour
{
    [Header("Prefabs to choose from")]
    public List<GameObject> prefabs;

    [Header("Spawn scripts on this shelf")]
    public List<Spawn> spawners;

    private int chosenIndex;

    private void Awake()
    {
        // 1. Pick a random prefab index
        chosenIndex = Random.Range(0, prefabs.Count);

        // 2. Assign that prefab to every spawn script
        GameObject selectedPrefab = prefabs[chosenIndex];

        foreach (var spawn in spawners)
        {
            spawn.prefab = selectedPrefab;
        }
    }
}
