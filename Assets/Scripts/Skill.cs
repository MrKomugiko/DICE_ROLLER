using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[SerializeField]
public class Skill
{
    static public List<Skill> ListOfSkills = new List<Skill>();
    public TestDelegate methodToCall;
    public int selectedSkillLevel;
    string selectedCastingPlayer;
    bool _skillIsSelected;
    protected GameManager GM_Script = GameObject.Find("GameManager").GetComponent<GameManager>();
    public God God { get; set; }
    public int ID { get; set; }
    public string GodName { get; set; }
    public string SkillName { get; set; }
    public bool SkillIsSelected
    {
        get => _skillIsSelected;
        set
        {
            _skillIsSelected = value;

            if (value)
            {
                methodToCall = UseSkill;
            }
            else
            {
                methodToCall = null;
            }
        }
    }

    protected string OwnerName { get; set; }
    public delegate void TestDelegate(int skillLevel, string castingPlayer);
  
    public void LastSelectedSkillReadyToUse()
    {
        string color = OwnerName == "Player1" ? "green" : "red";
        if (methodToCall == null)
        {
            AndroidLogger.Log("Skill not beed selected to use.");
            return;
        }

        AndroidLogger.Log("Execute " + SkillName + "[" + selectedSkillLevel + " level], by player  " + selectedCastingPlayer, color);

        methodToCall(selectedSkillLevel, selectedCastingPlayer);
        SkillIsSelected = false;
    }
    public void TrySelectSkill(int skillLevel, string castingPlayer, God god)
    {
        if (!SelectionController.CheckIfAnyOtherSkillsAlreadySelected(castingPlayer))
        {
            SelectSkill(skillLevel, castingPlayer, god);
        }
        else
        {
            if (CheckIfItsDoubleSelectPreviousSkill(skillLevel, castingPlayer, god))
            {
                Debug.Log("anulowanie wyboru skilla - przez ponownejego jego wybranie");
                UnSelectAnySelectedSkill();
                return;
            }

            UnSelectAnySelectedSkill();

            SelectSkill(skillLevel, castingPlayer, god);
        }
    }
    protected virtual void UseSkill(int skillLevel, string castingPlayer)
    {
        AndroidLogger.Log("no skill to use ;d");
    }
    bool CheckIfItsDoubleSelectPreviousSkill(int newSelectedskillLevel, string castingPlayer, God newSelectedGod)
    {
        Skill recentSelectedSkill = SelectionController.GetSelectedSkill(castingPlayer);

        int recentSelectedSkillLevel = recentSelectedSkill.selectedSkillLevel;
        God recentSelectedGod = recentSelectedSkill.God; ;


        if (recentSelectedSkillLevel == newSelectedskillLevel)
        {
            if (recentSelectedGod == newSelectedGod)
            {
                Debug.Log("Zmiana skilla tego samego Boga");
                return true;
            }
        }
        if (recentSelectedGod != newSelectedGod)
        {
            Debug.Log("Wybranie skilla od innego Boga");
            return false;
        }

        return false;
    }
    void UnSelectAnySelectedSkill()
    {
        Debug.Log("unselect any selected skill before");
        Skill recentSelectedSkill = SelectionController.GetSelectedSkill(OwnerName);
        SelectionController.UnselectControllerWhoContainSkill(recentSelectedSkill, OwnerName);
        recentSelectedSkill.SkillIsSelected = false;

        Debug.Log($"[{recentSelectedSkill.OwnerName}]\t[{recentSelectedSkill.GodName}]\t[{recentSelectedSkill.selectedSkillLevel}]\t[OLD]");

        if (OwnerName == "Player1")
        {
            Debug.Log("unselect p1");
            GM_Script.Player1GodSkillWindow.GetComponent<GodsManager>().CollorSkillButtonsIfCanBeUsed();
        }
        if (OwnerName == "Player2")
        {
            Debug.Log("unselect p2");
            GM_Script.Player2GodSkillWindow.GetComponent<GodsManager>().CollorSkillButtonsIfCanBeUsed();
        }
    }
    void SelectSkill(int skillLevel, string castingPlayer, God god)
    {
        if (!SkillIsSelected)
        {
            selectedSkillLevel = skillLevel;
            selectedCastingPlayer = castingPlayer;

            SkillIsSelected = true;

            Debug.Log($"[{castingPlayer}]\t[ {GodName} ]\t[ {selectedSkillLevel} lvl.]\t[NEW SELECT]");
        }
    }
    public int GetValueForSkillLevel(int skillLevel)
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
    public int GetGoldCostForSkillLevel(int skillLevel)
    {
        int skillCost = 0;

        switch (skillLevel)
        {
            case 1:
                skillCost = God.LevelCost;
                break;

            case 2:
                skillCost = God.Leve2Cost;
                break;

            case 3:
                skillCost = God.Leve3Cost;
                break;
        }

        return skillCost;
    }
    
    static public Skill GetGodSkillByID(int id, God godData, string ownerName)
    {
        GenerateGodsSkillScripts(godData, ownerName);

        return ListOfSkills.Where(s => s.ID == id && s.OwnerName == ownerName).First();
    }
    static public void GenerateGodsSkillScripts(God godData, string ownerName)
    {
        switch (godData.name)
        {
            case "Bragi":
                new BragiSkill(godData, ownerName);
                break;
            case "Idun":
                new IdunSkill(godData, ownerName);
                break;
            case "Thor":
                new ThorSkill(godData, ownerName);
                break;
            case "Odin":
                new OdinSkill(godData, ownerName);
                break;
        }

        Debug.Log("Aktualna liczbaskilli w pamieci :" + ListOfSkills.Count);
        AndroidLogger.Log("Current avaiable skills in memory :" + ListOfSkills.Count);
    }
    static public bool CheckIfPlayerHaveEnoughtGoldToUseSkill(string player, Skill skill, int level)
    {
        int currentPlayerGold = GameManager.GetPlayerGoldValue(player);
        if (skill.GetGoldCostForSkillLevel(level) > currentPlayerGold) return false;

        return true;
    }
}







