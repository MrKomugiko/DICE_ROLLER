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
    public List<GodScript> _godCardsInContainer;
    public List<SelectionController> _selectionControllers;
    [SerializeField] Text _currentGoldText;
    [SerializeField] public string ownerName { get; private set; }
    [SerializeField] List<God> _listOfAvailableGodsTotems;
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
        _selectionControllers = GetComponentsInChildren<SelectionController>().ToList();
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
                if(CheckIfSkillCanBeUsed(myGod._skill,myGod._skill.selectedSkillLevel))
                {
                    PayGoldForSkill(myGod, myGod.ownerName);
                    lastUsedSkill.LastSelectedSkillReadyToUse();
                }
                else
                {
                    AndroidLogger.Log("you dont have enought Gold to cast skill",playerColorLog);
                    lastUsedSkill.SkillIsSelected = false;
                }
            }
        }
    }

    

    private void PayGoldForSkill(GodScript myGod, string godOwner)
    {
        var p1coin = GameObject.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>();
        var p2coin = GameObject.Find("CoinTextPlayer2").GetComponent<TextMeshProUGUI>();
        switch (godOwner)
        {
            case "Player1":
                for (int i = 0; i < myGod._skill.GetGoldCostForSkillLevel(myGod._skill.selectedSkillLevel); i++)
                {
                    GM_Script.TemporaryGoldVault_player1--;
                    p1coin.color = Color.red;
                }
                break;

            case "Player2":
                for (int i = 0; i < myGod._skill.GetGoldCostForSkillLevel(myGod._skill.selectedSkillLevel); i++)
                {
                    GM_Script.TemporaryGoldVault_player2--;
                    p2coin.color = Color.red;
                }
                break;
        }

        GM_Script.CumulativeGoldStealingCounterP1 = 0;
        GM_Script.CumulativeGoldStealingCounterP2 = 0;
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
    public bool CheckIfSkillCanBeUsed(Skill skill, int level)
    {
        if (skill.GetGoldCostForSkillLevel(level) > CurrentGold ) return false;

        return true;
    }

    public void BlockGodButtonsIfCombatStarted()
    {

    }
}
