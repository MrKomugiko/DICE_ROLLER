using System;
using UnityEngine;

[Serializable]
public class BragiSkill : Skill
{
    /*
    
        Title:
        "Bragi`s Verve"

        Description:
        Gain [Gold] for each die that rolled [Hand].

        Skills:
        [cost]  [effect]
          4     Gain 2 [Gold] per die
          8     Gain 3 [Gold] per die
          12    Gain 4 [Gold] per die

    */
    public BragiSkill(God godData)
    {
        God = godData;
        ID = 2;
        GodName = "Bragi";
        SkillName = "Bragi's Verve";
        
        ListOfSkills.Add(this);
     
      Debug.Log("Jestem skillem "+God.name+".");

        // Debug.Log("utworzono nowy skill aktualna liczba to "+ListOfSkills.Count);
        // GodsManager.AndroidDebug("utworzono nowy skill aktualna liczba to "+ListOfSkills.Count);
    }

    public override void SelectSkill(int skillLevel, string castingPlayer)
    {
        base.SelectSkill(skillLevel, castingPlayer);
        Debug.Log("Skill Użyty dla boga "+ GodName+" "+skillLevel);
                // TU COS SIE BEDZIE DZIAĆ :D
    }
}   