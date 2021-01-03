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
    [SerializeField] private int _numberOfDicesOnBattlefield;

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

    public int NumberOfDicesOnBattlefield { get => _numberOfDicesOnBattlefield; set => _numberOfDicesOnBattlefield = value; }

    void Start()
    {
        _AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES = false;
    }

    public void OnClick_ROLLDICES()
    {
        foreach (var dice in Dices)
        {
            dice.StartRolling();
        //    dice.IsAbleToPickup = true;
        }
    }
}
