using System;
using System.Collections.Generic;
using UnityEngine;

public class AnchorLoader : MonoBehaviour
{
    public GameObject prefabToSpawn;

    async void Start()
    {
        // Wait for Quest tracking to initialize
        await System.Threading.Tasks.Task.Delay(2000);

        await LoadAnchors();
    }

    public async System.Threading.Tasks.Task LoadAnchors()
    {
        List<Guid> anchorGuids = GetSavedAnchorGuids();

        if (anchorGuids.Count == 0)
        {
            Debug.Log("No saved anchors found.");
            return;
        }

        List<OVRSpatialAnchor.UnboundAnchor> unboundAnchors =
            new List<OVRSpatialAnchor.UnboundAnchor>();

        var result = await OVRSpatialAnchor.LoadUnboundAnchorsAsync(
            anchorGuids,
            unboundAnchors
        );

        if (!result.Success)
        {
            Debug.LogWarning("Failed to load anchors.");
            return;
        }

        Debug.Log($"Loaded {unboundAnchors.Count} anchors.");

        foreach (var unbound in unboundAnchors)
        {
            // Localize anchor
            if (!unbound.Localized)
            {
                bool localized = await unbound.LocalizeAsync();

                if (!localized)
                {
                    Debug.LogWarning($"Failed to localize {unbound.Uuid}");
                    continue;
                }
            }

            // Temporary anchor object just to get position
            GameObject temp = new GameObject("TempAnchor");

            OVRSpatialAnchor anchor =
                temp.AddComponent<OVRSpatialAnchor>();

            unbound.BindTo(anchor);

            // Wait for transform update
            await System.Threading.Tasks.Task.Yield();
            await System.Threading.Tasks.Task.Yield();

            Vector3 position = anchor.transform.position;

            Destroy(temp);

            // Spawn prefab with ZERO rotation
            Instantiate(
                prefabToSpawn,
                position,
                Quaternion.identity
            );

            Debug.Log($"Spawned prefab at {position}");
        }
    }

    private List<Guid> GetSavedAnchorGuids()
    {
        List<Guid> guids = new();

        int count = PlayerPrefs.GetInt("numUuids", 0);

        for (int i = 0; i < count; i++)
        {
            string key = "uuid" + i;

            if (PlayerPrefs.HasKey(key))
            {
                string uuidString = PlayerPrefs.GetString(key);

                if (Guid.TryParse(uuidString, out Guid guid))
                {
                    guids.Add(guid);
                }
            }
        }

        return guids;
    }
}