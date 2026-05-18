using UnityEngine;

public class HandsSensor : MonoBehaviour
{
    [Header("Hand Sensor Settings")]
    public bool Hand; // right = true
    public string HandName;
    public string HandDetail;

    public NotificationManager NM;

    [Header("Desk Isolation Settings")]
    public float checkDistance = 8;
    public LayerMask deskMask;
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

        if (!isIsolated)
        {

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Itemns"))
        {
            ProductData pd = other.GetComponent<ProductData>();
            if (pd != null)
            {
                HandDetail = HandName + "\n"
                           + "ID: " + pd.ID + "\n"
                           + "Name: " + pd.Name + "\n"
                           + "Type: " + pd.Type;

                NM.DisplayHand(HandDetail, Hand);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Itemns"))
        {
            NM.NoDisplayHand(Hand);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * checkDistance);
        Gizmos.DrawLine(transform.position, transform.position - transform.right * checkDistance);
    }
}