using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Newtonsoft.Json;
using System.IO;
using System;

[DefaultExecutionOrder(-100)]//to enable DataManager before any isaveble
public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    private List<ISaveable> saveableList = new List<ISaveable>();
    private Data saveData; 
    private string jsonFolder;
    [Header("Event Listen")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadDataEvent;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        saveData=new Data();
        jsonFolder = Application.persistentDataPath+"/SAVE DATA/";
        ReadSavedData();
    }
    private void OnEnable()
    {
        saveDataEvent.OnEventRaised += Save;
        loadDataEvent.OnEventRaised += Load;
    }
    private void OnDisable()
    {
        saveDataEvent.OnEventRaised -= Save;
        loadDataEvent.OnEventRaised -= Load;
    }
    public void RegisterSaveData(ISaveable saveable)
    {
        if (!saveableList.Contains(saveable))
        {
            saveableList.Add(saveable);
        }
    }
    public void UnregisterSaveData(ISaveable saveable)
    {
        saveableList.Remove(saveable);
    }

    public void Save()
    {
        foreach(var saveable in saveableList)
        {
            saveable.SaveData(saveData);
        }
        String savePath = jsonFolder + "data.sav";
        String dataJson=JsonConvert.SerializeObject(saveData);
        if (!File.Exists(savePath))
        {
            Directory.CreateDirectory(jsonFolder);
        }
        File.WriteAllText(savePath, dataJson);
    }

    public void Load()
    {
        foreach (var saveable in saveableList)
        {
            saveable.LoadData(saveData);
        }
    }
    private void ReadSavedData()
    {
        var savePath=jsonFolder+"data.sav";
        if (File.Exists(savePath))
        {
            String dataStr=File.ReadAllText(savePath);
            saveData = JsonConvert.DeserializeObject<Data>(dataStr);
        }
    }

    private void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            Load();
        }
        if (Keyboard.current.sKey.wasPressedThisFrame && Keyboard.current.ctrlKey.wasPressedThisFrame)
        {
            Save();
        }
    }
}
