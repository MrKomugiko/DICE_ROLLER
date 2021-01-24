using System.CodeDom.Compiler;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;

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
            GodsManager.AndroidDebug("Skill not beed selected to use.");
            return;
        }

        GodsManager.AndroidDebug("Execute "+SkillName+"["+selectedSkillLevel+ " level], by player  "+selectedCastingPlayer);
        methodToCall(selectedSkillLevel,selectedCastingPlayer);
        SkillIsSelected = false;
    }

    public static Skill GetGodSkillByID(int id, God godData, string ownerName)
    {
        GenerateGodsSkillScripts(godData,ownerName); 

        return ListOfSkills.Where(s => s.ID == id && s.OwnerName == ownerName).First();
    }

    public void SelectSkill(int skillLevel, string castingPlayer)
    {
        string color = castingPlayer == "Player1"?"green":"red";
        if(!SkillIsSelected)
           {
            selectedSkillLevel = skillLevel;
            selectedCastingPlayer = castingPlayer;
            
            Debug.Log($"Wybieram skilla  {skillLevel}lvl");
            GodsManager.AndroidDebug("You choose" + GodName + " skill at level : " + skillLevel,color);
            SkillIsSelected = true;
           }
    }

    protected virtual void UseSkill(int skillLevel, string castingPlayer)
    {
        GodsManager.AndroidDebug("Using skill");
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
        GodsManager.AndroidDebug("Current avaiable skills in memory :" + ListOfSkills.Count);
    }
    public bool CheckIfCanBeUsed(int currentGold, int skillCost)
    {
        if (currentGold < skillCost) return true;

        return true;
    }
}







