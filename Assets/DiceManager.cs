using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DiceManager : MonoBehaviour
{
    [SerializeField] public GameObject PlayerBattlefieldDiceHolder;
    [SerializeField] bool _AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES;
    [SerializeField] public List<DiceRollScript> Dices;
    [SerializeField] bool _setDicesOff;
    [SerializeField] int _numberOfDicesOnBattlefield;
    [SerializeField] Dictionary<int,string> DicesOnBattlefield;
    void Start()
    {
        if(PlayerBattlefieldDiceHolder == null)
        {        
            if (this.transform.parent.name == "Player1")
            {
                PlayerBattlefieldDiceHolder = GameObject.Find("Battlefield").transform.Find("Player1Dices").gameObject;
            }
            if (this.transform.parent.name == "Player2")
            {
                PlayerBattlefieldDiceHolder = GameObject.Find("Battlefield").transform.Find("Player2Dices").gameObject;
            }
        }

        _AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES = false;
    }
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
            if (value == true)
            {
                foreach (var dice in Dices)
                {
                    dice.IsAbleToPickup = false;
                }
                _setDicesOff = false;
            }
        }
    }
    public int NumberOfDicesOnBattlefield
    {
        get => _numberOfDicesOnBattlefield;
        set => _numberOfDicesOnBattlefield = value;
    }
    public void OnClick_ROLLDICES()
    {
        var whosThisDice = "";
        foreach (DiceRollScript dice in Dices)
        {
            dice.StartRolling();
            whosThisDice = dice.DiceOwner;
        }

        StartCoroutine(ShowInfoWhenRollingIsCompleteSuccesfully());
    }



    IEnumerator ShowInfoWhenRollingIsCompleteSuccesfully()
    {
        yield return new WaitUntil(()=>CheckIfPlayerFinishRolling());
    }

    private bool CheckIfPlayerFinishRolling()
    {
        // print("sprawdzanie czy wszystkie sie zrollowały");
        if( Dices.Where(d=>d.RollingIsCompleted == false).Any() ) return false;
        return true;
    }
}
