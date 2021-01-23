using System;
using UnityEngine;

[Serializable]
public class ThorSkill : Skill
{
    public ThorSkill()
    {
        ID = 15;
        GodName = "Thor";
        SkillName = "Thor's Strike";

        ListOfSkills.Add(this);

        Debug.Log("Jestem skillem Thora");

        Debug.Log("utworzono nowy skill aktualna liczba to " + ListOfSkills.Count);
    }

    public override void UseSkill(int skillLevel)
    {
        base.UseSkill(skillLevel);
        Debug.Log("Bóg" + GodName + " uzywa skilla " + skillLevel+" posiomu!.");
        // TU COS SIE BEDZIE DZIAĆ :D
    }
}