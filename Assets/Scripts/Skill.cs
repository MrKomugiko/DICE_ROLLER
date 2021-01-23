using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;

[SerializeField]
public class Skill
{
    public int ID { get; set; }
    public string GodName { get; set; }
    public string SkillName { get; set; }

    public static List<Skill> ListOfSkills = new List<Skill>();
    public Skill()
    { 
    }

    public static Skill GetGodSkillByID(int id)
    {
        Debug.Log(("aktualna liczbaskilli w pamieci :" + ListOfSkills.Count));
        return ListOfSkills.Where(s => s.ID == id).FirstOrDefault();
    }

    public virtual void UseSkill(int skillLevel)
    {
        Debug.Log($"UZywam skilla  {skillLevel}lvl");

        // TU COS SIE BEDZIE DZIAĆ :D
    }
}