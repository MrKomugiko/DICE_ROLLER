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
            GodsManager.AndroidDebug($"{castingPlayer} use [{SkillName}][LVL-{skillLevel}]");

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
        GodsManager.AndroidDebug("You dealt " + damageValue.ToString() + " damage.");
    }

    internal int GetValueForSkillLevel(int skillLevel) 
    {
         int skillValue = 0;

        switch (skillLevel)
        {
            case 1:
                skillValue = God.Level1SkillValue;
                break;

            case 2:
                skillValue = God.Level2SkillValue;
                break;

            case 3:
                skillValue = God.Level3SkillValue;
                break;
        }

        return skillValue;
    }
}