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
    }
    protected override void UseSkill(int skillLevel, string castingPlayer)
    {
        base.UseSkill(skillLevel,castingPlayer);
        
        int healValue = GetValueForSkillLevel(skillLevel);

        HealCaster(castingPlayer, healValue);
    }
    private void HealCaster(string castingPlayer, int healValue)
    {
        for (int i = 0; i < healValue; i++)
        {
            if (castingPlayer == "Player1")
            {
                GM_Script.Player_1.TemporaryIntakeDamage--;
            }
            else
            {
                GM_Script.Player_2.TemporaryIntakeDamage--;
            }
        }
        AndroidLogger.Log("You healed for " + healValue.ToString() + " health points.", AndroidLogger.GetPlayerLogColor(castingPlayer));
    }
}