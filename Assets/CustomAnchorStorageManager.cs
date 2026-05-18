using System;
using Meta.XR.BuildingBlocks;
using UnityEngine;

public class CustomAnchorStorageManager : MonoBehaviour
{
    public SpatialAnchorCoreBuildingBlock _spatialAnchorCore;
    private const string NumUuidsPlayerPref = "numUuids";

    // Set this before calling SpawnSpatialAnchor so we know
    // which prefab index to associate with the next created anchor
    public int NextPrefabIndex { get; set; } = 0;

    private void Start()
    {
        //_spatialAnchorCore = FindAnyObjectByType<SpatialAnchorCoreBuildingBlock>();
        _spatialAnchorCore.OnAnchorCreateCompleted.AddListener(OnAnchorCreated);
    }

    private void OnAnchorCreated(OVRSpatialAnchor anchor, OVRSpatialAnchor.OperationResult result)
    {
        if (result != OVRSpatialAnchor.OperationResult.Success)
            return;

        // Save UUID
        if (!PlayerPrefs.HasKey(NumUuidsPlayerPref))
            PlayerPrefs.SetInt(NumUuidsPlayerPref, 0);

        int count = PlayerPrefs.GetInt(NumUuidsPlayerPref);
        PlayerPrefs.SetString("uuid" + count, anchor.Uuid.ToString());
        PlayerPrefs.SetInt(NumUuidsPlayerPref, ++count);

        // Save prefab index linked to this UUID
        PlayerPrefs.SetInt("prefab_" + anchor.Uuid.ToString(), NextPrefabIndex);

        PlayerPrefs.Save();

        Debug.Log($"Saved anchor {anchor.Uuid} with prefab index {NextPrefabIndex}");
    }

    private void OnDestroy()
    {
        _spatialAnchorCore.OnAnchorCreateCompleted.RemoveListener(OnAnchorCreated);
    }
}