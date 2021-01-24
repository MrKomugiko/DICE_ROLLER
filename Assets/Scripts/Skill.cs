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
 
    public bool IsSkillInUse {  get; set;  }

    public static Skill GetGodSkillByID(int id, God godData)
    {
        GenerateGodsSkillScripts(godData);

        return ListOfSkills.Where(s => s.ID == id).First();
    }

    public virtual void UseSkill(int skillLevel, string castingPlayer)
    {
        Debug.Log($"UZywam skilla  {skillLevel}lvl");
        GodsManager.AndroidDebug("Skill Użyty dla boga"+ GodName+" poziom: "+skillLevel);
        IsSkillInUse = true;
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
        if(currentGold < skillCost) return true;

        return true;
    }
}







