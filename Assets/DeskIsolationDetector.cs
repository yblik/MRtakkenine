using UnityEngine;

public class DeskIsolationDetector : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float checkDistance = 8;
    public LayerMask deskMask;

    [Header("Debug")]
    public bool isIsolated = false;

    public GameObject Desk;
    public GameObject EPOS;

    public Spawn shelfSpawn;

    void Update()
    {

        float checkRadius = 0.2f;

        Vector3 leftPos = transform.position - transform.right;
        Vector3 rightPos = transform.position + transform.right;

        bool leftHit = Physics.CheckSphere(leftPos, checkRadius, deskMask);
        bool rightHit = Physics.CheckSphere(rightPos, checkRadius, deskMask);

        isIsolated = !leftHit && !rightHit;


        if (isIsolated)
        {
            EPOS.SetActive(true);
            Desk.SetActive(false);
            Debug.Log("Desk is isolated.");
        }
        else
        {
            shelfSpawn.SpawnIn();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * checkDistance);
        Gizmos.DrawLine(transform.position, transform.position - transform.right * checkDistance);
    }
}
