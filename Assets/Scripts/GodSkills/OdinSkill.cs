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
  
    public OdinSkill(God godData, string ownerName)
    {
              OwnerName = ownerName;
        God = godData;
        ID = 12;
        GodName = "Odin";
        SkillName = "Odin`s Sacrifice";

        ListOfSkills.Add(this);
    }

}