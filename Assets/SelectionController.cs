using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectionController : MonoBehaviour
{
    public string Owner
    {
        get
        {
            return God_Script.ownerName;
        }
    }
    GodScript God_Script;
    CardScript Card_Script;
    Skill Skill = null;
    [SerializeField] bool _isSkillSelected;

    int loggerCounter = 1;
    [SerializeField] public string _infoAboutCurrentActiveSkill;
    string InfoAbutCurrentActiveSkill 
    { 
        get 
        {
            return _infoAboutCurrentActiveSkill;
        }
        set
        {
            _infoAboutCurrentActiveSkill = value;
            if(value == String.Empty) loggerCounter = 1;
            if(loggerCounter >0)
            {
                loggerCounter--;
            }
        }
    }
    public bool IsSkillSelected
    {
        get => God_Script._skill.SkillIsSelected;
        set
        {
            _isSkillSelected = value;
            if (value == true)
            {
                GameObject skillButton = FindLocalisationOfButtonSelectedSkill(Skill);
                switch (Owner)
                {
                    case "Player1":
                        MarkSkillAsSelected(skillButton, Owner);
                        InfoAbutCurrentActiveSkill = $"[{Owner}] [{Skill.GodName}] [{Skill.SkillName}] [{Skill.selectedSkillLevel}]";
                        break;

                    case "Player2":
                        MarkSkillAsSelected(skillButton, Owner);
                        InfoAbutCurrentActiveSkill = $"[{Owner}] [{Skill.GodName}] [{Skill.SkillName}] [{Skill.selectedSkillLevel}]";
                        break;
                }
            }
            else
            {
                switch (Owner)
                {
                    case "Player1":
                        if (LastActivatedBorder_Player1 != null)
                        {
                            InfoAbutCurrentActiveSkill = String.Empty;
                            HideLastSelectedBorderByPlayerName("Player1");
                        }
                        break;

                    case "Player2":
                        if (LastActivatedBorder_Player2 != null)
                        {
                            InfoAbutCurrentActiveSkill = String.Empty;
                            HideLastSelectedBorderByPlayerName("Player2");
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
        if (Card_Script.IsReverseRevelated)
        {
            if (God_Script._skill != null)
            {
                Skill = God_Script._skill;
            }

            if (Skill != null)
            {
                // self triggering set method
                IsSkillSelected = IsSkillSelected;
            }
        }
    }

    void HideLastSelectedBorderByPlayerName(string owner)
    {
        print("hide last border by player " + owner);
        if (owner == "Player1") 
        {
            GameObject skillButton = LastActivatedBorder_Player1.transform.parent.transform.gameObject;
            skillButton.GetComponent<Image>().color = new Color32(255, 255, 255, 128);
            skillButton.GetComponentInChildren<Text>().color = Color.white;

            LastActivatedBorder_Player1.transform.GetComponent<Image>().color = Color.clear;
            LastActivatedBorder_Player1 = null;
        }
        if (owner == "Player2")
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

        var godCardDescription = skillButton.transform.parent;
        var godCard = godCardDescription.transform.parent;
        var godsContainer = godCard.transform.parent;
        var godsManager_Script = godsContainer.GetComponent<GodsManager>();
        int levelOfSkillToCheck = godCard.GetComponent<GodScript>()._skill.selectedSkillLevel;
        Skill skillCurrentlychecking = godCard.GetComponent<GodScript>()._skill;

        if (Skill.CheckIfPlayerHaveEnoughtGoldToUseSkill(Owner , skillCurrentlychecking, levelOfSkillToCheck))
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
        if (owner == "Player1")
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
    public GameObject FindLocalisationOfButtonSelectedSkill(Skill skill) => 
        this.transform.Find("RewersContent")
            .transform.Find($"Skill Level {skill.selectedSkillLevel}")
            .transform.gameObject;
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

        var skills = listOfSelectionControllers.Where(s => s.Skill == skill);
        foreach (var skil in skills)
        {
            skil.IsSkillSelected = false;
        }
    }
    static public bool CheckIfAnyOtherSkillsAlreadySelected(string skillOwner)
    {
        var selectorControllers = GameObject.Find(skillOwner)
            .GetComponentInChildren<GodsManager>().
            _selectionControllers;

        if (selectorControllers.Where(s => s.IsSkillSelected).Any()) 
            return true;
        
        return false;
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