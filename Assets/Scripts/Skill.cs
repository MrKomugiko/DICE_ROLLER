using System.CodeDom.Compiler;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using System.Collections;

[SerializeField]
public class Skill
{
    protected string OwnerName {get;set;}
    public God God { get; set; }
    public int ID { get; set; }
    public string GodName { get; set; }
    public string SkillName { get; set; }
    public static List<Skill> ListOfSkills = new List<Skill>();
    internal GameManager GM_Script = GameObject.Find("GameManager").GetComponent<GameManager>();

    public delegate void TestDelegate(int skillLevel, string castingPlayer);
    public TestDelegate methodToCall;
    private bool _skillIsSelected;
    public bool SkillIsSelected 
    { 
        get => _skillIsSelected; 
        set 
        { 
            _skillIsSelected = value; 
            
            if(value)
            {
                methodToCall = UseSkill;
                // zapisanie delegaty ( wybranej metody ktoa uzyje sie pozniej ? ) 
            }
            else
            {
                methodToCall = null;
            }
        }
    }
    int selectedSkillLevel; string selectedCastingPlayer;
    public void LastSelectedSkillReadyToUse()
    {
        if(methodToCall == null)    
        {
            AndroidLogger.Log("Skill not beed selected to use.");
            return;
        }

        AndroidLogger.Log("Execute "+SkillName+"["+selectedSkillLevel+ " level], by player  "+selectedCastingPlayer);
        // TODO: tutaj walniemy sprawdanie golda

        // odejmowanie wymaganego golda graczowi któy rzuca skill
        var p1coin = GameObject.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>();
        var p2coin = GameObject.Find("CoinTextPlayer2").GetComponent<TextMeshProUGUI>();
        switch (OwnerName)
        {
            case "Player1":
                for (int i = 0; i < GetGoldCostForSkillLevel(selectedSkillLevel); i++)
                {
                    GM_Script.TemporaryGoldVault_player1--;
                    p1coin.color = Color.red;
                }
                break;
            
            case "Player2":
                for (int i = 0; i < GetGoldCostForSkillLevel(selectedSkillLevel); i++)
                {
                    GM_Script.TemporaryGoldVault_player2--;
                    p2coin.color = Color.red;
                }
                break;
        }

        methodToCall(selectedSkillLevel,selectedCastingPlayer);
        SkillIsSelected = false;

    }

    public void SelectSkill(int skillLevel, string castingPlayer)
    {
        string color = castingPlayer == "Player1"?"green":"red";
        if(!SkillIsSelected)
        {
            selectedSkillLevel = skillLevel;
            selectedCastingPlayer = castingPlayer;

            AndroidLogger.Log("You choose" + GodName + " skill at level : " + skillLevel,color);
            SkillIsSelected = true;
        }
    }

    protected virtual void UseSkill(int skillLevel, string castingPlayer)
    {
        AndroidLogger.Log("Using skill");
    }
    public static Skill GetGodSkillByID(int id, God godData, string ownerName)
    {
        GenerateGodsSkillScripts(godData,ownerName); 

        return ListOfSkills.Where(s => s.ID == id && s.OwnerName == ownerName).First();
    } 
    static void GenerateGodsSkillScripts(God godData, string ownerName)
    {
        switch (godData.name)
        {
            case "Bragi":
                new BragiSkill(godData,ownerName);
                break;
            case "Idun":
                new IdunSkill(godData,ownerName);
                break;
            case "Thor":
                new ThorSkill(godData,ownerName);
                break;
            case "Odin":
                new OdinSkill(godData,ownerName);
                break;
        }

        Debug.Log("Aktualna liczbaskilli w pamieci :" + ListOfSkills.Count);
        AndroidLogger.Log("Current avaiable skills in memory :" + ListOfSkills.Count);
    }
    public bool CheckIfCanBeUsed(int currentGold, int skillCost)
    {
        if (currentGold < skillCost) return true;

        return true;
    }
    public int GetValueForSkillLevel(int skillLevel)
    {
        int skillValue = 0;

        switch (skillLevel)
        {
            case 1:
                skillValue = God.Level1SkillValue;
                break;

            case 2:
                skillValue = God.Level2SkillValue;
                break;

            case 3:
                skillValue = God.Level3SkillValue;
                break;
        }

        return skillValue;
    }
    public int GetGoldCostForSkillLevel(int skillLevel)
    {
        int skillCost = 0;

        switch (skillLevel)
        {
            case 1:
                skillCost = God.LevelCost;
                break;

            case 2:
                skillCost = God.Leve2Cost;
                break;

            case 3:
                skillCost = God.Leve3Cost;
                break;
        }

        return skillCost;
    }
}







