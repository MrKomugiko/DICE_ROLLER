using System.Data.Common;
using System.Linq;
using System;
using UnityEngine;

[Serializable]
public class ThorSkill : Skill
{
    /*
    
        Title:
        "Thor's Strike"

        Description:
        Deal damage to the opponent after the Resolution phase.
        
        Skills:
        [cost]  [effect]
          4     Deal 2 damage
          8     deal 5 damage
          12    deal 8 damage

    */

    public ThorSkill(God godData, string ownerName)
    {
        OwnerName = ownerName;
        God = godData;
        ID = 15;
        GodName = "Thor";
        SkillName = "Thor's Strike";

        ListOfSkills.Add(this);
    }

    protected override void UseSkill(int skillLevel, string castingPlayer)
    {
        AndroidLogger.Log($"{castingPlayer} use [{SkillName}][LVL-{skillLevel}]");

        int damageValue = GetValueForSkillLevel(skillLevel);

        DamageOpponent(castingPlayer, damageValue);
    }

    private void DamageOpponent(string castingPlayer, int damageValue)
    {

        for (int i = 0; i < damageValue; i++)
        {
            if (castingPlayer == "Player1")
            {
                GM_Script.TemporaryIntakeDamage_Player2++;
            }
            else
            {
                GM_Script.TemporaryIntakeDamage_Player1++;
            }
        }
        AndroidLogger.Log("You dealt " + damageValue.ToString() + " damage.");
    }
}