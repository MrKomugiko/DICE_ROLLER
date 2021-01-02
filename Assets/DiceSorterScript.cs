using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceSorterScript : MonoBehaviour
{
    [SerializeField] private List<Sprite> listOfEveryDiceFaces;
    public List<Image> DicesOnBattleground;
    [SerializeField] private bool needToSort;
    [SerializeField] bool diceSorted;

    public bool NeedToSort
    {
        get => needToSort;
        set
        {
            if (value == true)
            {
                GetDicesFromBattleground();
            }
            needToSort = false;
        }
    }

    void FixedUpdate()
    {
        if (this.transform.childCount == 6)
        {
            if (diceSorted == false)
            {
                NeedToSort = true;
            }
        }
    }

    void GetDicesFromBattleground()
    {
        diceSorted = true;
        foreach (var dice in this.GetComponentsInChildren<DiceRollScript>())
        {
            DicesOnBattleground.Add(dice.DiceImage);
        }
        ReplaceDicesOnBattleground(SortDicesByType(DicesOnBattleground));
    }

    void ReplaceDicesOnBattleground(Dictionary<int, string> sortedDicesDict){
        foreach (KeyValuePair<int, string> dice in sortedDicesDict)
        {
            this.transform.GetChild(dice.Key).GetComponent<DiceRollScript>().DiceImage.sprite = listOfEveryDiceFaces.Where(p=>p.name == dice.Value).FirstOrDefault();
            print("podmiana obrazka");
        }
    }

    Dictionary<int, string> SortDicesByType(List<Image> dicesOnBattleground)
    {
        Dictionary<int, string> SortedDict = new Dictionary<int, string>();

        List<int> correctOrderDiceFaces = new List<int>(){1,2,6,4,5,3};
        /*
            1,2     Axe     Mele Attack
            6       Helmet  Mele Deffence 
            4       Bow     Ranged Attack
            5       Sheield Ranged Deffence
            3       Hand    Steal
        */
        
        int diceIndex = 0;
        foreach (int diceFace in correctOrderDiceFaces)
        {
            foreach (Image diceImage in dicesOnBattleground)
            {
                // MELE ATTACK
                if (diceImage.sprite.name.ElementAt(0).ToString() == diceFace.ToString())
                {
                    SortedDict.Add(diceIndex, diceImage.sprite.name);
                    diceIndex++;
                }
            }
        }
        return SortedDict;
    }

    private void UpdateDiceObjectName(int index)
    {
       this.GetComponentInParent<DiceRollScript>().DiceImage = this.GetComponent<Image>();
    }
}
