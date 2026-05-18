using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public Animator BuyAnim;
    public TextMeshProUGUI BuyText;

    public TextMeshProUGUI Lhand;
    public TextMeshProUGUI Rhand;

    //Buying notification
    public void AddToCartNotif(string Item) //paying or adding to cart?
    {
        BuyText.text = Item + "Has been added to cart!";
        BuyAnim.Play("Buying");
    }
    public void NoAddToCartNotif() //paying or adding to cart?
    {
        BuyText.text = "Nothing removed since there's nothing to remove";
        BuyAnim.Play("Buying");
    }
    public void PayForNotif(string Items)
    {
        BuyText.text = Items + " have been paid for!";
        BuyAnim.Play("Buying");
    }
    public void PayForNothhign()
    {
        BuyText.text = "Nothing has been purchased as nothings in cart";
        BuyAnim.Play("Buying");
    }
    public void DisplayHand(string Item, bool Hand) 
    {
        if (!Hand)
        {
            Lhand.gameObject.SetActive(true);
            Lhand.text = Item;
        }
        else
        {
            Rhand.gameObject.SetActive(true);
            Rhand.text = Item;
        }

    }
    public void NoDisplayHand(bool Hand)
    {
        if (!Hand)
        {
            Lhand.gameObject.SetActive(false);
        }
        else
        {
            Rhand.gameObject.SetActive(false);
        }

    }

}
