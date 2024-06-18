using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class WaterManager : MonoBehaviour
{
    [Header("Reference")]
    public CompositeCollider2D coll;

    [Header("Parameters")]
    public float amplitude;
    public float originalHeight;
    public float length;
    public float speed;
    public float waveOffset;

    private void Awake()
    {
        coll = GetComponent<CompositeCollider2D>();
        waveOffset = 0;
        originalHeight = coll.bounds.max.y;
    }
    private void Update()
    {
        waveOffset += speed*Time.deltaTime;
        if(waveOffset>=length*2f*Mathf.PI)
        {
            waveOffset = 0f;
        }
    }
    public float GetWaveHeight(float x)
    {
        return originalHeight+amplitude*Mathf.Sin(x/length+waveOffset);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PhysicalCheck physicalCheck = collision.GetComponent<PhysicalCheck>();
        if (!physicalCheck)
        {
            return;
        }
        physicalCheck.isInWater = true;
        physicalCheck.currentWater = this;
        collision.GetComponent<Rigidbody2D>().drag = physicalCheck.waterDrag;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        PhysicalCheck physicalCheck = collision.GetComponent<PhysicalCheck>();
        if (!physicalCheck)
        {
            return;
        }
        physicalCheck.isInWater = false;
        collision.GetComponent<Rigidbody2D>().drag = physicalCheck.regularDrag;
    }
}



//private void OnTriggerStay2D(Collider2D collision)
//{
//    PhysicalCheck physicalCheck = collision.GetComponent<PhysicalCheck>();
//    physicalCheck.currentWater = this;
//}
