using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class Attack : MonoBehaviour
{
    [Header("Basic Value")]
    public int basicDamageValue;
    public float basicAttackRange;
    public float basicAttackRate;
    public float basicFightBack;
    [Header("Value")]
    public int damageValue;
    public float attackRange;
    public float attackRate;
    public float fightBack;
    [Header("Settings")]
    public bool haveFightBack;


    void Start()
    {
        
    }

    void Update()
    {

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.GetComponent<Character>()?.TakeDamage(damageValue, this);  
    }
}
