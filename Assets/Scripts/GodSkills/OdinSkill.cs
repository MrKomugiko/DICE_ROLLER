using System;
using UnityEngine;

[Serializable]
public class OdinSkill : Skill
{
    /*
    
        Title:
        "Odin`s Sacrifice"

        Description:
        After the Resolution phase, sacrifice any
        number of your health tokens.
        Gain [Gold] per health token sacrificed.

        Skills:
        [cost]  [effect]
          6     Gain 3 [Gold] per health token
          8     Gain 4 [Gold] per health token
          10    Gain 5 [Gold] per health token

    */
    public OdinSkill(God godData)
    {
        God = godData;
        ID = 12;
        GodName = "Odin";
        SkillName = "Odin`s Sacrifice";

        ListOfSkills.Add(this);

        Debug.Log("Jestem skillem "+God.name+".");

       // Debug.Log("utworzono nowy skill aktualna liczba to " + ListOfSkills.Count);
    }

    public override void UseSkill(int skillLevel, string castingPlayer)
    {
        base.UseSkill(skillLevel, castingPlayer);
        Debug.Log("Bóg" + GodName + " uzywa skilla " + skillLevel+" posiomu!.");
        // TU COS SIE BEDZIE DZIAĆ :D
    }
}