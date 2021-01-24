using System.CodeDom.Compiler;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;

[SerializeField]
public class Skill
{
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
                methodToCall = SelectSkill;
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
            GodsManager.AndroidDebug("brak wybranego skilla do uzycia");
            return;
        }

        GodsManager.AndroidDebug("Wykonanie dla "+selectedSkillLevel+ " level, przez gracza "+selectedCastingPlayer);
        methodToCall(selectedSkillLevel,selectedCastingPlayer);
        SkillIsSelected = false;
    }

    public static Skill GetGodSkillByID(int id, God godData)
    {
        GenerateGodsSkillScripts(godData);

        return ListOfSkills.Where(s => s.ID == id).First();
    }

    public virtual void SelectSkill(int skillLevel, string castingPlayer)
    {
        if(SkillIsSelected) 
        {
            GodsManager.AndroidDebug("Skill został już wybrany wczesniej");
            return;
        }

        selectedSkillLevel = skillLevel;
        selectedCastingPlayer = castingPlayer;
        
        SkillIsSelected = true;
        Debug.Log($"UZywam skilla  {skillLevel}lvl");
        GodsManager.AndroidDebug("Skill Użyty przez boga" + GodName + " poziom: " + skillLevel);
    }

    static void GenerateGodsSkillScripts(God godData)
    {
        switch (godData.name)
        {
            case "Bragi":
                new BragiSkill(godData);
                break;
            case "Idun":
                new IdunSkill(godData);
                break;
            case "Thor":
                new ThorSkill(godData);
                break;
            case "Odin":
                new OdinSkill(godData);
                break;
        }

        Debug.Log("Aktualna liczbaskilli w pamieci :" + ListOfSkills.Count);
        GodsManager.AndroidDebug("Aktualna liczbaskilli w pamieci :" + ListOfSkills.Count);
    }
    public bool CheckIfCanBeUsed(int currentGold, int skillCost)
    {
        if (currentGold < skillCost) return true;

        return true;
    }
}







