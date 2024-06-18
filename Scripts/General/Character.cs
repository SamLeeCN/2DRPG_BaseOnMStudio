using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class Character : MonoBehaviour,ISaveable
{
    [Header("Reference")]
    public Animator animator;
    public Rigidbody2D rb;
    public PlayerCharacterController characterController;
    public Transform weaponSlot;
    public Transform armorSlot;
    
    [Header("Properties")]
    public int maxHealth;
    public int currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;
    public float defence;
    public float slidePowerCost;
    public float invulnerableDuration;

    [Header("Assistant")]
    public float invulnerableCounter;

    [Header("Status")]
    public bool isDead;
    public bool isInvulnerable;
    public bool isPlayer;
    
    [Header("Event Broadcast")]
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent<Character> OnHealthChange;
    public UnityEvent OnDie;
    [Header("Event Listen")]
    public VoidEventSO backToMenuEvent;
    public VoidEventSO newGameEvent;
    public VoidEventSO afterSceneLoadEvent;
    
    void Start()
    {
        
        currentHealth = maxHealth;
        if (isPlayer)
        {
            currentPower = maxPower;
        }
        OnHealthChange?.Invoke(this);

        #region Initialize Status
        isDead = false;
        isInvulnerable = false;
        invulnerableCounter = invulnerableDuration;
        
        #endregion

        #region Initialize Preference
        rb= GetComponent<Rigidbody2D>();
        animator=GetComponent<Animator>();
        characterController = GetComponent<PlayerCharacterController>();
        #endregion
    }

    void Update()
    {
        if (isPlayer && currentPower <= maxPower)
        {
            currentPower =Mathf.Clamp(currentPower+Time.deltaTime*powerRecoverSpeed,0f,maxPower);
        }
        
    }
    private void OnEnable()
    {
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;
        newGameEvent.OnEventRaised += OnNewGameEvent;
        afterSceneLoadEvent.OnEventRaised += OnAfterSceneLoadEvent;
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }
    private void OnDisable()
    {
        backToMenuEvent.OnEventRaised -= OnBackToMenuEvent;
        newGameEvent.OnEventRaised -= OnNewGameEvent;
        afterSceneLoadEvent.OnEventRaised -= OnAfterSceneLoadEvent;
        ISaveable saveable = this;
        saveable.UnregisterSaveData();
    }
    private void FixedUpdate()
    {
        if (isInvulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerableCounter = invulnerableDuration;
                isInvulnerable = false;
            }
        }
    }
    public void TakeDamage(int damage,Attack attacker)
    {
        if (isInvulnerable || isDead)
        {
            return;
        }
        int resultDamage=(int)Mathf.Clamp(damage-defence, 1, Mathf.Infinity);
        if (resultDamage > currentHealth)
        {
            currentHealth = 0;
            isDead = true;
            TriggerDie();
        }
        else
        {
            currentHealth -= resultDamage;
            TriggerInvulnerable();
        }
        if (isDead)
        {
            return;
        }
        OnTakeDamage?.Invoke(attacker.transform);
        OnHealthChange?.Invoke(this);
    }

    public void TriggerDie() { 
        isDead = true;
        OnDie?.Invoke();
        OnHealthChange?.Invoke(this);
        characterController?.SetInputControllerStatus(false);
    }
    public void TriggerInvulnerable()
    {
        if (!isInvulnerable)
        {
            isInvulnerable = true;
        }
    }
    public void CostPower(float cost) {
        currentPower = Mathf.Clamp(currentPower -cost, 0f, maxPower);
    }
    
    #region save&load


    private void OnAfterSceneLoadEvent()
    {
        OnHealthChange.Invoke(this);
    }

    private void OnNewGameEvent()
    {
        currentHealth = maxHealth;
        currentPower=maxPower;
    }

    private void OnBackToMenuEvent()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }
    /// <summary>
    /// Write data in the parameter "data"
    /// </summary>
    /// <param name="data"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void SaveData(Data data)
    {
        //save current position
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            data.characterPosDict[GetDataID().ID] = new SerializeVector3(transform.position);
            data.intSaveData[GetDataID().ID + "health"] = currentHealth;
            data.floatSaveData[GetDataID().ID+ "power"]=currentPower;
        }
        else
        {
            data.characterPosDict.Add(GetDataID().ID, new SerializeVector3(transform.position));
            data.intSaveData.Add(GetDataID().ID + "health", currentHealth);
            data.floatSaveData.Add(GetDataID().ID + "power", currentPower);
        }
    }
    
    public void LoadData(Data data)
    {
        //load position
        if(data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            transform.position = data.characterPosDict[GetDataID().ID].ToVector3();
            currentHealth = data.intSaveData[GetDataID().ID + "health"];
            currentPower = data.floatSaveData[GetDataID().ID + "power"];
            OnHealthChange.Invoke(this);//update health in UI
        }
    }
    #endregion
}
