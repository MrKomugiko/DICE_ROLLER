using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectionController : MonoBehaviour
{
    
    public string Owner {
        get 
        {
            return God_Script.ownerName;
        }
    }
    [SerializeField] GodScript God_Script;
    [SerializeField] CardScript Card_Script;
    [SerializeField] Skill Skill = null;
    [SerializeField] bool _isSkillSelected;
    [SerializeField]
    public bool IsSkillSelected
    {
        get => God_Script._skill.SkillIsSelected;
        set
        {
            _isSkillSelected = value;
            if (value == true)
            {
                //TODO: sprawdzenie czy można uzywać skila, w inym wypadku środek zostaje czerwony ?
                GameObject skillButton = FindLocalisationOfButtonSelectedSkill(Skill);
                MarkSkillAsSelected(skillButton, Owner);
            }
            else
            {
                switch (Owner)
                {
                    
                    case "Player1":
                        if (LastActivatedBorder_Player1 != null)
                        {
                            HideLastSelectedBorder("Player1");
                        }
                        break;
                    case "Player2":
                        if (LastActivatedBorder_Player2 != null)
                        {
                            HideLastSelectedBorder("Player2");
                        }
                        break;     
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
            DEBUG_Border_Player1 = LastActivatedBorder_Player1;
            DEBUG_Border_Player2 = LastActivatedBorder_Player2;

        if (Card_Script.IsReverseRevelated)
        {
            if (God_Script._skill != null)
            {

                // add script only once when its added into godScript
                Skill = God_Script._skill;
            }

            if (Skill != null)
            {
                IsSkillSelected = IsSkillSelected;
            }
        }
    }


    void HideLastSelectedBorder(string owner)
    {
        print("hide last border by player "+ owner);
         if(owner == "Player1")
        {
            GameObject skillButton = LastActivatedBorder_Player1.transform.parent.transform.gameObject;
            skillButton.GetComponent<Image>().color = new Color32(255, 255, 255, 128);
            skillButton.GetComponentInChildren<Text>().color = Color.white;

            LastActivatedBorder_Player1.transform.GetComponent<Image>().color = Color.clear;
            LastActivatedBorder_Player1 = null;
        }
         if(owner == "Player2")
        {
            GameObject skillButton = LastActivatedBorder_Player2.transform.parent.transform.gameObject;
            skillButton.GetComponent<Image>().color = new Color32(255, 255, 255, 128);
            skillButton.GetComponentInChildren<Text>().color = Color.white;

            LastActivatedBorder_Player2.transform.GetComponent<Image>().color = Color.clear;
            LastActivatedBorder_Player2 = null;
        }
    }
    
    void MarkSkillAsSelected(GameObject skillButton, string owner)
    {
     
    Color selectionBackgroundColor = Color.clear;
    Color selectionBorderColor = Color.clear;
    Color selectionTextColor = Color.clear;

      //TODO: sprawdzenie czy można uzywać skila, w inym wypadku środek zostaje czerwony ?
      var godCardDescription = skillButton.transform.parent;
      var godCard = godCardDescription.transform.parent;
      var godsContainer = godCard.transform.parent;
      var godsManager_Script = godsContainer.GetComponent<GodsManager>(); 
      int levelOfSkillToCheck = godCard.GetComponent<GodScript>()._skill.selectedSkillLevel;
      Skill skillCurrentlychecking =   godCard.GetComponent<GodScript>()._skill;

      if(godsManager_Script.CheckIfSkillCanBeUsed(skillCurrentlychecking,levelOfSkillToCheck))
      {
          // ZAZNACZONY SKILL KTOREGO MOZEMY UZYC
        selectionBackgroundColor = new Color32(255, 255, 0, 128); // yellow
        selectionBorderColor = Color.yellow;
        selectionTextColor = Color.yellow;
      }
        else 
        {
          // ZAZNACZONy SKILL NA KTÓRY NAS NIE STAĆ :D
            selectionBackgroundColor = new Color32(255, 0, 0, 128); // red
            selectionBorderColor = Color.red;
            selectionTextColor = Color.red;
        }
      
        //save and change border state
        //  LastActivatedBorder = skillButton.transform.Find("Border-Selected").transform.gameObject;
        //  LastActivatedBorder.GetComponent<Image>().color = Color.yellow;
        if(owner == "Player1")
        {
            LastActivatedBorder_Player1 = skillButton.transform.Find("Border-Selected").transform.gameObject;
            LastActivatedBorder_Player1.GetComponent<Image>().color = selectionBorderColor;
            // get button and make on him colorize stuff
            skillButton.GetComponent<Image>().color = selectionBackgroundColor;
            skillButton.GetComponentInChildren<Text>().color = selectionTextColor;
        }
        else
        {
            LastActivatedBorder_Player2 = skillButton.transform.Find("Border-Selected").transform.gameObject;
            LastActivatedBorder_Player2.GetComponent<Image>().color = selectionBorderColor;
                    // get button and make on him colorize stuff
            skillButton.GetComponent<Image>().color = selectionBackgroundColor;
            skillButton.GetComponentInChildren<Text>().color = selectionTextColor;
        }

    }
    public GameObject FindLocalisationOfButtonSelectedSkill(Skill skill)
    {
        int skillLevel = skill.selectedSkillLevel;
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

    [SerializeField] GameObject DEBUG_Border_Player1;
    [SerializeField] GameObject DEBUG_Border_Player2;

    [SerializeField] static GameObject LastActivatedBorder = null; 
    static GameObject LastActivatedBorder_Player1 = null;
    static GameObject LastActivatedBorder_Player2 = null;

    static public void UnselectControllerWhoContainSkill(Skill skill, string ownerName)
    {
        GameManager GM = GameObject.FindObjectOfType<GameManager>();
        List<SelectionController> listOfSelectionControllers = new List<SelectionController>();
        switch (ownerName)
        {
            case "Player1":
                listOfSelectionControllers = GM.Player1GodSkillWindow.GetComponent<GodsManager>()._selectionControllers;
                break;

            case "Player2":
                listOfSelectionControllers = GM.Player2GodSkillWindow.GetComponent<GodsManager>()._selectionControllers;
                break;
        }

        //print("odznaczenie skilla na tej samej karcie");
        var skills = listOfSelectionControllers.Where(s => s.Skill == skill);
        foreach(var skil in skills)
        {
            skil.IsSkillSelected = false;
        }
            
        // throw new NotImplementedException();
    }
    static public bool CheckIfAnyOtherSkillsAlreadySelected(string skillOwner)
    {
        // przeszukaj kopie selection controllerow dla reszty swoich bogów 
        // jeżeli znajdziesz inny select zwróc true

        var selectorControllers = GameObject.Find(skillOwner).GetComponentInChildren<GodsManager>()._selectionControllers;

        if (selectorControllers.Where(s => s.IsSkillSelected).Any())
        {
            print("Inny skill jest już zaznaczony");
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

        foreach (var god in allOwnerGods)
        {
            ownedSkills.Add(god._skill);
        }

        Skill selectedSkill = ownedSkills.Where(s => s.SkillIsSelected).FirstOrDefault();

        //print($"aktualnie wybrany skill to: {selectedSkill.SkillName}, {selectedSkill.selectedSkillLevel} lvl. Należący do boga: {selectedSkill.GodName}");
        return selectedSkill;
    }
}