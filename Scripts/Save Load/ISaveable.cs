using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public interface ISaveable
{
    DataDefination GetDataID();

    void RegisterSaveData() => DataManager.instance.RegisterSaveData(this);
    void UnregisterSaveData() => DataManager.instance.UnregisterSaveData(this);
    void SaveData(Data data);
    void LoadData(Data data);
}
