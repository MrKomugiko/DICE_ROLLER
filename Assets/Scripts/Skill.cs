using System.CodeDom.Compiler;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;

[SerializeField]
public class Skill
{
    public int ID { get; set; }
    public string GodName { get; set; }
    public string SkillName { get; set; }

    public static bool IsPopulated = false;
    public static List<Skill> ListOfSkills = new List<Skill>();
 
    public Skill()
    { 
    }

    public static Skill GetGodSkillByID(int id)
    {
        if(ListOfSkills.Count == 0) GenerateGodsSkillScripts();

        return ListOfSkills.Where(s => s.ID == id).First();
    }

    public virtual void UseSkill(int skillLevel)
    {
        Debug.Log($"UZywam skilla  {skillLevel}lvl");
        GodsManager.AndroidDebug("Skill Użyty dla boga"+ GodName+" poziom: "+skillLevel);
    }

    static void GenerateGodsSkillScripts()
    {
        if(IsPopulated) return;

        GodsManager.AndroidDebug("Generowanie skryptów skilli dla bogów.");
        Debug.Log("Generowanie skryptów skilli dla bogów.");

        new BragiSkill();
        new IdunSkill();
        new ThorSkill();

        Debug.Log("Aktualna liczbaskilli w pamieci :" + ListOfSkills.Count);
        GodsManager.AndroidDebug("Aktualna liczbaskilli w pamieci :" + ListOfSkills.Count);

    }
}







