using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class Boar : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new BoarPatrolState();
        chaseState= new BoarChaseState();
    }
    protected override void OnEnable()
    {

        base.OnEnable();
    }
    
}
