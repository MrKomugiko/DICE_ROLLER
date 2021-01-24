﻿using System.Diagnostics;
using System.Linq;
using DiceRoller_Console;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GodsManager : MonoBehaviour
{
    [SerializeField] Text _currentGoldText;
    [SerializeField] public string ownerName {get; private set;}
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

    public static List<string> Logs { get => logs; set => logs = value; }
    public static int LoggerLineCounter { get => loggerLineCounter; set => loggerLineCounter = value; }

    void Awake()
    { 
        ownerName = transform.parent.gameObject.name;
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

        public string playerColorLog => ownerName == "Player1"?"green":"red";

    [ContextMenu("Test delegted skill = execute selected skill")]
    public void TESTSELEGATEDSKILL()
    {
        if(_godCardsInContainer.Where(g=>g._skill.SkillIsSelected == true).FirstOrDefault() == null) 
        {
            GodsManager.AndroidDebug("Skill is not selected / skill already used",playerColorLog);
        }
        foreach (var myGod in _godCardsInContainer.Where(g=>g._skill.SkillIsSelected == true))
        {
            GodsManager.AndroidDebug("Now execute selected skill",playerColorLog);
            GodsManager.AndroidDebug("God : "+myGod._godData.Name,playerColorLog);
            var lastUsedSkill = myGod._skill;
            lastUsedSkill.LastSelectedSkillReadyToUse();
        }
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






































    [SerializeField] static int loggerLineCounter = 0;
    [SerializeField] static List<string> logs = new List<string>();
    
    public static void AndroidDebug(string newLog, string color = "white")
    {
        // customowy debugger zeby na andku widzieć konsole :D
        // 16 lini maksymalnie, potem usuwa sie najstarsza wiadomosc
        string message = "";
        LoggerLineCounter ++;
        
        if(LoggerLineCounter == 15)
        {
            Logs.RemoveAt(0);
            LoggerLineCounter = 14;
        } 
    
        Logs.Add($"<color=\"{color}\">"+newLog+"</color>");

        foreach(string log in Logs)
        {
            message += log + "\n";
        }
        print("MESSAGE: "+message);
        GameObject.Find("ANDROIDLOGGER_TMP").GetComponent<TextMeshProUGUI>().SetText(message);
    }
}
