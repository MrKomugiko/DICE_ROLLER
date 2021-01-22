using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using System.Net.Mail;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public partial class CardScript : MonoBehaviour
{
    GodScript _godTotem;
    Transform _transform;
    [SerializeField] Sprite _cardReversImage;
    Image _cardImage;
    [SerializeField] Sprite _defaultEmptyImage;
    [SerializeField] GameObject _cardReversDetailsContainer;
    TextMeshProUGUI _cardDescription;

    [SerializeField] int _spinningSpeedMultiplifer = 4;
    [SerializeField] bool _isCurrentSpinning = false;
    [SerializeField] bool _isRevealed = false;
    [SerializeField] status _currentstatus = status.standard;
    Button _backgroundButton;
    [SerializeField] List<GameObject> _godSkills;
    GodsManager _godsManager;
    Button _button;
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
            // print(name + " changed status to " + Currentstatus.ToString());
            switch (value)
            {
                case status.unfocused:
                    _button.onClick.RemoveAllListeners();
                    _button.onClick.AddListener(() => OnClickSelectCard());
                    break;

                case status.standard:
                    _button.onClick.RemoveAllListeners();
                    // jeżeli jest bez focusu następny click ją aktywuje na selected
                    // gdy karta jest w trybie standard następny click ją aktywuje na selected
                    _button.onClick.AddListener(() => OnClickSelectCard());
                    break;

                case status.selected:
                    _button.onClick.RemoveAllListeners();
                    // gdy karta jest wybrana, nastepny click ją odwróci 
                    _button.onClick.AddListener(() => OnClick_SpinCard());
                    break;

                case status.revealed:
                    _button.onClick.RemoveAllListeners();
                    // gdy karta jest odwrocona, nastepny click ją odwróci spowrotem
                    _button.onClick.AddListener(() => OnClick_SpinCard());
                    break;
            }
        }
    }
    [ContextMenu("Change card to 'SELECTED' mode AND every else to 'UNFOCUSED")]
    public void OnClickSelectCard()
    {
        OnClick_ChangeAllCardsToUnfocusedMode();
        OnClick_ChangeCardToSelectedMode();
    }
    public Sprite DefaultEmptyImage { get => _defaultEmptyImage; }
    private List<CardScript> Karty { get => _godsManager.ListOfAllCards;}
    public bool IsCurrentSpinning 
    { 
        get => _isCurrentSpinning; 
        set 
        {
            _isCurrentSpinning = value; 
            if(value)
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
        foreach(var karta in Karty){
            if(karta.IsCurrentSpinning) return true;
        }
        return false;
    }

    void FixedUpdate()
    {
        if(isAnyCardCurrentlySpinning()){_backgroundButton.interactable = false;}else{_backgroundButton.interactable = true;}
        AutoFixFlipCardIfIsRevealedInWrongStatus();
    }

    public void AutoFixFlipCardIfIsRevealedInWrongStatus()
    {
        if(IsReverseRevelated)
        {
            if(Karty.Where(k=>k.Currentstatus == status.selected || k.Currentstatus == status.standard).FirstOrDefault()!=null)
            {   
                OnClick_SpinCard();
                this.Currentstatus = status.unfocused;
            }
        }
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

        _cardDescription = _cardReversDetailsContainer.transform.Find("Description").GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        Currentstatus = Currentstatus;
    }

    [ContextMenu("Change 'ALL' cards to 'UNFOCUSED' mode.")]
    public void OnClick_ChangeAllCardsToUnfocusedMode()
    {
        print("OnClick_ChangeAllCardsToUnfocusedMode");
        //print("liczba karty w folderze to "+ karty.Count);
        foreach (CardScript card in Karty)
        {
            card.OnClick_ChangeCardToUnfocusedMode();
        }
    }

    [ContextMenu("Change card to 'SELECTED' mode.")]
    public void OnClick_ChangeCardToSelectedMode()
    {
        print("OnClick_ChangeCardToSelectedMode");
        Currentstatus = status.selected;
        StartCoroutine(SetCardAsSelectedMode());
    }

    [ContextMenu("Spin a card [only if is 'selected']")]
    public void OnClick_SpinCard()
    {
        if (!IsCurrentSpinning)
        {
            StartCoroutine(SpinAnimation(_spinningSpeedMultiplifer));
        }
    }
    [ContextMenu("change card to unfocused ")]

    public void OnClick_ChangeCardToUnfocusedMode()
    {
        print("OnClick_ChangeCardToUnfocusedMode");
        Currentstatus = status.unfocused;
        StartCoroutine(SetCardAsUnfocusedMode());
    }

    [SerializeField] bool NewColorChangingInPRocess = false;
    IEnumerator ChangeColor(Color32 color)
    {
        yield return new WaitWhile(()=>NewColorChangingInPRocess);
        for (float i = 0f; i <= 1; i += 0.1f)
        {
            _cardImage.color = Color.Lerp(_cardImage.color, color, i);
            yield return new WaitForSeconds(0.02f);
        }
        NewColorChangingInPRocess=false;
    }

    void Resize(float x, float y)
    {
        print("START coroutin resize");

        this.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);

        // obliczenie aktualnej wielkosci

        // potem obliczenie roznicy miedzy aktualna i wymaganą

        // podzielic ta wartosc na jakąś równą czesc zeby to zapętlic w animacje

        // pretla for z przerwami co 0.05s
        
        print("resize is completed");
    }
    public void SetDescription(string descriptionTextValue)
    {
        _cardDescription.SetText(descriptionTextValue);
    }
    public void SetSkillDescription(int skillLevel, string skillDescription)
    {
        _godSkills[skillLevel - 1].GetComponentInChildren<Text>().text = skillDescription;
    }

    // bool Delegate_SpinningChecker()
    // {
    //     if(_isCurrentSpinning) {
    //         return true;
    //     }else{
    //         return false;
    //     }
    // }

    IEnumerator SpinAnimation(int speedMultiplifer)
    {
        IsCurrentSpinning = true;
        Sprite spriteToSet = IsReverseRevelated ? _godTotem.GodTotemMainImage : _cardReversImage;

        print("start krecenia");
        for (int i = 0; i < 90; i += speedMultiplifer)
        {
            _transform.Rotate(new Vector3(0f, speedMultiplifer, 0f), Space.Self);
            yield return new WaitForFixedUpdate();
        }

        print("Podmiana obrazka na rewers i odblokowanie pzycisków");
        _cardImage.sprite = spriteToSet;
        IsReverseRevelated = !IsReverseRevelated;

        for (int i = 90; i > 0; i -= speedMultiplifer)
        {
            //print("spinn in progress current rotate = "+ i);
            _transform.Rotate(new Vector3(0f, -speedMultiplifer, 0f), Space.Self);
            yield return new WaitForFixedUpdate();
        }

        IsCurrentSpinning = false;

        yield return true;
    }

    public void OnClick_SetCardToNormalMode()
    {
        StartCoroutine(BackToNormalSize());
    }

    IEnumerator BackToNormalSize()
    {
        print("START coroutin Back to normal");
        switch (Currentstatus)
        {
            case status.revealed:
                print("---------- Wybana karta jest Odwrócona skillami na wierzch trzeba ją odwrocic i zmniejszyc ----------");

                print("Trwa odwracanie karty totemem na wierzch");
                yield return StartCoroutine(SpinAnimation(_spinningSpeedMultiplifer));

                print("Trwa zmiana rozmiaru");
                //StopCoroutine("Resize");
                Resize(327.4f, 537.3f);

                print("Zamiana statusu karty na 'standard'");
                Currentstatus = status.standard;
                break;

            case status.selected:
                print("---------- Wybana karta jest Odwrócona wizerunkiem totemu na wierzch trzeba ją tylko zmiejszyc ----------");

                print("Trwa zmiana rozmiaru");
                //StopCoroutine("Resize");
                Resize(327.4f, 537.3f);

                print("Zamiana statusu karty na 'standard'");
                Currentstatus = status.standard;
                break;

            case status.standard:
                print("---------- Wybana karta jest juz ustawiona normalnie nie trzeba nic robic----------");
                break;

            case status.unfocused:
                print("---------- Wybana karta jest w trybie poza focusem, trzeba ją zwiekszych i rozjasnic ----------");

                print("Trwa zmiana rozmiaru");
                //StopCoroutine("Resize");
                Resize(327.4f, 537.3f);

                print("zmiana koloru na jasniejszy");
      
                yield return StartCoroutine(ChangeColor(new Color32(255, 255, 255, 255)));

                print("Zamiana statusu karty na 'standard'");
                Currentstatus = status.standard;
                break;
        }
        print("KARTA WROCILA DO STANDARDU");
    }

    IEnumerator SetCardAsSelectedMode()
    {
        //StopCoroutine("Resize");
        Resize(475.6f, 780.67f);

        yield return StartCoroutine(ChangeColor(new Color32(255, 255, 255, 255)));
    }
    IEnumerator SetCardAsUnfocusedMode()
    {

        Resize(270.0f, 443.16f);

        yield return StartCoroutine(ChangeColor(new Color32(75, 75, 75, 255)));
    }
}
