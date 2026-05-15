using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandButton : MonoBehaviour
{
    public EPOSsystem EPOS;
    public bool Buy;

    public AudioSource Audio;
    public void OnTriggerEnter(Collider other) //if hand anyways
    {
        if (!Buy)
        {
            EPOS.RemoveOne();
            Audio.Play();
        }
        else
        {
            EPOS.Buy();
            Audio.Play();
        }
    }
}
