using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EPOSsystem : MonoBehaviour
{
    public Text ItemTotalTXT;
    public Text LastItem;

    public int ItemCount;
    public int PriceTotal;
    public int LastPrice;

    public Animator Anim;

    //all items
    public GameObject  GhostPotion;
    public GameObject  GhostPouch;
    public GameObject GhostBucket;
    public GameObject GhostSkull;
    public GameObject GhostLamp;
    public GameObject GhostKey;
    public GameObject GhostVase;

    public string items;

    private void Update()
    {
        ItemTotalTXT.text = "Items: " + ItemCount.ToString() + " Total: £" + PriceTotal.ToString();
    }

    public void Buy() //play buy sound 
    {
        FindObjectOfType<NotificationManager>().PayForNotif(items);
        ItemCount = 0;
        LastPrice = 0;
        items = null;
    }
    public void AddItem(string Name, int Price)
    {
        LastItem.text = "Last item: " + Name + " £" + Price;
        LastPrice = Price;
        ItemCount++;
        PriceTotal += Price;
        items = items + "," + Name;
    }
    public void RemoveOne()
    {
        PriceTotal -= LastPrice;
        ItemCount--;
    }

    //animations need to be handled
    public void AddedToCart(int ItemID)
    {
        ClearGHost();
        if (ItemID == 1)
        {
            GhostPotion.SetActive(true);
        }
            if (ItemID == 2)
            {
                GhostPouch.SetActive(true);
            }
            if (ItemID == 3)
            {
                GhostBucket.SetActive(true);
        }
        if (ItemID == 4)
        {
            GhostSkull.SetActive(true);
        }
        if (ItemID == 5)
        {
            GhostLamp.SetActive(true);  
        }
        if (ItemID == 6)
        {
            GhostKey.SetActive(true);
        }
        if (ItemID == 7)
        {
            GhostVase.SetActive(true);
        }
        Anim.Play("AddItemAnim");


    }
    public void ClearGHost()
    {
        GhostPotion.SetActive(false);
        GhostPouch.SetActive(false);
        GhostBucket.SetActive(false);
        GhostSkull.SetActive(false);
        GhostLamp.SetActive(false);
        GhostKey.SetActive(false);
        GhostVase.SetActive(false);

    }
}
