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
            DONE - pojedyńcze losowanie na turę
            DONE - kontrolowanie tury gracza:
            DONE - poprzez blokade losowania lub wybierania kości
            - możliwośc podglądu skili swojego bożka
            DONE - zapisywanie/wybieranie wylosowanych kości
            DONE - po wybraniu kośc wędruje na środek (battlefield)
                DONE (its hidden and blocked)* miejsce po wybranej kosci nie bierze udziału w dalszym losowaniu
                DONE * zmiana jasności pola kości lub jego zniknięcie
            DONE - 3 tury rzutów, po tym razie wszystkie wylosowane kosci ląduja na srodku
        => FAZA WYBORU SKILI BÓSTW (W planach na później)
        => FAZA PRZYZNANIA GOLDA
        => FAZA ATAKU I OBRONY
        => FAZA KRADZIEŻY
            - ponowne sprawdzenie
        => FAZA AKTYWACJI SKILA BOŻKA
            - ponowne sprawdzenie wymagań (gold) po wcześniejszym zyskaniu i kradzieży
        => RESET GRY
            - wraca możliwośc losowania kośćmi
    */
    [SerializeField] public GameObject DicePrefab;
    [SerializeField] GameObject BattleField;


    public float TurnNumber
    {
        get => _turnNumber;
        set
        {
            _turnNumber = value;
            if (TurnNumber > 3.0)
            {
                print("its time for duel :D");
                ChangeUIToBattleMode();
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
        TurnNumber = 0;
        Player1_RollingCounter = 0;
        Player2_RollingCounter = 0;
        ChangePlayersTurn();
    }
    void Update()
    {
        if (Player1_RollingCounter >= 3.0)
        {
            if (Player1_RollingCounter == 3.0)
            {
                GameObject.Find("GameCanvas").transform.Find("Player1").transform.Find("DiceHolder").GetComponent<DiceManager>().AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES = true;
            }
            else if (Player1_RollingCounter >= 4)
            {
                GameObject.Find("GameCanvas").transform.Find("Player1").transform.Find("Roll Button").transform.SetSiblingIndex(1);
            }
            else if(GameObject.Find("Player1").GetComponentInChildren<DiceManager>().NumberOfDicesOnBattlefield >= 6)
            {
                GameObject.Find("GameCanvas").transform.Find("Player1").transform.Find("Roll Button").transform.SetSiblingIndex(1);
            }
        }

        if (Player2_RollingCounter >= 3.0)
        {
            if (Player2_RollingCounter == 3.0)
            {
                GameObject.Find("GameCanvas").transform.Find("Player2").transform.Find("DiceHolder").GetComponent<DiceManager>().AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES = true;
            }
            else if (Player2_RollingCounter >= 4)
            {
                GameObject.Find("GameCanvas").transform.Find("Player2").transform.Find("Roll Button").transform.SetSiblingIndex(1);
            }
            else if(GameObject.Find("Player2").GetComponentInChildren<DiceManager>().NumberOfDicesOnBattlefield >= 6)
            {
                GameObject.Find("GameCanvas").transform.Find("Player2").transform.Find("Roll Button").transform.SetSiblingIndex(1);
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


    }
    void ChangeUIToBattleMode()
    {
        // zmiana wielkości areny przelicznik 3.2x wysokosc
        var battlefieldRT = BattleField.GetComponent<RectTransform>();
        battlefieldRT.sizeDelta = new Vector2(battlefieldRT.sizeDelta.x,battlefieldRT.sizeDelta.y*3.2f);

        // ukrycie paneli przyciemniajacych - sygnalizowanie ktory gracz ma ture
        Player1TurnBlocker.SetActive(false);
        Player2TurnBlocker.SetActive(false);

        // ? albo w trakcie )1 posortowanie kosci na planszy ? deff / attack / steal
        #region notatki dotyczące numeracji i znaczenia poszczególnych kości
        /*
            1,2     Axe     Mele Attack
            3       Hand    Steal
            4       Bow     Ranged Attack
            5       Sheield Ranged Deffence
            6       Helmet  Mele Deffence

            
        */
        #endregion

        // wybranie skilla bozka jezeli to mozliwe

        // przyzna kazzdej ze strony golda

        // pojedyńcze atakowanie, jeżeli napotka swojego defa to przeciwnik nie otrzymuje obrażeń
            // - helm to obona na topór
            // - tarcza obona na łuk
        // atak 2giego gracza

        // kradziez gracza 1 , potem 2
        //throw new NotImplementedException();
    }
}

