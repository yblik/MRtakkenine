using UnityEngine;
using Meta.XR.MRUtilityKit;
using TMPro;

public class DisplayLabel : MonoBehaviour
{
    public Transform rayStartPoint;
    public float rayStrength = 5f;

    public TextMeshPro labelText;

    void Update()
    {
        if (MRUK.Instance == null)
            return;

        MRUKRoom room = MRUK.Instance.GetCurrentRoom();

        if (room == null)
            return;

        Ray ray = new Ray(
            rayStartPoint.position,
            rayStartPoint.forward
        );

        RaycastHit hit;
        MRUKAnchor anchor;

        bool hasHit = room.Raycast(
            ray,
            rayStrength,
            out hit,
            out anchor
        );

        if (hasHit && anchor != null)
        {
            labelText.transform.position = hit.point;

            labelText.transform.rotation =
                Quaternion.LookRotation(-hit.normal);

            labelText.text = "anchor: " + anchor.AnchorLabels.ToString();
        }
    }
}