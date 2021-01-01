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
    [SerializeField] public List<String> DicesOnBattleground_Names;
    [SerializeField] private List<Image> _dicesOnBattleground_Images;
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
        ReplaceDicesOnBattleground(SortDicesByType());
    }

    void ReplaceDicesOnBattleground(Dictionary<int, string> sortedDicesDict){
        foreach (KeyValuePair<int, string> dice in sortedDicesDict)
        {
            print(dice.Key + " " + dice.Value);
            this.transform.GetChild(dice.Key).GetComponent<DiceRollScript>().DiceImage.sprite = listOfEveryDiceFaces.Where(p=>p.name == dice.Value).First();
        }
    }

    Dictionary<int, string> SortDicesByType()
    {
        // TODO: OGANAC KOLEJNOŚĆ KTO PIERWSZY ATAKUJE A KTO MA DEFA NA POCZATKU?
        /*
            1,2     Axe     Mele Attack
            3       Hand    Steal
            4       Bow     Ranged Attack
            5       Sheield Ranged Deffence
            6       Helmet  Mele Deffence 
        */

        int diceIndex = 0;
        Dictionary<int, string> SortedDict = new Dictionary<int, string>();

        foreach (Image diceImage in DicesOnBattleground)
        {
            // MELE ATTACK
            if (diceImage.sprite.name.ElementAt(0).ToString() == "1")
            {
                SortedDict.Add(diceIndex, diceImage.sprite.name);
                diceIndex++;
            }
        }

        foreach (Image diceImage in DicesOnBattleground)
        {
            // MELE ATTACK
            if (diceImage.sprite.name.ElementAt(0).ToString() == "2")
            {
                SortedDict.Add(diceIndex, diceImage.sprite.name);
                diceIndex++;
            }
        }

        foreach (Image diceImage in DicesOnBattleground)
        {
            // MELE DEFFENCE
            if (diceImage.sprite.name.ElementAt(0).ToString() == "6")
            {
                SortedDict.Add(diceIndex, diceImage.sprite.name);
                diceIndex++;
            }
        }

        foreach (Image diceImage in DicesOnBattleground)
        {
            // RANGED ATTACK
            if (diceImage.sprite.name.ElementAt(0).ToString() == "4")
            {
                SortedDict.Add(diceIndex, diceImage.sprite.name);
                diceIndex++;
            }
        }

        foreach (Image diceImage in DicesOnBattleground)
        {
            // RANGED DEFFENCE
            if (diceImage.sprite.name.ElementAt(0).ToString() == "5")
            {
                SortedDict.Add(diceIndex, diceImage.sprite.name);
                diceIndex++;
            }
        }

        foreach (Image diceImage in DicesOnBattleground)
        {
            // STEAL
            if (diceImage.sprite.name.ElementAt(0).ToString() == "3")
            {
                SortedDict.Add(diceIndex, diceImage.sprite.name);
                diceIndex++;
            }
        }

        return SortedDict;
    }
}
