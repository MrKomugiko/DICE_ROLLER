using System.Net;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class CardScript : MonoBehaviour
{
    GodScript _godTotem;
    Transform _transform;
    Image _cardImage;
    TextMeshProUGUI _cardDescription;
    GodsManager _godsManager;
    CombatManager _combatManager;
    Button _button;
    Button _backgroundButton;
    [SerializeField] bool _isCurrentSpinning = false;
    [SerializeField] bool _isRevealed = false;
    [SerializeField] status _currentstatus = status.standard;
    List<CardScript> Karty { get => _godsManager.ListOfAllCards; }

    [SerializeField] public List<GameObject> _godSkills;
    [SerializeField] Sprite _cardReversImage;
    [SerializeField] Sprite _workInProgressImage;
    [SerializeField] GameObject _cardReversDetailsContainer;
    [SerializeField] int _spinningSpeedMultiplifer = 10;

    public bool IsReverseRevelated
    {
        get => _isRevealed;
        set
        {
            _isRevealed = value;
            if (value)
            { 
                _cardReversDetailsContainer.transform.localScale = new Vector3(1,1,1);
                Currentstatus = status.revealed;
            }

            _cardReversDetailsContainer.SetActive(value); 
        }
    }
    private status Currentstatus
    {
        get => _currentstatus;
        set
        {
            _currentstatus = value;
            _button.onClick.RemoveAllListeners();
            switch (value)
            {
                case status.unfocused:
                    _cardReversDetailsContainer.transform.localScale = new Vector3(0,0,0);
                    _button.onClick.AddListener(() => OnClick_SelectCard());
                    break;

                case status.standard:
                    _cardReversDetailsContainer.transform.localScale = new Vector3(0,0,0);
                    _button.onClick.AddListener(() => OnClick_SelectCard());
                    break;

                case status.selected:
                    _cardReversDetailsContainer.transform.localScale = new Vector3(0,0,0);
                    _button.onClick.AddListener(() => OnClick_SpinCard());
                    break;

                case status.revealed:
                    _cardReversDetailsContainer.transform.localScale = new Vector3(1,1,1);
                    _button.onClick.AddListener(() => OnClick_SpinCard());
                    break;
            }
        }
    }
    public bool IsCurrentSpinning
    {
        get => _isCurrentSpinning;
        set
        {
            _isCurrentSpinning = value;
            _button.interactable = value?false:true;            
        }
    }
    bool isButtonsBlocked;
    float time = 0;
    [SerializeField] float SpeedOfRefreshingButtonCollors = 0.5f;

    void Awake()
    {
        _combatManager = GameObject.Find("FightZone").GetComponent<CombatManager>();
        _backgroundButton = GameObject.Find("GODSkillsWindow").GetComponent<Button>();
        _godsManager = this.GetComponentInParent<GodsManager>();
        _button = this.GetComponent<Button>();
        _godTotem = this.GetComponent<GodScript>();
        _cardImage = this.GetComponent<Image>();
        _transform = this.transform;

        for (int i = 1; i <= 3; i++)
        {
            _godSkills.Add(_cardReversDetailsContainer.transform.Find($"Skill Level {i}").gameObject);
        }

        Currentstatus = Currentstatus;
        _cardDescription = _cardReversDetailsContainer.transform.Find("Description").GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        _cardReversDetailsContainer.transform.localScale = new Vector3(0,0,0);
        BackToNormalSize();
    }
    void FixedUpdate()
    {
        time += Time.deltaTime;

        if (IsReverseRevelated)
        {
            if (time >= this.SpeedOfRefreshingButtonCollors)
            {
                time = time - SpeedOfRefreshingButtonCollors;
                _godsManager.CollorSkillButtonsIfCanBeUsed();
            }
        }

        _backgroundButton.interactable = isAnyCardCurrentlySpinning()?false:true;

        AutoFixFlipCardIfIsRevealedInWrongStatus();

        if (_combatManager.IndexOfCombatAction > 0)
        {
            // zablokowanie skili na początku walki
            if (isButtonsBlocked == false)
            {
                BlockSkillButtons(true);
                isButtonsBlocked = true;
            }
        }
        else if (isButtonsBlocked == true)
        {
            BlockSkillButtons(false);
            isButtonsBlocked = false;
        }
    }
    
    private bool isAnyCardCurrentlySpinning()
    {
        foreach (var karta in Karty)
        {
            if (karta.IsCurrentSpinning) return true;
        }
        return false;
    }   
    public void AutoFixFlipCardIfIsRevealedInWrongStatus()
    {
        if (IsReverseRevelated)
        {
            if (Karty.Where(k => k.Currentstatus == status.selected || k.Currentstatus == status.standard).FirstOrDefault() != null)
            {
                OnClick_SpinCard();
            }
        }
    }
    public void SetDescription(string descriptionTextValue)
    {
        _cardDescription.SetText(descriptionTextValue);
    }
    public void SetSkillDescription(int skillLevel, string skillDescription)
    {
        _godSkills[skillLevel - 1].GetComponentInChildren<Text>().text = skillDescription;
    }
    public void AttachSkillsFunctionToButtons(int skillLevel, Skill skill)
    {
        _godSkills[skillLevel - 1].GetComponentInChildren<Button>().onClick.AddListener(() => skill.TrySelectSkill(skillLevel, _godsManager.ownerName, skill.God));
    }
    void Resize(float x, float y)
    {
        if(x == 270.0f && y == 443.16f && IsReverseRevelated)
        {
             _cardReversDetailsContainer.transform.localScale = new Vector3(0.9f,0.55f,1);
        }
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
    }
    IEnumerator ChangeColor(Color32 color)
    {
        //yield return new WaitWhile(() => NewColorChangingInPRocess);
        for (float i = 0f; i <= 1; i += 0.1f)
        {
            _cardImage.color = Color.Lerp(_cardImage.color, color, i);
            yield return new WaitForSeconds(0.02f);
        }
        //NewColorChangingInPRocess = false;
    }
    IEnumerator SpinAnimation(int speedMultiplifer)
    {
        IsCurrentSpinning = true;
        if (Currentstatus == status.revealed) 
        {
            Currentstatus = status.selected;
        }
        
        for (int i = 0; i < 90; i += speedMultiplifer)
        {
            if(IsReverseRevelated && Currentstatus == status.selected) 
                _cardReversDetailsContainer.transform.localScale = new Vector3(1,1,1);
            
            _transform.Rotate(new Vector3(0f, speedMultiplifer, 0f), Space.Self);
            yield return new WaitForSeconds(0.01f);
        }

        IsReverseRevelated = !IsReverseRevelated;
        ChangeCardSprite(IsReverseRevelated); 

        for (int i = 90; i > 0; i -= speedMultiplifer)
        {
            _transform.Rotate(new Vector3(0f, -speedMultiplifer, 0f), Space.Self);
            yield return new WaitForSeconds(0.01f);
        }

        IsCurrentSpinning = false;
    }
    void ChangeCardSprite(bool isCardRevealed)
    {
        _cardImage.sprite = IsReverseRevelated ?_godTotem.GodObject.CardReverseImage: _godTotem.GodTotemMainImage;
    }
    IEnumerator BackToNormalSize()
    {
        Resize(327.4f, 537.3f);
        switch (Currentstatus)
        {
            case status.revealed:
            // odkręcenie karty
                _cardReversDetailsContainer.transform.localScale = new Vector3(0.9f,0.55f,1);
                yield return StartCoroutine(SpinAnimation(_spinningSpeedMultiplifer));
                break;

            // case status.selected:
            // brak dodatkowych akcji , tylko zmniejszenie

            case status.unfocused: 
            // rozjasnienie karty
                yield return StartCoroutine(ChangeColor(new Color32(255, 255, 255, 255)));
                break;
        }
        Currentstatus = status.standard;
    }
    IEnumerator SetCardAsSelectedMode()
    {
        Resize(475.6f, 780.67f);
        yield return StartCoroutine(ChangeColor(new Color32(255, 255, 255, 255)));
    }
    IEnumerator SetCardAsUnfocusedMode()
    {
        Resize(270.0f, 443.16f);
        yield return StartCoroutine(ChangeColor(new Color32(75, 75, 75, 255)));
    }
    public void BlockSkillButtons(bool value)
    {
        AndroidLogger.Log("Combat started, skill buttons is now Dissabled",AndroidLogger.GetPlayerLogColor(_godsManager.ownerName));
        foreach (var skillButton in _godSkills)
        {
            skillButton.GetComponent<Button>().interactable = !value;
        }
    }

    #region FUNCTIONAL BUTTONS
    [ContextMenu("Turn card to 'normal' state.")] public void OnClick_SetCardToNormalMode()
    {
        StartCoroutine(BackToNormalSize());
    }
    [ContextMenu("Select Card, turn off rest of them.")] public void OnClick_SelectCard()
    {
        OnClick_ChangeAllCardsToUnfocusedMode();
        OnClick_ChangeCardToSelectedMode();
    }
    [ContextMenu("Turn off All cards.")] public void OnClick_ChangeAllCardsToUnfocusedMode()
    {
        foreach (CardScript card in Karty)
        {
            card.OnClick_ChangeCardToUnfocusedMode();
        }
    }
    [ContextMenu("Turn card to 'selected' state.")] public void OnClick_ChangeCardToSelectedMode()
    {
        Currentstatus = status.selected;
        StartCoroutine(SetCardAsSelectedMode());
    }
    [ContextMenu("Spin a single Card")] public void OnClick_SpinCard()
    {
        if (!IsCurrentSpinning) StartCoroutine(SpinAnimation(_spinningSpeedMultiplifer));
    }
    [ContextMenu("Turn card to 'unfocused' state.")] public void OnClick_ChangeCardToUnfocusedMode()
    {
        Currentstatus = status.unfocused;
        StartCoroutine(SetCardAsUnfocusedMode());
    }
    #endregion
}
