using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductData : MonoBehaviour
{
    //for epos script - pull values from here
    public int ID;
    public int Price;
    public string Name;
    public string Type;

    public Text NameAndPriceTxt;
    public Text IDtxt;
    public Text TypeTxt;

    public void  Awake()
    {
        NameAndPriceTxt.text = Name + " - £" + Price.ToString();
        IDtxt.text = "ID: " + ID.ToString();
        TypeTxt.text = "Type: " + Type;
    }


}
