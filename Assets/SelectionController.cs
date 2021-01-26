using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectionController : MonoBehaviour
{   
    [SerializeField] GodScript God_Script;
    [SerializeField] CardScript Card_Script;
    [SerializeField] Skill Skill = null;
    [SerializeField] bool _isSkillSelected;
    [SerializeField] public bool IsSkillSelected
    {
        get => God_Script._skill.SkillIsSelected;
        set
        {
            _isSkillSelected = value;
            if (value == true)
            {
                GameObject skillButton = FindLocalisationOfButtonSelectedSkill(Skill);
                MarkSkillAsSelected(skillButton);
            }
            else
            {
                if(LastActivatedBorder != null)
                {
                    HideLastSelectedBorder();
                }
            }
        }
    }
    
    void Start()
    {
        Card_Script = GetComponent<CardScript>();
        God_Script = GetComponent<GodScript>();
    }
    void FixedUpdate()
    {
        if(Card_Script.IsReverseRevelated)
        {
            if(God_Script._skill != null)
            {
                // add script only once when its added into godScript
                Skill = God_Script._skill;    
            }

            if(Skill != null)
            {
                IsSkillSelected = IsSkillSelected;
            }
        }
    }

   
    void HideLastSelectedBorder()
    {
        GameObject skillButton = LastActivatedBorder.transform.parent.transform.gameObject;
        skillButton.GetComponent<Image>().color = new Color32(255,255,255,128);
        skillButton.GetComponentInChildren<Text>().color = Color.white;

        LastActivatedBorder.transform.GetComponent<Image>().color = Color.clear;
        LastActivatedBorder = null;
    }
    void MarkSkillAsSelected(GameObject skillButton)
    {
        //save and change border state
        LastActivatedBorder = skillButton.transform.Find("Border-Selected").transform.gameObject;
        LastActivatedBorder.GetComponent<Image>().color = Color.yellow;

        // get button and make on him colorize stuff
        skillButton.GetComponent<Image>().color = new Color32(255,255,0,128);
        skillButton.GetComponentInChildren<Text>().color = Color.yellow;
    }
    public GameObject FindLocalisationOfButtonSelectedSkill(Skill skill)
    {
        int skillLevel= skill.selectedSkillLevel;
        switch (skillLevel)
        {
            case 1:
          //  print(skillLevel);
                return this.transform.Find("RewersContent").transform.Find("Skill Level 1").transform.gameObject;

            case 2:
            //print(skillLevel);
                return this.transform.Find("RewersContent").transform.Find("Skill Level 2").transform.gameObject;
           
            case 3:
            //print(skillLevel);
                return this.transform.Find("RewersContent").transform.Find("Skill Level 3").transform.gameObject;
        }

        return null;
    }
    
    // TODO: zmieinć to na liste 2 obiektów dictionary przechowujących obiekt ostatniego selektu i nazwe gracza
    [SerializeField] static GameObject LastActivatedBorder = null; 
    static public void UnselectControllerWhoContainSkill(Skill skill, string ownerName)
    {
        GameManager GM = GameObject.FindObjectOfType<GameManager>();
        List<SelectionController> listOfSelectionControllers = new List<SelectionController>();
        switch(ownerName)
        {
            case "Player1":
                listOfSelectionControllers = GM.Player1GodSkillWindow.GetComponent<GodsManager>()._selectionControllers;
            break;

            case "Player2":
                listOfSelectionControllers = GM.Player2GodSkillWindow.GetComponent<GodsManager>()._selectionControllers;
            break;
        }

        print("odznaczenie skilla na tej samej karcie");
        listOfSelectionControllers.Where(s=>s.Skill == skill).First().IsSkillSelected = false;
        // throw new NotImplementedException();
    }
    static public bool CheckIfAnyOtherSkillsAlreadySelected(string skillOwner)
    {
        // przeszukaj kopie selection controllerow dla reszty swoich bogów 
        // jeżeli znajdziesz inny select zwróc true

        var selectorControllers = GameObject.Find(skillOwner).GetComponentInChildren<GodsManager>()._selectionControllers;

        if(selectorControllers.Where(s=>s.IsSkillSelected).Any()) 
        {
            print("skill dla innego boga jest juz zanzaczony");
            return true;
        }
        else
        {
            return false;
        }
    }
    static public Skill GetSelectedSkill(string skillOwner)
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