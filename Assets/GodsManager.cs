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
    [SerializeField] List<God> _listOfAvailableGodsTotems;
    [SerializeField] List<CardScript> _listOfAllCards;
    [SerializeField] public string ownerName 
    { 
        get; 
        private set; 
    }
    public List<CardScript> ListOfAllCards
    {
        get => _listOfAllCards;
        set => _listOfAllCards = value;
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
    [ContextMenu("Execute selected skill")] public void OnClick_ExecuteSelectedGodSkill()
    {
        if (!AnySkillInOwnedCardsIsSelected)
        {
            AndroidLogger.Log("Skill is not selected / skill already used",AndroidLogger.GetPlayerLogColor(ownerName));
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
                    print("nie stać Cie na tego skilla");
                    AndroidLogger.Log("you dont have enought Gold to cast skill", AndroidLogger.GetPlayerLogColor(ownerName));
                    lastUsedSkill.SkillIsSelected = false;
                }
            }
        }
        CollorSkillButtonsIfCanBeUsed();
    }
    public void CollorSkillButtonsIfCanBeUsed()
    {
        try
        {    
            foreach (var god in _godCardsInContainer.Where(g=>g._card.IsReverseRevelated))
            {
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
        catch (System.Exception)
        {
            print("skill uzyty przez bota bez uzycia interfejsu.");
        }
    }
    private void ChangeSkillButtonToEnabled(GodScript godScript, int level)
    {
        GameObject skillButton = godScript.transform.Find("RewersContent")
            .transform.Find($"Skill Level {level}")
            .transform.gameObject;

        skillButton.GetComponent<Image>().color = new Color32(255, 255, 255, 128);
        skillButton.GetComponentInChildren<Text>().color = Color.white;;
    }
    private void ChangeSkillButtonToDissabled(GodScript godScript, int level)
    {
        GameObject skillButton = godScript.transform.Find("RewersContent")
            .transform.Find($"Skill Level {level}")
            .transform.gameObject;

        skillButton.GetComponent<Image>().color = new Color32(255, 0, 0, 128);
        skillButton.GetComponentInChildren<Text>().color = Color.red;
    }
    private void PayGoldForSkill(GodScript myGod, string godOwner)
    {
        var coinText = godOwner == "Player1"?GameObject.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>():GameObject.Find("CoinTextPlayer2").GetComponent<TextMeshProUGUI>();

        for (int i = 0; i < myGod._skill.GetGoldCostForSkillLevel(myGod._skill.selectedSkillLevel); i++)
        {
            if(godOwner == "Player1") GM_Script.Player_1.TemporaryGoldVault--;
            if(godOwner == "Player2") GM_Script.Player_2.TemporaryGoldVault--;
            coinText.color = Color.red;
        }

        GM_Script.Player_1.CumulativeGoldStealingCounter = 0;
        GM_Script.Player_2.CumulativeGoldStealingCounter = 0;
        
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
