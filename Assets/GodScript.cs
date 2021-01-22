using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodScript : MonoBehaviour
{
    God _godData;
    CardScript _card;
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
            print("przypisanie boga:"+value.Name);
            _godData = value;
        }
    }

    void Awake()
    {
        _card = GetComponent<CardScript>();
        if(_godData == null) DefaultEmptyGodConfiguration();
    }

    public void SelfConfigure(God godData)
    {   
        _godData = godData;
        this.name = godData.Name;
        this.GodTotemMainImage = godData.Image;
        _card.SetDescription($"<size=40><b>{godData.TotemFullName}</b></size>\n<i>{godData.Description}</i>");

        int skillLevel = 1;

        List<string> skillsDescriptionList = godData.GenerateListOFSkillsDescription();

        for (int i = 0; i < 3; i++)
        {
            _card.SetSkillDescription(skillLevel, skillsDescriptionList[i]);
            skillLevel++;
        }
    }
    void DefaultEmptyGodConfiguration()
    {
        this.name = "unnamed_god";
        this.GodTotemMainImage = _card.DefaultEmptyImage;
    }

    
}
