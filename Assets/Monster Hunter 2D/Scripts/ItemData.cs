using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public int quantity;
}

public enum ItemType { Potion, Antidote, Whetstone }
