using System.Linq;
using DiceRoller_Console;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GodsManager : MonoBehaviour
{
    GameManager GM_Script;
    public List<GodScript> _godCardsInContainer;
    public List<SelectionController> _selectionControllers;
    [SerializeField] Text _currentGoldText;
    [SerializeField] List<God> _listOfAvailableGodsTotems;
    [SerializeField] List<CardScript> _listOfAllCards;
    [SerializeField] private int _amountOfGoldDeponedForSkills;
    [SerializeField] public string ownerName { get; private set; }

    public int CurrentGold
    {
        get => Convert.ToInt32(_currentGoldText.text);

    }

    public List<CardScript> ListOfAllCards
    {
        get => _listOfAllCards;
        set
        {
            _listOfAllCards = value;
            print("liczba kart: " + value.Count);
        }
    }
    private bool AnySkillInOwnedCardsIsSelected
    {
        get => _godCardsInContainer.Where(g => g._skill.SkillIsSelected == true).FirstOrDefault() != null;
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
                if (Skill.CheckIfPlayerHaveEnoughtGoldToUseSkill(ownerName, myGod._skill, myGod._skill.selectedSkillLevel))
                {
                    PayGoldForSkill(myGod, myGod.ownerName);
                    lastUsedSkill.LastSelectedSkillReadyToUse();
                }
                else
                {
                    AndroidLogger.Log("you dont have enought Gold to cast skill", playerColorLog);
                    lastUsedSkill.SkillIsSelected = false;
                }
            }
        }
        CollorSkillButtonsIfCanBeUsed();
    }



    public void CollorSkillButtonsIfCanBeUsed()
    {
        foreach (var god in _godCardsInContainer)
        {
            //print("colloring for: "+ownerName);
            // wykluczenie zmiany koloru aktualnie zaznaczonego
            int ignoredButtonIndex = 0;
            if (god._skill.SkillIsSelected == true) ignoredButtonIndex = god._skill.selectedSkillLevel;

            for(int level = 1; level <= 3; level++)
            {
                if (ignoredButtonIndex != level)
                {
                    if (Skill.CheckIfPlayerHaveEnoughtGoldToUseSkill(ownerName, god._skill, level) == false)
                    {
                        ChangeSkillButtonToDissabled(god, level: level);
                    }
                    else
                    {
                        ChangeSkillButtonToEnabled(god, level: level);
                    }
                }
            }
        }
    }

    private void ChangeSkillButtonToEnabled(GodScript godScript, int level)
    {
        GameObject skillButton = null;
        switch (level)
        {
            case 1:
                skillButton = godScript.transform.Find("RewersContent").transform.Find("Skill Level 1").transform.gameObject;
                break;

            case 2:
                skillButton = godScript.transform.Find("RewersContent").transform.Find("Skill Level 2").transform.gameObject;
                break;

            case 3:
                skillButton = godScript.transform.Find("RewersContent").transform.Find("Skill Level 3").transform.gameObject;
                break;
        }
        skillButton.GetComponent<Image>().color = new Color32(255, 255, 255, 128);
        skillButton.GetComponentInChildren<Text>().color = Color.white;;
    }

    private void ChangeSkillButtonToDissabled(GodScript godScript, int level)
    {
        // print("zmiana koloru na CZERWONY"+ godScript._skill.GodName +" | "+godScript._skill.SkillName+ " | "+ level);
        GameObject skillButton = null;
        switch (level)
        {
            case 1:
                skillButton = godScript.transform.Find("RewersContent").transform.Find("Skill Level 1").transform.gameObject;
                break;

            case 2:
                skillButton = godScript.transform.Find("RewersContent").transform.Find("Skill Level 2").transform.gameObject;
                break;

            case 3:
                skillButton = godScript.transform.Find("RewersContent").transform.Find("Skill Level 3").transform.gameObject;
                break;
        }
        skillButton.GetComponent<Image>().color = new Color32(255, 0, 0, 128);
        skillButton.GetComponentInChildren<Text>().color = Color.red;
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
        CollorSkillButtonsIfCanBeUsed();
    }

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
