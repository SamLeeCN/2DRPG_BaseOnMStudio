using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(menuName ="Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    public UnityAction<Character> OnEventRaised;
    public void RaiseEvent(Character character)
    {
        OnEventRaised?.Invoke(character);
    }
}
