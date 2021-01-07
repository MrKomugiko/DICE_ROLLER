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
    public bool DiceSorted
    {
        get
        {
            return _diceSorted;
        }
        set
        {
            _diceSorted = value;
        }
    }

[SerializeField] private bool _diceSorted;
    public bool NeedToSort
    {
        get => needToSort;
        set
        {
            if (value == true)
            {
                GetDicesFromBattleground();
                print("sorting finished");

                   if(DiceSorted == true)
                   {
                        // for (int i = 0; i < 6; i++)
                        // {
                        // /////////////////////this.transform.GetChild(i).GetComponent<DiceActionScript>().AddGoldFromBlessedItems = true;
                        //     print(this.transform.GetChild(i).GetComponent<DiceRollScript>().transform.name + " test from need to sort");
                        // }
                    }
               // print(this.name);
               // GameObject.Find("GameManager").GetComponent<GameManager>().CollectGold(this.name);
            }
            needToSort = false;
        }
    }

    void GetDicesFromBattleground()
    {
        foreach (var dice in this.GetComponentsInChildren<DiceRollScript>().Where(d=>d.DiceNumber != 0))
        {
            DicesOnBattleground.Add(dice.DiceImage);
        }
        ReplaceDicesOnBattleground(SortDicesByType(DicesOnBattleground));
        
        //update list of dices in correct order;
        foreach(var dice in DicesOnBattleground){
//            print(this.name); 
            if (this.name == "Player1Dices")
            {
                GameManager.OnBattlefield_Dice_Player1 = DicesOnBattleground;
            }
            else
            {
                GameManager.OnBattlefield_Dice_Player2 = DicesOnBattleground;
            }

        }
    
        DiceSorted = true;
    }
    [SerializeField] public List<int> newDiceNumbersOrderedList = new List<int>();
    void ReplaceDicesOnBattleground(Dictionary<int, string> sortedDicesDict){
       // int index = 0;
        //string playerName = "";
        //if(this.name == "Player1Dices"){
         //   playerName = "Player1";
        //}else{
         //   playerName = "Player2";
       // }
        //print(playerName);
        //var usedDicesFromHand = GameObject.Find(playerName).transform.Find("DiceHolder").GetComponentsInChildren<DiceRollScript>();
        //print(usedDicesFromHand.Count());
        foreach (KeyValuePair<int, string> dice in sortedDicesDict)
        {
            this.transform.GetChild(dice.Key).GetComponent<DiceRollScript>().DiceImage.sprite = listOfEveryDiceFaces.Where(p=>p.name == dice.Value).FirstOrDefault();
            this.transform.GetChild(dice.Key).GetComponent<DiceActionScript>().AddGoldFromBlessedItems = true;
            // dopasowanie ideksów do obrazków
            //print(this.name);
            //this.transform.GetChild(dice.Key).GetComponent<DiceRollScript>().DiceNumber = usedDicesFromHand.Where(d=>d.name == this.transform.GetChild(dice.Key).GetComponent<DiceRollScript>().transform.name).First().DiceNumber;

          //  print("podmiana obrazka");
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
