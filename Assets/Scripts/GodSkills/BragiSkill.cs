using System;
using UnityEngine;

[Serializable]
public class BragiSkill : Skill
{
    public BragiSkill()
    {
        ID = 2;
        GodName = "Bragi";
        SkillName = "Bragi's Verve";
        
        ListOfSkills.Add(this);
     
        Debug.Log("Jestem skillem bragiego");

        Debug.Log("utworzono nowy skill aktualna liczba to "+ListOfSkills.Count);
        GodsManager.AndroidDebug("utworzono nowy skill aktualna liczba to "+ListOfSkills.Count);
    }

    public override void UseSkill(int skillLevel)
    {
        base.UseSkill(skillLevel);
        Debug.Log("Skill Użyty dla boga "+ GodName+" "+skillLevel);
                // TU COS SIE BEDZIE DZIAĆ :D
    }
}   