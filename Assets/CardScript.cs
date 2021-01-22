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
    Button _button;
    Button _backgroundButton;
    bool NewColorChangingInPRocess = false;
    bool _isCurrentSpinning = false;
    bool _isRevealed = false;
    status _currentstatus = status.standard;
    public Sprite DefaultEmptyImage { get => _defaultEmptyImage; }
    List<CardScript> Karty { get => _godsManager.ListOfAllCards; }

    [SerializeField] public List<GameObject> _godSkills;
    [SerializeField] Sprite _cardReversImage;
    [SerializeField] Sprite _defaultEmptyImage;
    [SerializeField] GameObject _cardReversDetailsContainer;
    [SerializeField] int _spinningSpeedMultiplifer = 4;

    public bool IsReverseRevelated
    {
        get => _isRevealed;
        set
        {
            if (value) { Currentstatus = status.revealed; }
            _isRevealed = value;
            _cardReversDetailsContainer.SetActive(value);
        }
    }
    private status Currentstatus
    {
        get => _currentstatus;
        set
        {
            _currentstatus = value;
            switch (value)
            {
                case status.unfocused:
                    _button.onClick.RemoveAllListeners();
                    _button.onClick.AddListener(() => OnClickSelectCard());
                    break;

                case status.standard:
                    _button.onClick.RemoveAllListeners();
                    _button.onClick.AddListener(() => OnClickSelectCard());
                    break;

                case status.selected:
                    _button.onClick.RemoveAllListeners();
                    _button.onClick.AddListener(() => OnClick_SpinCard());
                    break;

                case status.revealed:
                    _button.onClick.RemoveAllListeners();
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
            if (value)
            {
                _button.interactable = false;
            }
            else
            {
                _button.interactable = true;
            }
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

    void Awake()
    {
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
    void FixedUpdate()
    {
        if (isAnyCardCurrentlySpinning()) { _backgroundButton.interactable = false; } else { _backgroundButton.interactable = true; }
        AutoFixFlipCardIfIsRevealedInWrongStatus();
    }

    public void AutoFixFlipCardIfIsRevealedInWrongStatus()
    {
        if (IsReverseRevelated)
        {
            if (Karty.Where(k => k.Currentstatus == status.selected || k.Currentstatus == status.standard).FirstOrDefault() != null)
            {
                OnClick_SpinCard();
                this.Currentstatus = status.unfocused;
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

    void Resize(float x, float y)
    {
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
    }
    IEnumerator ChangeColor(Color32 color)
    {
        yield return new WaitWhile(() => NewColorChangingInPRocess);
        for (float i = 0f; i <= 1; i += 0.1f)
        {
            _cardImage.color = Color.Lerp(_cardImage.color, color, i);
            yield return new WaitForSeconds(0.02f);
        }
        NewColorChangingInPRocess = false;
    }
    IEnumerator SpinAnimation(int speedMultiplifer)
    {
        IsCurrentSpinning = true;

        Sprite spriteToSet = IsReverseRevelated ? _godTotem.GodTotemMainImage : _cardReversImage;

        for (int i = 0; i < 90; i += speedMultiplifer)
        {
            _transform.Rotate(new Vector3(0f, speedMultiplifer, 0f), Space.Self);
            yield return new WaitForFixedUpdate();
        }

        _cardImage.sprite = spriteToSet;
        IsReverseRevelated = !IsReverseRevelated;

        for (int i = 90; i > 0; i -= speedMultiplifer)
        {
            _transform.Rotate(new Vector3(0f, -speedMultiplifer, 0f), Space.Self);
            yield return new WaitForFixedUpdate();
        }

        IsCurrentSpinning = false;
    }
    IEnumerator BackToNormalSize()
    {
        switch (Currentstatus)
        {
            case status.revealed:
                yield return StartCoroutine(SpinAnimation(_spinningSpeedMultiplifer));
                Resize(327.4f, 537.3f);
                Currentstatus = status.standard;
                break;

            case status.selected:
                Resize(327.4f, 537.3f);
                Currentstatus = status.standard;
                break;

            case status.unfocused:
                Resize(327.4f, 537.3f);
                yield return StartCoroutine(ChangeColor(new Color32(255, 255, 255, 255)));
                Currentstatus = status.standard;
                break;
        }
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

    #region BUTTONS
    public void OnClick_SetCardToNormalMode()
    {
        StartCoroutine(BackToNormalSize());
    }
    public void OnClickSelectCard()
    {
        OnClick_ChangeAllCardsToUnfocusedMode();
        OnClick_ChangeCardToSelectedMode();
    }
    public void OnClick_ChangeAllCardsToUnfocusedMode()
    {
        foreach (CardScript card in Karty)
        {
            card.OnClick_ChangeCardToUnfocusedMode();
        }
    }
    public void OnClick_ChangeCardToSelectedMode()
    {
        Currentstatus = status.selected;
        StartCoroutine(SetCardAsSelectedMode());
    }
    public void OnClick_SpinCard()
    {
        if (!IsCurrentSpinning)
        {
            StartCoroutine(SpinAnimation(_spinningSpeedMultiplifer));
        }
    }
    public void OnClick_ChangeCardToUnfocusedMode()
    {
        Currentstatus = status.unfocused;
        StartCoroutine(SetCardAsUnfocusedMode());
    }
    #endregion
}
