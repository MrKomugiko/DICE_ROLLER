using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectionController : MonoBehaviour
{   
    [SerializeField] GodScript God_Script;
    Skill Skill = null;

    [SerializeField] bool _IsSkillSelected;

    public bool IsSkillSelected1 => God_Script._skill.SkillIsSelected; 
    void Start()
    {
        God_Script = GetComponent<GodScript>();
    }
    void FixedUpdate()
    {
        if(God_Script._skill != null)
        {
            // add script only once when its added into godScript
            Skill = God_Script._skill;    
        }

        if(Skill != null)
        {
            _IsSkillSelected = IsSkillSelected1;
        }
    }

    static public bool CheckIfAnyOtherSkillsAlreadySelected(string skillOwner)
    {
        // przeszukaj kopie selection controllerow dla reszty swoich bogów 
        // jeżeli znajdziesz inny select zwróc true

        var selectorControllers = GameObject.Find(skillOwner).GetComponentInChildren<GodsManager>()._selectionControllers;

        if(selectorControllers.Where(s=>s.IsSkillSelected1).Any()) 
        {
            print("skill dla innego boga jest juz zanzaczony");
            return true;
        }
        else
        {
            print("żaden skill nie jest zaznaczony");
            return false;
        }
    }

    public static Skill GetSelecteSkill(string skillOwner)
    {
        var allOwnerGods = GameObject.Find(skillOwner).GetComponentInChildren<GodsManager>()._godCardsInContainer;

        List<Skill> ownedSkills = new List<Skill>();

        foreach(var god in allOwnerGods)
        {
            ownedSkills.Add(god._skill);
        }

        Skill selectedSkill = ownedSkills.Where(s=>s.SkillIsSelected).FirstOrDefault();

        print($"aktualnie wybrany skill to: {selectedSkill.SkillName}, {selectedSkill.selectedSkillLevel} lvl. Należący do boga: {selectedSkill.GodName}");
        return selectedSkill;
    }
}