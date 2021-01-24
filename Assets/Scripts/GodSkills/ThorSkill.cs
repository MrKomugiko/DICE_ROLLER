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
    public ThorSkill(God godData)
    {
        God = godData;
        ID = 15;
        GodName = "Thor";
        SkillName = "Thor's Strike";

        ListOfSkills.Add(this);

        Debug.Log("Jestem skillem " + God.name + ".");

        // Debug.Log("utworzono nowy skill aktualna liczba to " + ListOfSkills.Count);
    }

    public override void UseSkill(int skillLevel, string castingPlayer)
    {
        base.UseSkill(skillLevel, castingPlayer);

        GodsManager.AndroidDebug($"{castingPlayer} use [{SkillName}][LVL-{skillLevel}]");

        GameManager GM_Script = GameObject.Find("GameManager").GetComponent<GameManager>();

        int value = 0;
        switch (skillLevel)
        {
            case 1:
                value = God.Level1SkillValue;
                break;

            case 2:
                value = God.Level2SkillValue;
                break;

            case 3:
                value = God.Level3SkillValue;
                break;
        }
        // zadanie obrażeń
        switch (castingPlayer)
        {
            case "Player1":
                for (int i = 0; i < value; i++)
                {
                    GM_Script.TemporaryIntakeDamage_Player2 ++;
                }
                break;

            case "Player2":
                for (int i = 0; i < value; i++)
                {
                    GM_Script.TemporaryIntakeDamage_Player1 ++;
                }
                break;
        }

        GodsManager.AndroidDebug("Zadano "+value.ToString()+" obrażeń.");
        // reset obrażeń dla przeciwnika
    }


    /*
     *  Resource checking
     *
     *  Skill cast blocked until right moment
     *
     *  
     *
     *  fastforwrd battle ;d
     *      1. kostka musi znajdowac sie na polu bitwy -> trzeba to obejść i rozpisac inaczej
     *      2. TemporaryIntakeDamage_Player1/2 trzeba ręcznie wyzerować, żeby wskaznik zadanego obrazenia zniknął
     */
}