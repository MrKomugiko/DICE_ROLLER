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
    [SerializeField] List<GameObject> _godSkills;

    public bool IsReverseRevelated
    {
        get => _isRevealed;
        set
        {

            if(value) {Currentstatus = status.revealed;}
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
            print(name+" changed status to "+Currentstatus.ToString());
        }
    }

    public Sprite DefaultEmptyImage { get => _defaultEmptyImage;}

    void Awake()
    {
        _godTotem = this.GetComponent<GodScript>(); 
        _cardImage = this.GetComponent<Image>();
        _transform = this.transform;
        
        for (int i = 1; i <= 3; i++)        
        {
            _godSkills.Add(_cardReversDetailsContainer.transform.Find($"Skill Level {i}").gameObject);    
        }

        _cardDescription = _cardReversDetailsContainer.transform.Find("Description").GetComponent<TextMeshProUGUI>();
    }

    [ContextMenu("Spin a card [only if is 'selected']")]
    public void OnClick_SpinCard()
    {
        if (!_isCurrentSpinning && (Currentstatus != status.unfocused || Currentstatus != status.standard))
        {
            StartCoroutine(SpinAnimation(_spinningSpeedMultiplifer));
        }
    }

 [ContextMenu("test odwrotu karty i zmiana statusu na normal")]
    public void OnClick_ResetCardToStandard()
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
                yield return StartCoroutine(Resize(10f,20f));

                print("Zamiana statusu karty na 'standard'");
                Currentstatus = status.standard;
            break;

            case status.selected:
                print("---------- Wybana karta jest Odwrócona wizerunkiem totemu na wierzch trzeba ją tylko zmiejszyc ----------");

                print("Trwa zmiana rozmiaru");
                yield return StartCoroutine(Resize(10f,20f));

                print("Zamiana statusu karty na 'standard'");
                Currentstatus = status.standard;
            break;

            case status.standard:
                print("---------- Wybana karta jest juz ustawiona normalnie nie trzeba nic robic----------");
            break;

            case status.unfocused:
                print("---------- Wybana karta jest w trybie poza focusem, trzeba ją zwiekszych i rozjasnic ----------");

                print("Trwa zmiana rozmiaru");
                yield return StartCoroutine(Resize(10f,20f));

                print("zmiana koloru na jasniejszy");
                StartCoroutine(ChangeColor(new Color32(255,255,255,255)));

                print("Zamiana statusu karty na 'standard'");
                Currentstatus = status.standard;
            break;
        }
        //yield return new WaitUntil(Delegate_CheckingIfCardSpinning);
        print("KARTA WROCILA DO STANDARDU");
    }
    IEnumerator ChangeColor(Color32 color)
    {
        for (float i = 0f; i <= 1; i += 0.1f)
        {
            this.GetComponent<Image>().color = Color.Lerp(this.GetComponent<Image>().color, color, i);
            yield return new WaitForSeconds(0.05f);
        }
        print("koniec zmiany koloru");
    }
    IEnumerator Resize(float x, float y)
    {
        print("START coroutin resize");

        // obliczenie aktualnej wielkosci
        
        // potem obliczenie roznicy miedzy aktualna i wymaganą

        // podzielic ta wartosc na jakąś równą czesc zeby to zapętlic w animacje

        // pretla for z przerwami co 0.05s
        yield return new WaitForSeconds(0.1f);
        
        print("resize is completed");
    }
    public void SetDescription(string descriptionTextValue)
    {
        _cardDescription.SetText(descriptionTextValue);
    }
    public void SetSkillDescription(int skillLevel, string skillDescription)
    {
        _godSkills[skillLevel-1].GetComponentInChildren<Text>().text = skillDescription;
    }
    IEnumerator SpinAnimation(int speedMultiplifer)
    {
        _isCurrentSpinning = true;
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

        _isCurrentSpinning = false;

        yield return true;
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
}
