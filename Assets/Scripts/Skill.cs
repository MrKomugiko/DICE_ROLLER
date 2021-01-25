using System.Runtime;
using System.Dynamic;
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
                string color = OwnerName == "Player1"?"green":"red";
                AndroidLogger.Log("Skill used.",color);
                methodToCall = null;
            }
        }
    }


    public int selectedSkillLevel; string selectedCastingPlayer;
    public void LastSelectedSkillReadyToUse()
    {
        string color = OwnerName == "Player1"?"green":"red";
        if(methodToCall == null)    
        {
            AndroidLogger.Log("Skill not beed selected to use.");
            return;
        }

        AndroidLogger.Log("Execute "+SkillName+"["+selectedSkillLevel+ " level], by player  "+selectedCastingPlayer,color);

        methodToCall(selectedSkillLevel,selectedCastingPlayer);
        SkillIsSelected = false;
    }
    public void TrySelectSkill(int skillLevel, string castingPlayer)
    { 
        if(!SelectionController.CheckIfAnyOtherSkillsAlreadySelected(OwnerName))
        {
            SelectSkill(skillLevel, castingPlayer);
        }
        else
        {
            Debug.Log("odznaczanie wczesniejszego skilla");
            UnSelectAnySelectedSkill();

            Debug.Log("ponowna próba zanzaczenia aktualnie wybranego");
            SelectSkill(skillLevel, castingPlayer);
        }
    }


    private void UnSelectAnySelectedSkill()
    {
        Skill recentSelectedSkill = SelectionController.GetSelecteSkill(OwnerName);
        recentSelectedSkill.SkillIsSelected = false;
    }
    private void SelectSkill(int skillLevel, string castingPlayer)
    {
        string color = castingPlayer == "Player1" ? "green" : "red";
        if (!SkillIsSelected)
        {
            selectedSkillLevel = skillLevel;
            selectedCastingPlayer = castingPlayer;

            AndroidLogger.Log("You choose" + GodName + " skill at level : " + skillLevel, color);
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







