using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject BattleField;
    bool isBattleModeTurnOn;
    [SerializeField] public GameObject DicePrefab;
    public string currentGamePhase;

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
            int playerAvailableDices = 6 - GameObject.Find(CurrentPlayer).transform.GetComponentsInChildren<DiceRollScript>().Where(d => d.IsSentToBattlefield == true).Count();
            if (playerAvailableDices == 0)
            {
                SwapRollButonWithEndTurn_OnClick(CurrentPlayer);
            }
        }


    }

    float _turnNumber;

    [SerializeField] GameObject Player1TurnBlocker;
    int Player1_RollingCounter;
    bool Player1_LastRollWithAutomaticWithdraw;

    [SerializeField] GameObject Player2TurnBlocker;
    int Player2_RollingCounter;
    bool Player2_LastRollWithAutomaticWithdraw;

    string CurrentPlayer;

    float time = 0.0f;
    float interpolationPeriod = .5f;
    private int currentGold1 = 0;
    [SerializeField] TextMeshProUGUI Player1_GoldVault;
    private int currentGold2 = 0;
    [SerializeField] TextMeshProUGUI Player2_GoldVault;

    private int liczbaPrzelewowGolda_Player1;
    private int liczbaPrzelewowGolda_Player2;
    [SerializeField] public static List<Image> OnBattlefield_Dice_Player1 = new List<Image>();
    [SerializeField] public static List<Image> OnBattlefield_Dice_Player2 = new List<Image>();
    private int _temporaryGoldVault_player1;
    private int _temporaryGoldVault_player2;

    void Start()
    {
        currentGold1 = Convert.ToInt32(Player1_GoldVault.text);
        currentGold2 = Convert.ToInt32(Player2_GoldVault.text);

        CurrentPlayer = "Player1";
        currentGamePhase = "Dice Rolling Mode";
        TurnNumber = 0;
        Player1_RollingCounter = 0;
        Player2_RollingCounter = 0;
        ChangePlayersTurn();
    }
    void Update()
    {
        ManageOrderingRollButtonsAndActivateLastRollingTurn(Player1_RollingCounter, "Player1");
        ManageOrderingRollButtonsAndActivateLastRollingTurn(Player2_RollingCounter, "Player2");

        time += Time.deltaTime;

        if (time >= interpolationPeriod)
        {
            time = time - interpolationPeriod;

            if (liczbaPrzelewowGolda_Player1 > 0)
            {
                currentGold1++;
                Player1_GoldVault.SetText(currentGold1.ToString());

                liczbaPrzelewowGolda_Player1--;
                if (liczbaPrzelewowGolda_Player1 == 0)
                {
                    // wyzeruj skarbonke
                    TemporaryGoldVault_player1 = 0;
                    liczbaPrzelewowGolda_Player1 = 0;
                }
            }

            if (liczbaPrzelewowGolda_Player2 > 0)
            {
                currentGold2++;
                Player2_GoldVault.SetText(currentGold2.ToString());

                liczbaPrzelewowGolda_Player2--;
                if (liczbaPrzelewowGolda_Player2 == 0)
                {
                    // wyzeruj skarbonke
                    TemporaryGoldVault_player2 = 0;
                    liczbaPrzelewowGolda_Player2 = 0;
                }
            }
        }
    }

    /// <summary> ustalenie na podstawie numeru tury, czy ejst to ostatnie automatyczne losowanie
    /// </summary>
    /// <remarks>
    ///     <param name ="rollingTurnNumber">aktualna tura rozgrywki</param>
    ///     <param name ="playerName">identyfikator gracza (Player1 albo Player2)</param>
    /// </remarks>
    private void ManageOrderingRollButtonsAndActivateLastRollingTurn(int rollingTurnNumber, string player)
    {
        if (rollingTurnNumber >= 3.0)
        {
            var player2Object = GameObject.Find(player).transform;
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

    /// <summary> 
    ///     Oddanie tury przeciwnikowi, oraz przyciemnienie swojej częsci ekranu
    /// </summary>
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

    /// <summary> 
    ///     Zmiana kolejności wyświetlania się przycisków roll i endTurn
    /// </summary>
    /// <remarks>
    ///     <param name ="playerName">identyfikator gracza (Player1 albo Player2)</param>
    /// </remarks>
    public void SwapRollButonWithEndTurn_OnClick(string playerName)
    {
        print($"<b>{playerName}</b> roll his dices.");
        GameObject.Find(playerName).transform.Find("EndTurnButton").SetSiblingIndex(2);
    }

    /// <summary> 
    ///     Metoda odpowiedzialna za kończenie tury 
    ///     <list type="bullet">
    ///         <item>Oddanie ruchu przeciwnikowi</item>
    ///         <item>Zablokowanie kości dodanych na arene</item>
    ///     </list>
    /// </summary>
    /// <remarks>
    ///     <param name ="playerName">identyfikator gracza (Player1 albo Player2)</param>
    /// </remarks>
    public void EndYoursTurn_OnClick(string playerName)
    {
        GameObject.Find(playerName).transform.Find("EndTurnButton").SetSiblingIndex(1);
        GameObject.Find(playerName).transform.Find("DiceHolder").GetComponent<DiceManager>().SetDicesOff = true;
        CurrentPlayer = playerName;
        ChangePlayersTurn();
        print($"<b>{playerName}</b> decide to end of his turn. now its <b>{CurrentPlayer}</b> turn.");

        // automatyczne przelączenie sie do widoku walki jezeli na polu bitwy znajduje sie 12 kostek
        var battlefield = GameObject.Find("Battlefield").transform;
        var player1DiceOnField = battlefield.Find("Player1Dices");
        var player2DiceOnField = battlefield.Find("Player2Dices");
        //        print(player1DiceOnField +", "+player2DiceOnField);
        if ((player1DiceOnField.childCount + player2DiceOnField.childCount) == 12)
        {
            ChangeUIToBattleMode();
        }

        // zablokowanie powrotu kości z pola bitwy po zakończeniu swojej tury
        var currentdicesOnBattlefield = BattleField.GetComponentsInChildren<DiceRollScript>();
        foreach (var dice in currentdicesOnBattlefield)
        {
            // print(dice.name+" <- ta kostka juz tu zostanie for ever :D");
            dice.LockDiceOnBattlefield = true;
            // zablokowanie slotu po tej kości 
            var listaKosciDoZablokowania = GameObject.Find(playerName).transform.Find("DiceHolder")
                .GetComponentsInChildren<DiceRollScript>()
                .Where(d => d.DiceNumber == dice.DiceNumber)
                .ToList();

            foreach (var kosc in listaKosciDoZablokowania)
            {
                kosc.DiceSlotIsLocked = true;
            }
        }
    }

    /// <summary> 
    ///     Ustawienie trybu potyczki
    ///     <list type="bullet">
    ///         <item>zmiana układu i miejsca kości</item>
    ///         <item>ukrycie paneli blokujących gracza</item>
    ///         <item>Zainicjowanie wykonania funkcji zbierającej złoto z kości</item>
    ///     </list>
    /// </summary>
    void ChangeUIToBattleMode()
    {
        if (isBattleModeTurnOn == false)
        {
            currentGamePhase = "Battle: Phase 1 -> ''sorting dices''";

            var battlefieldRT = BattleField.GetComponent<RectTransform>();
            battlefieldRT.sizeDelta = new Vector2(battlefieldRT.sizeDelta.x, battlefieldRT.sizeDelta.y * 3.2f);

            //DONE ukrycie paneli przyciemniajacych - sygnalizowanie ktory gracz ma ture
            Player1TurnBlocker.SetActive(false);
            Player2TurnBlocker.SetActive(false);

            var p1Dices = GameObject.Find("Player1Dices").GetComponentsInChildren<DiceActionScript>();
            foreach (var dice in p1Dices)
            {
                dice.AddGoldFromBlessedItems = true;
            }
            var p2Dices = GameObject.Find("Player2Dices").GetComponentsInChildren<DiceActionScript>();
            foreach (var dice in p2Dices)
            {
                dice.AddGoldFromBlessedItems = true;
            }
            isBattleModeTurnOn = true;
        }
    }
    public int TemporaryGoldVault_player1
    {
        get
        {
            return _temporaryGoldVault_player1;
        }
        set
        {
            _temporaryGoldVault_player1 = value;
            var p1coin = GameObject.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>();
            p1coin.SetText("+" + _temporaryGoldVault_player1.ToString());

            liczbaPrzelewowGolda_Player1++;
        }
    }

    public int TemporaryGoldVault_player2
    {
        get
        {
            return _temporaryGoldVault_player2;
        }
        set
        {
            _temporaryGoldVault_player2 = value;
            var p2coin = GameObject.Find("CoinTextPlayer2").GetComponent<TextMeshProUGUI>();
            p2coin.SetText("+" + _temporaryGoldVault_player2.ToString());

            liczbaPrzelewowGolda_Player2++;
        }
    }

    /// <summary> 
    ///     Testowa metoda wyzwalająca ponowne zebranie "GodFavor" z pola bitwy
    /// </summary>
    public void ANDROID_AgainAddGold()
    {
        List<DiceActionScript> blessedDP1 = GameObject.Find("Player1Dices")
            .GetComponentsInChildren<DiceActionScript>()
            .Where(d => d.name.Contains("Blessed")).ToList();

        List<DiceActionScript> blessedDP2 = GameObject.Find("Player2Dices")
            .GetComponentsInChildren<DiceActionScript>()
            .Where(d => d.name.Contains("Blessed")).ToList();

        GameObject.Find("ANDROID_TEST_GOLDBUTTON").GetComponent<Button>()
            .GetComponentInChildren<TextMeshProUGUI>()
            .SetText($"{blessedDP1.Count()} <------> {blessedDP2.Count()}");

        foreach (DiceActionScript dice in blessedDP1)
        {
            dice.AddGoldFromBlessedItems = true;
        }
        foreach (DiceActionScript dice in blessedDP2)
        {
            dice.AddGoldFromBlessedItems = true;
        }
    }
}