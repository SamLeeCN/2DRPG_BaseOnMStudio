using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(menuName ="Attack")]
public class AttackDataSO : ScriptableObject
{
    [Header("Value")]
    public int damageValue;
    public float attackRange;
    public float attackRate;
    public float fightBack;
    public float criticalMultiplier;
    public float criticalChance;
    [Header("Settings")]
    public bool haveFightBack;
    public void ApplyWeaponData(AttackDataSO weapon)
    {

    }
}
