using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public AudioSource AS;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Itemns")
        {
            AS.Play();
        }
    }
}
