using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GodScript : MonoBehaviour
{
    [SerializeField] Sprite DefaultReversImage;
    [SerializeField] Sprite CardReversImage;
    [SerializeField] GameObject ContainerOfReverseData;
    [SerializeField] int spinningSpeedMultiplifer = 4;
    [SerializeField] bool isCurrentSpinning = false;
    [SerializeField] bool _isRevealed = false;
    public bool IsRevealed
    {
        get => _isRevealed;
        set
        {
            _isRevealed = value;
            ContainerOfReverseData.SetActive(value);
        }
    }
    Sprite _cardMainImage;
    public Sprite CardMainImage1
    {
        get => _cardMainImage;
        set
        {
            _cardMainImage = value;
            GetComponent<Image>().sprite = value;
        }
    }

    [SerializeField] God _godObject;
    public God GodObject
    {
        get => _godObject;
        set
        {
            _godObject = value;
            SelfConfigure(_godObject);
        }
    }
    
    void Start()
    {
        if(GodObject != null)
        {
            SelfConfigure(GodObject);
        }
        else
        {
            DefaultEmptyGodConfiguration();
        }
    }

    void SelfConfigure(God godData)
    {
        this.name = godData.Name;
        this.CardMainImage1 = godData.Image;
        ContainerOfReverseData.transform.Find("Description").GetComponent<TextMeshProUGUI>()
            .SetText($"<size=40><b>{godData.TotemFullName}</b></size>\n<i>{godData.Description}</i>");

        int levelObjectIndex = 1;

        List<string> skillsDescriptionList = godData.GenerateListOFSkillsDescription();

        for (int i = 0; i < 3; i++)
        {
            ContainerOfReverseData.transform.GetChild(levelObjectIndex).GetComponentInChildren<Text>().text = skillsDescriptionList[i];
            levelObjectIndex++;
        }
    }

    void DefaultEmptyGodConfiguration()
    {
        this.name = "god";
        this.CardMainImage1 = DefaultReversImage;
    }

    public void OnClick_SpinCard()
    {
        if (!isCurrentSpinning)
        {
            StartCoroutine(SpinAnimation(spinningSpeedMultiplifer));
        }
    }

    IEnumerator SpinAnimation(int speedMultiplifer)
    {
        isCurrentSpinning = true;
        Sprite spriteToSet = IsRevealed ? CardMainImage1 : CardReversImage;

        print("start krecenia");
        for (int i = 0; i < 90; i += speedMultiplifer)
        {
            this.transform.Rotate(new Vector3(0f, speedMultiplifer, 0f), Space.Self);
            yield return new WaitForFixedUpdate();
        }

        print("Podmiana obrazka na rewers i odblokowanie pzycisków");
        this.GetComponent<Image>().sprite = spriteToSet;
        IsRevealed = !IsRevealed;
        yield return new WaitForSeconds(0.005f);
        for (int i = 90; i > 0; i -= speedMultiplifer)
        {
            //print("spinn in progress current rotate = "+ i);
            this.transform.Rotate(new Vector3(0f, -speedMultiplifer, 0f), Space.Self);
            yield return new WaitForFixedUpdate();
        }

        print("karta gotowa");
        isCurrentSpinning = false;
    }
}
