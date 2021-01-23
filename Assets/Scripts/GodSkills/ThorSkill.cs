using System;
using UnityEngine;

[Serializable]
public class ThorSkill : Skill
{
    public ThorSkill()
    {
        ID = 2;
        GodName = "Bragi";
        SkillName = "Bragi's Verve";

        ListOfSkills.Add(this);

        Debug.Log("Jestem skillem bragiego");

        Debug.Log("utworzono nowy skill aktualna liczba to " + ListOfSkills.Count);
    }

    public override void UseSkill(int skillLevel)
    {
        Debug.Log("Skill Użyty dla boga" + GodName + " i posiomu " + skillLevel);
        // TU COS SIE BEDZIE DZIAĆ :D
    }
}