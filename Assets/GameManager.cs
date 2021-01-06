using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /*
        TODO:
        => START GRY
            - rzut monetą, wybór orła lub reszki. Wygrany zaczyna pierwszy.
        => FAZA LOSOWANIA
            - możliwośc podglądu skili swojego bożka
        => FAZA WYBORU SKILI BÓSTW (W planach na później)
        => FAZA ATAKU I OBRONY
        => FAZA KRADZIEŻY
            - ponowne sprawdzenie
        => FAZA AKTYWACJI SKILA BOŻKA
            - ponowne sprawdzenie wymagań (gold) po wcześniejszym zyskaniu i kradzieży
        => RESET GRY
            - wraca możliwośc losowania kośćmi
    */
    [SerializeField] GameObject BattleField;    
    [SerializeField] bool isBattleModeTurnOn;
    [SerializeField] public GameObject DicePrefab;
    [SerializeField] public string currentGamePhase;

    public float TurnNumber
    {
        get => _turnNumber;
        set
        {
            _turnNumber = value;
            if (TurnNumber > 3.0)
            {
                ChangeUIToBattleMode();
            }
        
            // sprawdzenie czy gracz ma jakieś kości któe mogłby przerolować , inaczej nie pokazuj guzika, pokaz pomin ture odrazu 

            int playerAvailableDices = 6-GameObject.Find(CurrentPlayer).transform.GetComponentsInChildren<DiceRollScript>().Where(d=>d.IsSentToBattlefield == true).Count();
            if(playerAvailableDices == 0){
                SwapRollButonWithEndTurn_OnClick(CurrentPlayer);
            }
        }

        
    }
    [SerializeField] float _turnNumber;

    [SerializeField] GameObject Player1TurnBlocker;
    [SerializeField] int Player1_RollingCounter;
    [SerializeField] bool Player1_LastRollWithAutomaticWithdraw;


    [SerializeField] GameObject Player2TurnBlocker;
    [SerializeField] int Player2_RollingCounter;
    [SerializeField] bool Player2_LastRollWithAutomaticWithdraw;


    [SerializeField] string CurrentPlayer;

    void Start()
    {
        CurrentPlayer = "Player1";
        currentGamePhase = "Dice Rolling Mode";
        TurnNumber = 0;
        Player1_RollingCounter = 0;
        Player2_RollingCounter = 0;
        ChangePlayersTurn();
    }
    void Update()
    {
        ManageOrderingRollButtonsAndActivateLastRollingTurn(Player1_RollingCounter,"Player1");
        ManageOrderingRollButtonsAndActivateLastRollingTurn(Player2_RollingCounter,"Player2");
    }

    private void ManageOrderingRollButtonsAndActivateLastRollingTurn(int rollingTurnNumber, string player)
    {
        if (rollingTurnNumber >= 3.0)
        {
            var player2Object =GameObject.Find("GameCanvas").transform.Find(player).transform;
            var rollButtonObject = player2Object.Find("Roll Button").transform;
            
            if (rollingTurnNumber == 3.0)
            {
                player2Object.Find("DiceHolder").GetComponent<DiceManager>().AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES = true;
            }
            else if (rollingTurnNumber >= 4 || player2Object.GetComponentInChildren<DiceManager>().NumberOfDicesOnBattlefield >= 6) 
            {
                rollButtonObject.SetSiblingIndex(1);
            }
        }
    }   

    void ChangePlayersTurn()
    {
        TurnNumber += 0.5f;
        if (CurrentPlayer == "Player1")
        {
            Player2_RollingCounter++;
            CurrentPlayer = "Player2";
        }
        else
        {
            Player1_RollingCounter++;
            CurrentPlayer = "Player1";
        }

        if (CurrentPlayer == "Player1")
        {
            Player1TurnBlocker.SetActive(false);
            Player2TurnBlocker.SetActive(true);
        }
        else
        {
            Player1TurnBlocker.SetActive(true);
            Player2TurnBlocker.SetActive(false);
        }
    }
    public void SwapRollButonWithEndTurn_OnClick(string playerName)
    {
        print($"<b>{playerName}</b> roll his dices.");
        GameObject.Find(playerName).transform.Find("EndTurnButton").SetSiblingIndex(2);
    }
    public void EndYoursTurn_OnClick(string playerName)
    {
        GameObject.Find(playerName).transform.Find("EndTurnButton").SetSiblingIndex(1);
        GameObject.Find(playerName).transform.Find("DiceHolder").GetComponent<DiceManager>().SetDicesOff = true;
        CurrentPlayer = playerName;
        ChangePlayersTurn();
        print($"<b>{playerName}</b> decide to end of his turn. now its <b>{CurrentPlayer}</b> turn.");

        // automatyczne przelączenie sie do widoku walki jezeli na polu bitwy znajduje sie 12 kostek
        var battlefield = GameObject.Find("Battlefield").transform;
        int player1DiceOnField = battlefield.Find("Player1Dices").childCount;
        int player2DiceOnField = battlefield.Find("Player2Dices").childCount;
        print(player1DiceOnField +", "+player2DiceOnField);
        if((player1DiceOnField + player2DiceOnField)== 12){
            ChangeUIToBattleMode();
        }

        // zablokowanie powrotu kości z pola bitwy po zakończeniu swojej tury
        var currentdicesOnBattlefield = BattleField.GetComponentsInChildren<DiceRollScript>();
        foreach(var dice in currentdicesOnBattlefield){
           // print(dice.name+" <- ta kostka juz tu zostanie for ever :D");
            dice.LockDiceOnBattlefield = true;
            // zablokowanie slotu po tej kości 
            var listaKosciDoZablokowania = GameObject.Find(playerName).transform.Find("DiceHolder")
                .GetComponentsInChildren<DiceRollScript>()
                .Where(d=>d.DiceNumber == dice.DiceNumber)
                .ToList();
                
               foreach(var kosc in listaKosciDoZablokowania){
                    kosc.DiceSlotIsLocked = true; 
               } 
        }



    }
    void ChangeUIToBattleMode()
    {
        if(isBattleModeTurnOn == false){

        
        currentGamePhase = "Battle: Phase 1 -> ''sorting dices''";
        // DONE zmiana wielkości areny przelicznik 3.2x wysokosc
        var battlefieldRT = BattleField.GetComponent<RectTransform>();
        battlefieldRT.sizeDelta = new Vector2(battlefieldRT.sizeDelta.x,battlefieldRT.sizeDelta.y*3.2f);

        //DONE ukrycie paneli przyciemniajacych - sygnalizowanie ktory gracz ma ture
        Player1TurnBlocker.SetActive(false);
        Player2TurnBlocker.SetActive(false);

         // DONE ? albo w trakcie )1 posortowanie kosci na planszy ? deff / attack / steal
    
        // wybranie skilla bozka jezeli to mozliwe

        // przyzna kazzdej ze strony golda
        List<DiceActionScript> DicesOnBF = new List<DiceActionScript>();
        DicesOnBF.AddRange(BattleField.transform.Find("Player1Dices").GetComponentsInChildren<DiceActionScript>().ToList());
        DicesOnBF.AddRange(BattleField.transform.Find("Player2Dices").GetComponentsInChildren<DiceActionScript>().ToList());

            foreach (var dice in DicesOnBF)
            {
                // a skrypt w kostce sprawdzi do kogo nalezy i czy jest "pobłogosławiona"
                //  oraz zostanie wywołana animacja i podsumowanie
                dice.AddGoldFromBlessedItems = true; 
            }
        // pojedyńcze atakowanie, jeżeli napotka swojego defa to przeciwnik nie otrzymuje obrażeń
            // - helm to obona na topór
            // - tarcza obona na łuk
        // atak 2giego gracza

        // kradziez gracza 1 , potem 2
        //throw new NotImplementedException();
        isBattleModeTurnOn = true;
        }
    }
}

