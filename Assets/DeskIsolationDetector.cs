using UnityEngine;

public class DeskIsolationDetector : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float checkDistance = 1.0f;
    public LayerMask deskMask;

    [Header("Debug")]
    public bool isIsolated = false;

    public GameObject Desk;
    public GameObject EPOS;

    void Update()
    {
        bool leftHit = Physics.Raycast(transform.position, -transform.right, checkDistance, deskMask);
        bool rightHit = Physics.Raycast(transform.position, transform.right, checkDistance, deskMask);

        // If neither side detects another desk isolated
        isIsolated = !(leftHit || rightHit);

        if (isIsolated)
        {
            EPOS.SetActive(true);
            Desk.SetActive(false);
            Debug.Log("Desk is isolated.");
        }   
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * checkDistance);
        Gizmos.DrawLine(transform.position, transform.position - transform.right * checkDistance);
    }
}
