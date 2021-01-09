using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DiceRoller_Console;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DiceRollScript : MonoBehaviour
{
    [SerializeField] public int DiceNumber;
    public static int WIELKOSC_KOSCI = 8;
    public static int ILOSC_RZUTOW = 100;

    [SerializeField] public bool DiceSlotIsLocked;
    [SerializeField] private bool rollingIsCompleted;
    [SerializeField] public List<Sprite> listaDiceImages;
    [SerializeField] private Image _diceImage;

    [SerializeField] bool _isSentToBattlefield;
    [SerializeField] bool _isAbleToPickup;
    [SerializeField] private bool _lockDiceOnBattlefield;

    [SerializeField]
    public bool IsAbleToPickup
    {
        get => _isAbleToPickup;
        set
        {
            try
            {
                _isAbleToPickup = value;
                if (this.GetComponentInParent<DiceManager>().gameObject.name == "DiceHolder")
                {
                    if (_isAbleToPickup == false)
                    {
                        this.GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        this.GetComponent<Button>().interactable = true;
                    }
                }
            }
            catch (System.Exception)
            {
                // print("Dice in battleground dont have buttons");
            }
        }
    }

    [SerializeField] public bool LockDiceOnBattlefield { get => _lockDiceOnBattlefield; set => _lockDiceOnBattlefield = value; }

    public void OnClick_TEST_WrocKoscZpolaBitwy()
    {
        // sprawdzenie blokady na kostce "matce" na ręce wyszukanej po numeze kości
        var myContainer = this.GetComponent<DiceActionScript>().transform.parent;
        DiceRollScript originDice;
//        print(myContainer.name);


        if(myContainer.name == "Player1Dices")
        {
            var diceOriginContainer = GameObject.Find("Player1").transform.Find("DiceHolder").transform.GetComponentsInChildren<DiceRollScript>();
            originDice = diceOriginContainer.Where(d=>d.DiceNumber == this.DiceNumber).First();
        }
        else
        {
            var diceOriginContainer = GameObject.Find("Player2").transform.Find("DiceHolder").transform.GetComponentsInChildren<DiceRollScript>();
            originDice = diceOriginContainer.Where(d=>d.DiceNumber == this.DiceNumber).First();
        }

        if (DiceSlotIsLocked == false)
        {
            if (LockDiceOnBattlefield == false)
            {
                originDice.IsSentToBattlefield = false;
            }
        }
        else
        {
        
        }
    }
    public bool IsSentToBattlefield
    {
        get => _isSentToBattlefield;
        set
        {
            _isSentToBattlefield = value;
            if (value == true)
            {
                this.GetComponent<Image>().color = Color.clear;
                this.GetComponent<Button>().interactable = false;
                this.IsAbleToPickup = false;
            }
            if (value == false)
            {
                // kostce wróciła możliwość losowania i ponownego wrzucenia na plansze
                this.GetComponent<Image>().color = Color.white;
                this.GetComponent<Button>().interactable = true;
                this.IsAbleToPickup = true;
                this.LockDiceOnBattlefield = false;

                // teraz trzeba usunac tą kostke z planszy
                // ? losowo, ważne że nazwa identyczna jak obrazek

                GameObject playerBattlefiel = GetComponentInParent<DiceManager>().PlayerBattlefieldDiceHolder;
                var Dices = playerBattlefiel.transform.GetComponentsInChildren<DiceRollScript>();
                Destroy(Dices.Where(d=>d.DiceNumber == this.DiceNumber).First().gameObject);
            }
        }
    }
    public bool RollingIsCompleted
    {
        get => rollingIsCompleted;
        set
        {
            rollingIsCompleted = value;
            // jezeli kość nie jest na polu bitwy
            if (!this.IsSentToBattlefield)
            {
                // jezeli losowanie sie zakończyło
                if (value == true)
                {
                    // jeżeli jest to ostatnia tura
                    if (GetComponentInParent<DiceManager>().AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES == true)
                    {
                        // jeżeli jakieś kości zostały na stole
                        if (IsSentToBattlefield == false)
                        {
                            SendDiceToBattlefield();
                            LockDiceOnBattlefield = true;
                        }
                    }
                    this.IsAbleToPickup = true;
                }
                else
                {
                    this.IsAbleToPickup = false;
                }
            }

            var endturnbuttons = GameObject.FindGameObjectsWithTag("EndTurnButton");
            if (value == true)
            {
                // po zakończeniu losowania
                foreach (var button in endturnbuttons)
                {
                    button.GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                //twanie losowania
                foreach (var button in endturnbuttons)
                {
                    button.GetComponent<Button>().interactable = false;
                }
            }
        }
    }
    public Image DiceImage
    {
        get => _diceImage;
        set
        {
            _diceImage = value;
        }
    }

    void Update()
    {
        if (this.name != this.GetComponent<Image>().sprite.name)
        {
            this.name = this.GetComponent<Image>().sprite.name;
            this.DiceImage.sprite = this.GetComponent<Image>().sprite;
//            print("zmiana nazwy obiektu i \"diceimage\" na identyczna jak aktualny obrazek");
        }
    }
    void Start()
    {
        this.IsAbleToPickup = false;
        DiceImage = this.GetComponent<Image>();
    }
    public void StartRolling()
    {
        RollingIsCompleted = false;
        List<int> WynikiLosowania = GenerujListeWynikowRzutemKoscia("classic");
        StartCoroutine(RollingAnimation(WynikiLosowania));

        #region statistic debug
        //  List<int> PodsumowanieLosowania = PodliczIloscWyrzuconychWartosciWCalymLosowaniu(WynikiLosowania);
        //  PokazProcentowyUdzialWylosowanychLiczb(PodsumowanieLosowania);
        // Pokaz10PierwszychWylosowanychElementow(WynikiLosowania);
        #endregion
    }
    private void SendDiceToBattlefield()
    {
        IsSentToBattlefield = true;
        var diceOnBattlefield = Instantiate(GameObject.Find("GameManager").GetComponent<GameManager>().DicePrefab, GetComponentInParent<DiceManager>().PlayerBattlefieldDiceHolder.transform.position, Quaternion.identity, GetComponentInParent<DiceManager>().PlayerBattlefieldDiceHolder.transform);
        DiceRollScript diceRollScript = diceOnBattlefield.GetComponent<DiceRollScript>();
        diceRollScript.DiceNumber = this.DiceNumber;
        diceRollScript.DiceImage = this.DiceImage;
        // diceOnBattlefield.AddComponent<DiceRollScript>();
        diceOnBattlefield.GetComponent<Button>().onClick.AddListener(() => diceOnBattlefield.GetComponent<DiceRollScript>().OnClick_TEST_WrocKoscZpolaBitwy());
        diceOnBattlefield.GetComponent<Button>().interactable = true;
        diceOnBattlefield.GetComponent<Image>().sprite = this.DiceImage.sprite;
        if (GetComponentInParent<DiceManager>().PlayerBattlefieldDiceHolder.name == "Player1Dices")
        {
            diceOnBattlefield.transform.Rotate(0, 0, 180f, Space.Self);
        }
        GetComponentInParent<DiceManager>().NumberOfDicesOnBattlefield++;
    }
    IEnumerator RollingAnimation(List<int> wynikiLosowania)
    {
        IsAbleToPickup = false;
        for (int i = 0; i < 25; i++)
        {
            this.GetComponent<Image>().sprite = listaDiceImages.ElementAt(wynikiLosowania[i] - 1);
            yield return new WaitForSeconds(0.05f);
        }
        RollingIsCompleted = true;
    }

    #region Generator + Statystyki
    private static List<int> GenerujListeWynikowRzutemKoscia(string type)
    {
        List<int> wynikiLosowania = new List<int>();

        switch (type)
        {
            case "classic":
                for (int i = 0; i < ILOSC_RZUTOW; i++)
                {
                    wynikiLosowania.Add(RandomNumberGenerator.NumberBetween(1, WIELKOSC_KOSCI));
                }
                break;

            case "simple":
                for (int i = 0; i < ILOSC_RZUTOW; i++)
                {
                    wynikiLosowania.Add(RandomNumberGenerator.SimpleNumberBetween(1, WIELKOSC_KOSCI));
                }
                break;
        }
        return wynikiLosowania;
    }
    /*
    private static void PokazProcentowyUdzialWylosowanychLiczb(List<int> podsumowanieLosowania)
    {
        for (int i = 0; i < WIELKOSC_KOSCI; i++)
        {
            Console.Write($"{i + 1} licba wystąpień = {podsumowanieLosowania[i]} [{((float)(podsumowanieLosowania[i] * 100) / ILOSC_RZUTOW)}%]\n");
        }
    }
    private void Pokaz10PierwszychWylosowanychElementow(List<int> wynikiLosowania)
    {
        string wyniki = "";
        for (int i = 0; i < 10; i++)
        {
            wyniki += wynikiLosowania[i].ToString() + ", ";
        }
        Debug.Log(wyniki);
    }
    private static List<int> PodliczIloscWyrzuconychWartosciWCalymLosowaniu(List<int> wynikiLosowania)
    {
        List<int> podsumowanie = new List<int>();
        for (int i = 1; i <= WIELKOSC_KOSCI; i++)
        {
            podsumowanie.Add(wynikiLosowania.Where(p => p.Equals(i)).Count());
        }
        return podsumowanie;
    }
   */
   #endregion
}
