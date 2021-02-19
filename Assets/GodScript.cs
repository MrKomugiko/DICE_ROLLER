using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodScript : MonoBehaviour
{
    [SerializeField] public string ownerName;
    public God _godData;
    public CardScript _card;
    [SerializeField] public Skill _skill;
    [SerializeField] Sprite _godTotemImage;
    public Sprite GodTotemMainImage
    {
        get => _godTotemImage;
        set
        {
            _godTotemImage = value;
            GetComponent<Image>().sprite = value;
        }
    }
    public God GodObject
    {
        get => _godData;
        set
        {
            _godData = value;
        }
    }
    
    void Awake()
    {
        _card = GetComponent<CardScript>();
    }
    public void SelfConfigure(God godData)
    {
        ownerName = this.transform.parent.transform.parent.name; // wyjscie z katalogu i wyjscie do nad katalogu playe1 lub player2
        _godData = godData;
        this.name = godData.Name;
        this.GodTotemMainImage = godData.IsGodPlayable == true ? godData.MainImage : godData.WorkInProgressImage;
        _card.SetDescription($"<size=40><b>{godData.TotemFullName}</b></size>\n<i>{godData.Description}</i>");

        int skillLevel = 1;

        List<string> skillsDescriptionList = godData.GenerateListOFSkillsDescription();

        AttachSkill(_godData);

        for (int i = 0; i < 3; i++)
        {
            _card.SetSkillDescription(skillLevel, skillsDescriptionList[i]);

            if (_skill != null)
            {
                _card.AttachSkillsFunctionToButtons(skillLevel, _skill);
            }
            skillLevel++;
        }
    }
    void AttachSkill(God godData)
    {
        _skill = Skill.GetGodSkillByGodID(godData.Index, godData, ownerName);
    }
}

