using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPainting : MonoBehaviour
{
    public GameObject P1;
    public GameObject P2;
    public GameObject P3;
    public GameObject P4;

    void Awake()
    {
        RandomPaint();
    }
    public void RandomPaint()
    {
        int random = Random.Range(1, 5);
        if (random == 1)
        {
            P1.SetActive(true);
        }
        else if (random == 2)
        {
            P2.SetActive(true);
        }
        else if (random == 3)
        {
            P3.SetActive(true);
        }
        else if (random == 4)
        {
            P4.SetActive(true);
        }
    }
}
