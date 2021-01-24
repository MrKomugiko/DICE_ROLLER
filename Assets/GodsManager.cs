using System.Diagnostics;
using System.Linq;
using DiceRoller_Console;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GodsManager : MonoBehaviour
{
    GameManager GM_Script;
    [SerializeField] Text _currentGoldText;
    [SerializeField] public string ownerName { get; private set; }
    [SerializeField] List<God> _listOfAvailableGodsTotems;
    [SerializeField] List<GodScript> _godCardsInContainer;
    [SerializeField] List<CardScript> _listOfAllCards;
    [SerializeField] private int _amountOfGoldDeponedForSkills;

    public int CurrentGold { get => Convert.ToInt32(_currentGoldText.text); }
    int AmountOfGoldDeponedForSkills { get => _amountOfGoldDeponedForSkills; set => _amountOfGoldDeponedForSkills = value; }

    public List<CardScript> ListOfAllCards
    {
        get => _listOfAllCards;
        set
        {
            _listOfAllCards = value;
            print("liczba kart: " + value.Count);
        }
    }

    void Awake()
    {   
        GM_Script = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        foreach (GodScript godCard in _godCardsInContainer)
        {
            godCard.SelfConfigure(godTotems[randomGodsTokenIndexes[index]]);

            index++;
        }
    }
    [ContextMenu("Test delegted skill = execute selected skill")]
    public void ExecuteSelectedGodSkill()
    {
        string playerColorLog = ownerName == "Player1" ? "green" : "red";

        if (!AnySkillInOwnedCardsIsSelected)
        {
            AndroidLogger.Log("Skill is not selected / skill already used", playerColorLog);
        }
        else
        {
            foreach (var myGod in _godCardsInContainer.Where(g => g._skill.SkillIsSelected == true))
            {
                var lastUsedSkill = myGod._skill;
                lastUsedSkill.LastSelectedSkillReadyToUse();
                AndroidLogger.Log("Test?");
   
                GM_Script.CumulativeGoldStealingCounterP1 = 0;
                GM_Script.CumulativeGoldStealingCounterP2 = 0;
            }
        }
    }
    
    private bool AnySkillInOwnedCardsIsSelected { get => _godCardsInContainer.Where(g => g._skill.SkillIsSelected == true).FirstOrDefault() != null; }
    private List<int> GenerateThreeDifferentRandomNumbers(int maxValue)
    {
        List<int> randomNumbers = new List<int>();

        do
        {
            int number = RandomNumberGenerator.NumberBetween(0, maxValue - 1);
            if (!randomNumbers.Contains(number))
            {
                randomNumbers.Add(number);
            }
        } while (randomNumbers.Count < 3);

        return randomNumbers;
    }
}
