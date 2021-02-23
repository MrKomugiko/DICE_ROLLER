using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DiceRoller_Console;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollScript : MonoBehaviour
{
    [SerializeField] public string DiceOwner;
    [SerializeField] public int DiceNumber;
    public static int WIELKOSC_KOSCI = 6;
    public static int ILOSC_RZUTOW = 20;

    [SerializeField] public bool DiceSlotIsLocked;
    [SerializeField] private bool _rollingIsCompleted;
    [SerializeField] public List<Sprite> listaDiceImages;

    [SerializeField] Image _diceImage;
    [SerializeField] bool _isSentToBattlefield;
    [SerializeField] bool _isAbleToPickup;
    [SerializeField] bool _lockDiceOnBattlefield;
    private GameManager GameManager_Script;

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
               // GameManager test = new GameManager();
            }
        }
    }
    [SerializeField] public bool LockDiceOnBattlefield 
    { 
        get => _lockDiceOnBattlefield; 
        set {
            _lockDiceOnBattlefield = value; 
        } 
    }
    public void OnClick_TEST_WrocKoscZpolaBitwy()
    {
        // sprawdzenie blokady na kostce "matce" na ręce wyszukanej po numeze kości
        var myContainer = this.GetComponent<DiceActionScript>().transform.parent;
        DiceRollScript originDice;
        if(myContainer.name == "Player1Dices")
        {
            var diceOriginContainer = GameObject.Find("Player1").transform.Find("DiceHolder").transform.GetComponentsInChildren<DiceRollScript>();
            originDice = diceOriginContainer.Where(d=>d.DiceNumber == this.DiceNumber).First();
           
            var diceOnBattlefield = GameManager_Script.Player_1.ListOfDicesOnBattleground.Where(d=>d.DiceNumber == this.DiceNumber).First();
            GameManager_Script.Player_1.ListOfDicesOnBattleground.Remove(diceOnBattlefield);
        }
        else
        {
            var diceOriginContainer = GameObject.Find("Player2").transform.Find("DiceHolder").transform.GetComponentsInChildren<DiceRollScript>();
            originDice = diceOriginContainer.Where(d=>d.DiceNumber == this.DiceNumber).First();

            var diceOnBattlefield = GameManager_Script.Player_2.ListOfDicesOnBattleground.Where(d=>d.DiceNumber == this.DiceNumber).First();
            GameManager_Script.Player_2.ListOfDicesOnBattleground.Remove(diceOnBattlefield);
        }
        if (DiceSlotIsLocked == false)
        {
            if (LockDiceOnBattlefield == false)
            {
                originDice.IsSentToBattlefield = false;
            }
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

                GameObject playerBattlefiel = GetComponentInParent<DiceManager>().PlayerBattlefieldDiceHolder;
                var Dices = playerBattlefiel.transform.GetComponentsInChildren<DiceRollScript>();
                 try{Destroy(Dices.Where(d=>d.DiceNumber == this.DiceNumber).First().gameObject);}
                 catch(Exception)
                 {
                     //print("nie znaleziono kostki do usunięcia");
                     }
            }
        }
    }
    public bool RollingIsCompleted
    {
        get => _rollingIsCompleted;
        set
        {
            _rollingIsCompleted = value;
            // jezeli kość nie jest na polu bitwy
            if (!this.IsSentToBattlefield)
            {
                // jezeli losowanie sie zakończyło
                if (value == true)
                {
                    this.IsAbleToPickup = true;
                    // jeżeli jest to ostatnia tura
                    if (GetComponentInParent<DiceManager>().AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES == true)
                    {
                        // jeżeli jakieś kości zostały na stole
                        if (IsSentToBattlefield == false)
                        {
                            SendDiceToBattlefield();
                            LockDiceOnBattlefield = true;
                            this.IsAbleToPickup = false;
                        }
                    }
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
        }
    }
    void Start()
    {
        GameManager_Script = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.IsAbleToPickup = false;
        DiceImage = this.GetComponent<Image>();
    }
    public void StartRolling()
    {
        RollingIsCompleted = false;
        List<int> WynikiLosowania = GenerujListeWynikowRzutemKoscia("classic");
        StartCoroutine(RollingAnimation(WynikiLosowania));
    }
    private void SendDiceToBattlefield()
    {
        IsSentToBattlefield = true;
        var diceOnBattlefield = Instantiate(GameManager_Script.DicePrefab, GetComponentInParent<DiceManager>().PlayerBattlefieldDiceHolder.transform.position, Quaternion.identity, GetComponentInParent<DiceManager>().PlayerBattlefieldDiceHolder.transform);
        DiceRollScript diceRollScript = diceOnBattlefield.GetComponent<DiceRollScript>();
        diceRollScript.DiceNumber = this.DiceNumber;
        diceRollScript.DiceOwner = this.DiceOwner;
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
        
        if(DiceOwner == "Player1")
        {
            GameManager_Script.Player_1.ListOfDicesOnBattleground.Add(diceOnBattlefield.GetComponent<DiceRollScript>());
        }
        else
        {
            GameManager_Script.Player_2.ListOfDicesOnBattleground.Add(diceOnBattlefield.GetComponent<DiceRollScript>());
        }
    }
    IEnumerator RollingAnimation(List<int> wynikiLosowania)
    {
        IsAbleToPickup = false;
        for (int i = 0; i < 10; i++)
        {
            this.GetComponent<Image>().sprite = listaDiceImages.ElementAt(wynikiLosowania[i]-1);
            yield return new WaitForSeconds(0.05f);
        }
        RollingIsCompleted = true;
    }
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
}
