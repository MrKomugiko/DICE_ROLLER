using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [SerializeField] public GameObject PlayerBattlefieldDiceHolder;
    [SerializeField] private bool _AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES;
    [SerializeField] private List<DiceRollScript> Dices;
    [SerializeField] bool _setDicesOff;

    public bool AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES
    {
        get => _AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES;
        set
        {
            _AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES = value;
        }
    }
    public bool SetDicesOff
    { 
        get => _setDicesOff; 
        set
        { 
            _setDicesOff = value; 
            if(value == true){
                foreach (var dice in Dices)
                {
                    dice.IsAbleToPickup = false;
                }
            _setDicesOff = false;
            }
        }
    }

    void Start()
    {
        _AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES = false;
        //  Dices.AddRange(GetComponentsInChildren<DiceRollScript>());
    }

    public void OnClick_ROLLDICES()
    {
        // TODO: before rolling - player cant pick dice to battlefield its too late for this move.
        foreach (var dice in Dices)
        {
            dice.StartRolling();
            dice.IsAbleToPickup = true;
        }
    }
}
