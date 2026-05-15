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
    public GameObject GhostSkull;

    private void Update()
    {
        ItemTotalTXT.text = "Items: " + ItemCount.ToString() + " Total: £" + PriceTotal.ToString();
    }

    public void Buy() //play buy sound 
    {
        ItemCount = 0;
        LastPrice = 0;
    }
    public void AddItem(string Name, int Price)
    {
        LastItem.text = "Last item: " + Name + " £" + Price;
        LastPrice = Price;
        ItemCount++;
        PriceTotal += Price;
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
        if (ItemID == 4)
        {
            GhostSkull.SetActive(true);
        }
        Anim.Play("AddItemAnim");


    }
    public void ClearGHost()
    {
        GhostPotion.SetActive(false);
        GhostSkull.SetActive(false);
    }
}
