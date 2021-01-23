using System;
using UnityEngine;

[Serializable]
public class IdunSkill : Skill
{
    public IdunSkill()
    {
        ID = 9;
        GodName = "Idun";
        SkillName = "Idun's Rejuvenation";
        
        ListOfSkills.Add(this);
     
        Debug.Log("Jestem skillem Iduna");

        Debug.Log("utworzono nowy skill aktualna liczba to "+ListOfSkills.Count);
    }
    
    public override void UseSkill(int skillLevel)
    {
        base.UseSkill(skillLevel);
        Debug.Log("Skill Użyty dla boga"+ GodName+" i posiomu "+skillLevel);
        // TU COS SIE BEDZIE DZIAĆ :D
    }
}