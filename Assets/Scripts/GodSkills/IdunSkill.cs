using System;
using UnityEngine;

[Serializable]
public class IdunSkill : Skill
{
    /*
    
        Title:
        "Idun`s Rejuvenation"

        Description:
        Heal Health after the Resolution phase.

        Skills:
        [cost]  [effect]
          4     Heal 2 Heath
          7     Heal 4 Heath
          10    Heal 6 Heath

    */
 
    public IdunSkill(God godData, string ownerName)
    {
              OwnerName = ownerName;
        God = godData;
        ID = 9;
        GodName = "Idun";
        SkillName = "Idun's Rejuvenation";
        
        ListOfSkills.Add(this);
     
        Debug.Log("Jestem skillem "+God.name+".");

        // Debug.Log("utworzono nowy skill aktualna liczba to "+ListOfSkills.Count);
    }
    
}