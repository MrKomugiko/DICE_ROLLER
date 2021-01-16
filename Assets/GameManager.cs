using System.Diagnostics.Tracing;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region GENERAL 
    [SerializeField] GameObject BattleField;
    [SerializeField] public GameObject DicePrefab;
    [SerializeField] float interpolationPeriod = .5f;
    [SerializeField] float _turnNumber;
    [SerializeField] bool isBattleModeTurnOn;
    bool IsBattleModeTurnOn
    {
        get => isBattleModeTurnOn;
        set
        {
            isBattleModeTurnOn = value;
            // posortuj kostki na arenie
            if (value)
            {
                BattleField.transform.Find("Player1Dices").GetComponent<DiceSorterScript>().PosortujKosci = true;
                BattleField.transform.Find("Player2Dices").GetComponent<DiceSorterScript>().PosortujKosci = true;

                Player1TurnBlocker.SetActive(false);
                Player2TurnBlocker.SetActive(false);

                GameObject.Find("Player1").transform.Find("EndTurnButton").gameObject.SetActive(false);
                GameObject.Find("Player2").transform.Find("EndTurnButton").gameObject.SetActive(false);

                GameObject.Find("Player1").transform.Find("Roll Button").gameObject.SetActive(false);
                GameObject.Find("Player2").transform.Find("Roll Button").gameObject.SetActive(false);
            }
        }
    }
    float time = 0.0f, time2 = 0.0f;
    [SerializeField] string CurrentPlayer;
    [SerializeField] string currentGamePhase;
    float TurnNumber
    {
        get => _turnNumber;
        set
        {
            _turnNumber = value;

            // sprawdzenie czy gracz ma jakieś kości któe mogłby przerolować , inaczej nie pokazuj guzika, pokaz pomin ture odrazu 
            int playerAvailableDices = 6 - GameObject.Find(CurrentPlayer).transform.GetComponentsInChildren<DiceRollScript>().Where(d => d.IsSentToBattlefield == true).Count();

            if (playerAvailableDices == 0)
            {
                SwapRollButonWithEndTurn_OnClick(CurrentPlayer);
            }
        }
    }
    #endregion

    #region PLAYER 1 
    int Player1_RollingCounter;
    [SerializeField] GameObject Player1TurnBlocker;

    #region GOLD Blessed + Steal
    [SerializeField] Text Player1_GoldVault;
    public int cumulativeGoldStealingCounterP1;
    int _currentGold1;
    public int CurrentGold1
    {
        get => _currentGold1;
        set => _currentGold1 = value;
    }
    int liczbaPrzelewowGolda_Player1;
    int _temporaryGoldVault_player1;
    public int TemporaryGoldVault_player1
    {
        get
        {
            return _temporaryGoldVault_player1;
        }
        set
        {
            var p1coin = GameObject.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>();
            if (value > 0)
            {
                // DODAWANIE GOLDA
                if (value != 0)
                {
                    cumulativeGoldStealingCounterP1++;
                    p1coin.SetText("+" + cumulativeGoldStealingCounterP1.ToString());
                    liczbaPrzelewowGolda_Player1++;
                }
            }
            if (value < 0)
            {
                // ODEJMOWANIE GOLDA
                if (value != 0)
                {
                    cumulativeGoldStealingCounterP1--;
                    p1coin.SetText(cumulativeGoldStealingCounterP1.ToString());
                    liczbaPrzelewowGolda_Player1--;
                }
            }
            _temporaryGoldVault_player1 = value;
        }
    }
    #endregion

    #region HEALTH Combat    
    [SerializeField] Text Player1_HPPoints;
    int Player1ActualHPValue;
    int liczbaPrzelewaniaObrazen_Player1;
    int _temporaryIntakeDamage_Player1;
    public int TemporaryIntakeDamage_Player1
    {
        get
        {
            return _temporaryIntakeDamage_Player1;
        }
        set
        {
            _temporaryIntakeDamage_Player1 = value;
            var p1hp = GameObject.Find("HealthTextPlayer1").GetComponent<TextMeshProUGUI>();
            if (value != 0)
            {
                p1hp.SetText("-" + _temporaryIntakeDamage_Player1.ToString());
                liczbaPrzelewaniaObrazen_Player1++;
            }

            if (value == 0)
            {
                p1hp.SetText("");
            }
        }
    }
    #endregion

    #endregion

    #region PLAYER 2 
    int Player2_RollingCounter;
    [SerializeField] GameObject Player2TurnBlocker;

    #region GOLD Blessed + Steal
    [SerializeField] Text Player2_GoldVault;
    public int cumulativeGoldStealingCounterP2;
    int _currentGold2;
    public int CurrentGold2
    {
        get => _currentGold2;
        set => _currentGold2 = value;
    }
    int liczbaPrzelewowGolda_Player2;
    int _temporaryGoldVault_player2;
    public int TemporaryGoldVault_player2
    {
        get
        {
            return _temporaryGoldVault_player2;
        }
        set
        {
            var p2coin = GameObject.Find("CoinTextPlayer2").GetComponent<TextMeshProUGUI>();
            if (value > 0)
            {
                // DODAWANIE GOLDA
                if (value != 0)
                {
                    cumulativeGoldStealingCounterP2++;
                    p2coin.SetText("+" + cumulativeGoldStealingCounterP2.ToString());
                    liczbaPrzelewowGolda_Player2++;
                }
            }
            if (value < 0)
            {
                // ODEJMOWANIE GOLDA
                if (value != 0)
                {
                    cumulativeGoldStealingCounterP2--;
                    p2coin.SetText(cumulativeGoldStealingCounterP2.ToString());
                    liczbaPrzelewowGolda_Player2--;
                }
            }
            _temporaryGoldVault_player2 = value;
        }
    }
    #endregion

    #region HEALTH Combat
    [SerializeField] Text Player2_HPPoints;
    int Player2ActualHPValue;
    int liczbaPrzelewaniaObrazen_Player2;
    int _temporaryIntakeDamage_Player2;
    public int TemporaryIntakeDamage_Player2
    {
        get
        {
            return _temporaryIntakeDamage_Player2;
        }
        set
        {
            _temporaryIntakeDamage_Player2 = value;
            var p2hp = GameObject.Find("HealthTextPlayer2").GetComponent<TextMeshProUGUI>();
            if (value != 0)
            {
                p2hp.SetText("-" + _temporaryIntakeDamage_Player2.ToString());
                liczbaPrzelewaniaObrazen_Player2++;
            }

            if (value == 0)
            {
                p2hp.SetText("");
            }
        }
    }
    #endregion

    #endregion
    
    void Start()
    {
        CurrentGold1 = Convert.ToInt32(Player1_GoldVault.text);
        CurrentGold2 = Convert.ToInt32(Player2_GoldVault.text);

        var p1hp = GameObject.Find("HealthTextPlayer1").GetComponent<TextMeshProUGUI>();
        p1hp.text = "";
        var p2hp = GameObject.Find("HealthTextPlayer2").GetComponent<TextMeshProUGUI>();
        p2hp.text = "";

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
        time2 += Time.deltaTime;

        TransferGoldToPlayers(ref time, interpolationPeriod);
        TransferDamageToPlayers(ref time2, interpolationPeriod);
    }

    /// <summary> 
    ///     Przelewanie golda z wczesniej naliczonych wartości tymczasowych do konta graczy z "animacją"
    /// </summary>
    /// <remarks>
    ///     <param name ="timePassedInGame"> ref => wraca zaktualizowaną wartość ( zeruje ) czas który upłynął w sekundach</param>
    ///     <param name ="timeDelayinSecons"> wartość opóźnienia między wykonywaniem iteracji</param>
    /// </remarks>
    private void TransferGoldToPlayers(ref float timePassedInGame, float timeDelayinSecons)
    {
        if (timePassedInGame >= this.interpolationPeriod)
        {

            // reset czasu do 0 i naliczanie dalej os początku
            timePassedInGame = timePassedInGame - interpolationPeriod;
            //---------------------------------------------------------------------------------------------------------------------------


            if (liczbaPrzelewowGolda_Player1 > 0)
            {
                // DODAWANIE GOLDA
                CurrentGold1++;
                Player1_GoldVault.text = CurrentGold1.ToString();
                liczbaPrzelewowGolda_Player1--;
            }
            else if (liczbaPrzelewowGolda_Player1 < 0)
            {
                // ODEJMOWANIE GOLDA
                CurrentGold1--;
                Player1_GoldVault.text = CurrentGold1.ToString();
                liczbaPrzelewowGolda_Player1++;
            }
            else if (liczbaPrzelewowGolda_Player1 == 0)
            {
                // ZEROWANIE WARTOSCI TYMCZASOWYCH
                TemporaryGoldVault_player1 = 0;
                liczbaPrzelewowGolda_Player1 = 0;
                
                if (cumulativeGoldStealingCounterP1 == 0)
                {
                    var p1coin = GameObject.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>();
                    p1coin.SetText("");
                }
            }
            //---------------------------------------------------------------------------------------------------------------------------

            if (liczbaPrzelewowGolda_Player2 > 0)
            {
                // DODAWANIE GOLDA
                CurrentGold2++;
                Player2_GoldVault.text = CurrentGold2.ToString();
                liczbaPrzelewowGolda_Player2--;
            }
            else if (liczbaPrzelewowGolda_Player2 < 0)
            {
                // ODEJMOWANIE GOLDA
                CurrentGold2 = Convert.ToInt32(Player2_GoldVault.text);
                CurrentGold2--;
                Player2_GoldVault.text = CurrentGold2.ToString();
                liczbaPrzelewowGolda_Player2++;
            }
            else if (liczbaPrzelewowGolda_Player2 == 0)
            {
                // ZEROWANIE WARTOSCI TYMCZASOWYCH
                TemporaryGoldVault_player2 = 0;
                liczbaPrzelewowGolda_Player2 = 0;

                if (cumulativeGoldStealingCounterP2 == 0)
                {
                    var p2coin = GameObject.Find("CoinTextPlayer2").GetComponent<TextMeshProUGUI>();
                    p2coin.SetText("");
                }
            }
        }
    }

    /// <summary> 
    ///     przenoszenie nazbieranych obrażeń naliczonych w wartości tymczasowych do konta graczy z "animacją" 
    /// </summary>
    /// <remarks>
    ///     <param name ="timePassedInGame"> ref => wraca zaktualizowaną wartość ( zeruje ) czas który upłynął w sekundach</param>
    ///     <param name ="timeDelayinSecons"> wartość opóźnienia między wykonywaniem iteracji</param>
    /// </remarks>
    private void TransferDamageToPlayers(ref float timePassedInGame, float timeDelayinSecons)
    {
        if (timePassedInGame >= this.interpolationPeriod)
        {
            Player1ActualHPValue = Convert.ToInt32(Player1_HPPoints.text);
            Player2ActualHPValue = Convert.ToInt32(Player2_HPPoints.text);

            // reset czasu do 0 i naliczanie dalej os początku
            timePassedInGame = timePassedInGame - interpolationPeriod;

            if (liczbaPrzelewaniaObrazen_Player1 > 0)
            {
                int p1Currenthp = Player1ActualHPValue;
                int p1NewHpValue = p1Currenthp - 1;
                Player1_HPPoints.text = (p1NewHpValue.ToString());

                liczbaPrzelewaniaObrazen_Player1--;
            }

            if (liczbaPrzelewaniaObrazen_Player2 > 0)
            {
                int p2Currenthp = Player2ActualHPValue;
                int p2NewHpValue = p2Currenthp - 1;

                Player2_HPPoints.text = (p2NewHpValue.ToString());

                liczbaPrzelewaniaObrazen_Player2--;
            }
        }
    }

    /// <summary> 
    /// ustalenie na podstawie numeru tury, czy ejst to ostatnie automatyczne losowanie
    /// </summary>
    /// <remarks>
    ///     <param name ="rollingTurnNumber">aktualna tura rozgrywki</param>
    ///     <param name ="playerName">identyfikator gracza (Player1 albo Player2)</param>
    /// </remarks>
    void ManageOrderingRollButtonsAndActivateLastRollingTurn(int rollingTurnNumber, string player)
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
    void SwapRollButonWithEndTurn_OnClick(string playerName)
    {
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

        // automatyczne przelączenie sie do widoku walki jezeli na polu bitwy znajduje sie 12 kostek
        var battlefield = GameObject.Find("Battlefield").transform;
        var player1DiceOnField = battlefield.Find("Player1Dices");
        var player2DiceOnField = battlefield.Find("Player2Dices");
        if ((player1DiceOnField.childCount + player2DiceOnField.childCount) == 12)
        {
            ChangeUIToBattleMode();
        }

        // zablokowanie powrotu kości z pola bitwy po zakończeniu swojej tury
        var currentdicesOnBattlefield = BattleField.GetComponentsInChildren<DiceRollScript>();
        foreach (var dice in currentdicesOnBattlefield)
        {
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
        if (IsBattleModeTurnOn == false)
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
            IsBattleModeTurnOn = true;
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