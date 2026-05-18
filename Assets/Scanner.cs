using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public AudioSource AS;
    public EPOSsystem  EPOSsystem;
    private ProductData currentPD;
    public Animator Anim;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Itemns")
        {
            AS.Play();
                 currentPD = other.GetComponentInParent<ProductData>();

            EPOSsystem.AddedToCart(currentPD.ID);
            EPOSsystem.AddItem(currentPD.Name, currentPD.Price);
            Anim.Play("Scanner");
            FindObjectOfType<NotificationManager>().AddToCartNotif(currentPD.name);
            Destroy(other.gameObject);


        }
    }
    public void ItemDisplayOver() //call in animation with things being disabled beforehand via parenting
    {
        EPOSsystem.AddItem(currentPD.Name, currentPD.Price);
    }
}
