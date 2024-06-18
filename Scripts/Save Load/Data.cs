using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class Data
{
    public string savedSceneJsonTxt;//save scene in json file
    public Dictionary<string,SerializeVector3> characterPosDict = new Dictionary<string,SerializeVector3>();
    public Dictionary<string,float> floatSaveData = new Dictionary<string,float>();
    public Dictionary<string,int> intSaveData= new Dictionary<string,int>();

    public void SaveSceneToData(GameSceneSO gameScene)
    {
        savedSceneJsonTxt=JsonUtility.ToJson(gameScene);
    }
    public GameSceneSO GetSavedScene()
    {
        var savedScene=ScriptableObject.CreateInstance<GameSceneSO>();
        JsonUtility.FromJsonOverwrite(savedSceneJsonTxt, savedScene);
        return savedScene;
    }
    
}
public class SerializeVector3
{
    public float x, y, z;
    public SerializeVector3(Vector3 pos)
    {
        x = pos.x;
        y = pos.y;
        z = pos.z;
    }
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
