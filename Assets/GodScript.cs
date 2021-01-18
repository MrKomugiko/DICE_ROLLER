using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GodScript : MonoBehaviour
{
    [SerializeField] status _currentstatus;
    [SerializeField] God _godData;
    [SerializeField] Sprite _defaultEmptyImage;
    [SerializeField] Sprite _cardMainImage;
    [SerializeField] Sprite _cardReversImage;  
    [SerializeField] TextMeshProUGUI _cardDescription;
    [SerializeField] GameObject _cardReversDetailsContainer;
    [SerializeField] int _spinningSpeedMultiplifer = 4;
    [SerializeField] bool isCurrentSpinning = false;
    [SerializeField] bool _isRevealed = false;

    public bool IsRevealed
    {
        get => _isRevealed;
        set
        {
            _isRevealed = value;
            _cardReversDetailsContainer.SetActive(value);
        }
    }  
    public Sprite CardMainImage
    {
        get => _cardMainImage;
        set
        {
            _cardMainImage = value;
            GetComponent<Image>().sprite = value;
        }
    }

    private Image _cardImage;
    private Transform _transform;
 
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
        if(_godData == null) DefaultEmptyGodConfiguration();
        _cardImage = this.GetComponent<Image>();
        _transform = this.transform;
        _cardDescription = _cardReversDetailsContainer.transform.Find("Description").GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
    }
[ContextMenu("Self Configuration")]
    public void SelfConfigure()
    {
        this.name = _godData.Name;
        this.CardMainImage = _godData.Image;
        _cardDescription.SetText($"<size=40><b>{_godData.TotemFullName}</b></size>\n<i>{_godData.Description}</i>");

        int levelObjectIndex = 1;

        List<string> skillsDescriptionList = _godData.GenerateListOFSkillsDescription();

        for (int i = 0; i < 3; i++)
        {
            _cardReversDetailsContainer.transform.GetChild(levelObjectIndex).GetComponentInChildren<Text>().text = skillsDescriptionList[i];
            levelObjectIndex++;
        }
    }
    void DefaultEmptyGodConfiguration()
    {
        this.name = "unnamed_god";
        this.CardMainImage = _defaultEmptyImage;
    }

[ContextMenu("Spin card test")]
    public void OnClick_SpinCard()
    {
        _transform = this.transform;
        if (!isCurrentSpinning && _currentstatus==status.selected)
        {
            StartCoroutine(SpinAnimation(_spinningSpeedMultiplifer));
        }
    }
    
    IEnumerator SpinAnimation(int speedMultiplifer)
    {
        isCurrentSpinning = true;
        Sprite spriteToSet = IsRevealed ? CardMainImage : _cardReversImage;

        print("start krecenia");
        for (int i = 0; i < 90; i += speedMultiplifer)
        {
            _transform.Rotate(new Vector3(0f, speedMultiplifer, 0f), Space.Self);
            yield return new WaitForFixedUpdate();
        }

        print("Podmiana obrazka na rewers i odblokowanie pzycisków");
        _cardImage.sprite = spriteToSet;
        IsRevealed = !IsRevealed;
        
        for (int i = 90; i > 0; i -= speedMultiplifer)
        {
            //print("spinn in progress current rotate = "+ i);
            _transform.Rotate(new Vector3(0f, -speedMultiplifer, 0f), Space.Self);
            yield return new WaitForFixedUpdate();
        }

        isCurrentSpinning = false;
    }

    IEnumerator SetCardAsSelectedMode()
    {
        // metoda odpowiedzialna za zmiane statusu na "Selected" w tym czasie powiększenie karty 
        // nastepnie wykonanie SetCardAsUnfocused dla pozostałych kart
        
        yield return new WaitForSeconds(1f);
    }

    // TODO: SetCardAsUnfocusedMode()
    IEnumerator SetCardAsSetCardAsUnfocusedModeSelectedMode()
    {
        
        yield return new WaitForSeconds(1f);
    }
    
    // TODO: SetCardAsStandardMode()
    IEnumerator SetCardAsStandardMode()
    {
       
        yield return new WaitForSeconds(1f);
    }
        enum status 
    {
        standard,
        selected,
        unfocused
    }

}
