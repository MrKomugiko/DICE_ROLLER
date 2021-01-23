using System.Diagnostics;
using System.Linq;
using DiceRoller_Console;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodsManager : MonoBehaviour
{
    [SerializeField] Text _currentGoldText;
    [SerializeField] string _tokensOwnerName;
    [SerializeField] List<God> _listOfAvailableGodsTotems;
    [SerializeField] List<GodScript> _godCardsInContainer;
    [SerializeField] List<CardScript> _listOfAllCards;
    

    public int CurrentGold { get => Convert.ToInt32(_currentGoldText.text); }

    public List<CardScript> ListOfAllCards { 
        get => _listOfAllCards; 
        set 
        { 
            _listOfAllCards = value;
            print("liczba kart: "+value.Count); 
        }
    }
    void Awake()
    {
        _godCardsInContainer = GetComponentsInChildren<GodScript>().ToList();
    }

    void Start()
    {
        PopulateContainerWithGodTokens(_listOfAvailableGodsTotems);

        ListOfAllCards = this.GetComponentsInChildren<CardScript>().ToList();
    }

    void PopulateContainerWithGodTokens(List<God> godTotems)
    {
        List<int> randomGodsTokenIndexes = GenerateThreeDifferentRandomNumbers(godTotems.Count);
        int index = 0;
        foreach(GodScript godCard in _godCardsInContainer)
        {
            godCard.SelfConfigure(godTotems[randomGodsTokenIndexes[index]]);
            index++;
        }
        // print($"{_tokensOwnerName} | nazwy bogów dodanych do kart:[{_godCardsInContainer[0].GodObject.Name}] [{_godCardsInContainer[1].GodObject.Name}] [{_godCardsInContainer[2].GodObject.Name}] ");
    }

    private List<int> GenerateThreeDifferentRandomNumbers(int maxValue)
    {
        List<int> randomNumbers = new List<int>();
        
        do
        {
            int number = RandomNumberGenerator.NumberBetween(0, maxValue-1);
            if(!randomNumbers.Contains(number))
            {
                randomNumbers.Add(number);
            }    
        } while (randomNumbers.Count < 3);

        return randomNumbers;
    }









    public static void AndroidDebug(string text)
    {
        GameObject.Find("ANDROIDLOGGER").GetComponent<Text>().text += text+"\n";
    }
}
