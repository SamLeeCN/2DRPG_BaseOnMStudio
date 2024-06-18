using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public enum EnemyStateEnum
{
    Patrol,Chase,Skill
}
public enum SceneType
{
    Location,Menu,Inventory
}
public enum PersistentType
{ 
    ReadWrite,DoNotPersist
}
public enum ItemType
{
    Consumeable,Weapon,Armor
}