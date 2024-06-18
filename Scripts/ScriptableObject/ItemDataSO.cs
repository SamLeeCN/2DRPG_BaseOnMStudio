using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(menuName ="Inventory/Item")]
public class ItemDataSO : ScriptableObject
{
    public int itemID;
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public bool stackable=true;
    public int maxStackAmount=64;
    public int itemAmount;
    [TextArea]
    public string itemDescription="";

    [Header("Weapon")]
    public GameObject weaponEqPrefeb;
    //GameObject show on player's body


}
