using System.Diagnostics.Tracing;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] public static float GameSpeedValueModifier = 50;

    #region GENERAL 
    [SerializeField] GameObject EndGameResultWindows;
    [SerializeField] GameObject BattleField;
    [SerializeField] public GameObject DicePrefab;
    [SerializeField] float interpolationPeriod = .5f;
    [SerializeField] float _turnNumber;
    [SerializeField] bool isBattleModeTurnOn;
    public bool IsBattleModeTurnOn
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
    GodsManager Player1GodsManagerScript;
    [SerializeField] GameObject Player1UseSkillTestButton;
    [SerializeField] public GameObject Player1GodSkillWindow;
    [SerializeField] GameObject Player1TurnBlocker;

    #region GOLD Blessed + Steal
    [SerializeField] Text Player1_GoldVault;
    [SerializeField] private int _cumulativeGoldStealingCounterP1;
    public int CumulativeGoldStealingCounterP1 { get => _cumulativeGoldStealingCounterP1; set => _cumulativeGoldStealingCounterP1 = value; }

    [SerializeField] int _currentGold1;
    public int CurrentGold1
    {
        get => _currentGold1;
        set
        {
         _currentGold1 = value;
        }
    }
    int _liczbaPrzelewowGolda_Player1;
    public int LiczbaPrzelewowGolda_Player1 
    { 
        get => _liczbaPrzelewowGolda_Player1; 
        set 
        {
            _liczbaPrzelewowGolda_Player1 = value; 
        }
    }
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
                    CumulativeGoldStealingCounterP1++;
                    p1coin.SetText("+" + CumulativeGoldStealingCounterP1.ToString());
                    LiczbaPrzelewowGolda_Player1++;
                }
            }
            if (value < 0)
            {
                // ODEJMOWANIE GOLDA
                if (value != 0)
                {
                    CumulativeGoldStealingCounterP1--;
                    p1coin.SetText(CumulativeGoldStealingCounterP1.ToString());
                    LiczbaPrzelewowGolda_Player1--;
                }
            }
            _temporaryGoldVault_player1 = value;
        }
    }
    #endregion

    #region HEALTH Combat    
    [SerializeField] Text Player1_HPPoints;
    [SerializeField] int Player1ActualHPValue;
    [SerializeField] int liczbaPrzelewaniaObrazen_Player1;
    [SerializeField] int _temporaryIntakeDamage_Player1;
    [SerializeField] bool EndOfGame = false;
    public int TemporaryIntakeDamage_Player1
    {
        get
        {
            return _temporaryIntakeDamage_Player1;
        }
        set
        {
            _temporaryIntakeDamage_Player1 = value;
            
            if(EndOfGame == false)
            {
                if(Player1ActualHPValue <= 0)
                {
                    print("Player 1 Actual HP = "+Player1ActualHPValue);
                    IsGameEnded = true;
                    ShowEndGameResultWindow(winner:"Player2");
                    _temporaryIntakeDamage_Player1 = 0;
                }
            }
            var p1hp = GameObject.Find("HealthTextPlayer1").GetComponent<TextMeshProUGUI>();
            if (value > 0)
            {
                p1hp.SetText("-" + _temporaryIntakeDamage_Player1.ToString());
                p1hp.color = Color.red;
                liczbaPrzelewaniaObrazen_Player1++;

            }

            if (value < 0)
            {
                print("value: " + value);

                print("różnica : " + (TemporaryIntakeDamage_Player1 - value).ToString());

                p1hp.SetText("+" + _temporaryIntakeDamage_Player1.ToString());
                p1hp.color = Color.green;
                liczbaPrzelewaniaObrazen_Player1--;
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
    [SerializeField] GodsManager Player2GodsManagerScript;
    [SerializeField] GameObject Player2UseSkillTestButton;
    [SerializeField] public GameObject Player2GodSkillWindow;
    [SerializeField] GameObject Player2TurnBlocker;

    #region GOLD Blessed + Steal
    [SerializeField] Text Player2_GoldVault;
    [SerializeField] private int _cumulativeGoldStealingCounterP2;
    [SerializeField] int _currentGold2;
    public int CurrentGold2
    {
        get => _currentGold2;
        set {
            _currentGold2 = value;
        }
    }

    [SerializeField] private bool _isGameEnded = false;
    private void ShowEndGameResultWindow(string winner)
    {
            EndOfGame = true;
            
            var CombatMAnager =  GameObject.Find("FightZone").GetComponent<CombatManager>();
            CombatMAnager.IndexOfCombatAction = 0;

            if(winner == "Player2")
            {
                EndGameResultWindows.transform.Find("WIN").transform.gameObject.SetActive(true);
            }
            else
            {
                EndGameResultWindows.transform.Find("LOSE").transform.gameObject.SetActive(true);
            }
    }

    int _liczbaPrzelewowGolda_Player2;
    public int LiczbaPrzelewowGolda_Player2 
    { 
        get => _liczbaPrzelewowGolda_Player2; 
        set 
        {
            _liczbaPrzelewowGolda_Player2 = value; 
        }
    }
    [SerializeField] int _temporaryGoldVault_player2;
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
                    CumulativeGoldStealingCounterP2++;
                    p2coin.SetText("+" + CumulativeGoldStealingCounterP2.ToString());
                    LiczbaPrzelewowGolda_Player2++;
                }
            }
            if (value < 0)
            {
                // ODEJMOWANIE GOLDA
                if (value != 0)
                {
                    CumulativeGoldStealingCounterP2--;
                    p2coin.SetText(CumulativeGoldStealingCounterP2.ToString());
                    LiczbaPrzelewowGolda_Player2--;
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
            if(EndOfGame == false)
            {
                if(Player2ActualHPValue <= 0)
                {
                    print("Player 2 Actual HP = "+Player2ActualHPValue);
                    IsGameEnded = true;
                    ShowEndGameResultWindow(winner:"Player1");
                    _temporaryIntakeDamage_Player2 = 0;
                }
            }
            var p2hp = GameObject.Find("HealthTextPlayer2").GetComponent<TextMeshProUGUI>();
            if (value > 0)
            {
                print("value: " + value);

                print("różnica : " + (TemporaryIntakeDamage_Player2 - value).ToString());

                p2hp.SetText("-" + _temporaryIntakeDamage_Player2.ToString());
                p2hp.color = Color.red;
                liczbaPrzelewaniaObrazen_Player2++;

            }

            if (value < 0)
            {
                print("value: " + value);

                print("różnica : " + (TemporaryIntakeDamage_Player2 - value).ToString());

                p2hp.SetText("+" + _temporaryIntakeDamage_Player2.ToString());
                p2hp.color = Color.green;
                liczbaPrzelewaniaObrazen_Player2--;
            }

            if (value == 0)
            {
                p2hp.SetText("");
            }
        }
    }

    public int CumulativeGoldStealingCounterP2 { get => _cumulativeGoldStealingCounterP2; set => _cumulativeGoldStealingCounterP2 = value; }
    public bool IsGameEnded { get => _isGameEnded; set => _isGameEnded = value; }

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

       Player1GodsManagerScript = Player1GodSkillWindow.GetComponent<GodsManager>();
       Player2GodsManagerScript = Player2GodSkillWindow.GetComponent<GodsManager>();

    }
    void Update()
    {
        Time.timeScale = GameSpeedValueModifier;

        ManageOrderingRollButtonsAndActivateLastRollingTurn(Player1_RollingCounter, "Player1");
        ManageOrderingRollButtonsAndActivateLastRollingTurn(Player2_RollingCounter, "Player2");

        time += Time.deltaTime;
        time2 += Time.deltaTime;

        TransferGoldToPlayers(ref time, interpolationPeriod);
        TransferDamageToPlayers(ref time2, interpolationPeriod);
    }

    internal static int GetPlayerGoldValue(string player)
    {
        GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();;
        switch (player)
        {
            case "Player1":
                return GM.CurrentGold1;

            case "Player2":
                return GM.CurrentGold2;
        }

        throw new Exception("Incorrect 'player' name");
    }
    private void TransferGoldToPlayers(ref float timePassedInGame, float timeDelayinSecons)
    {
        if (timePassedInGame >= this.interpolationPeriod)
        {
            // reset czasu do 0 i naliczanie dalej os początku
            timePassedInGame = timePassedInGame - interpolationPeriod;
            //---------------------------------------------------------------------------------------------------------------------------


            if (LiczbaPrzelewowGolda_Player1 > 0)
            {
                // DODAWANIE GOLDA
                CurrentGold1++;
                Player1_GoldVault.text = CurrentGold1.ToString();
                LiczbaPrzelewowGolda_Player1--;
                Player1GodsManagerScript.CollorSkillButtonsIfCanBeUsed();
            }
            else if (LiczbaPrzelewowGolda_Player1 < 0)
            {
                // ODEJMOWANIE GOLDA
                CurrentGold1--;
                Player1_GoldVault.text = CurrentGold1.ToString();
                LiczbaPrzelewowGolda_Player1++;
                Player1GodsManagerScript.CollorSkillButtonsIfCanBeUsed();
            }
            else if (LiczbaPrzelewowGolda_Player1 == 0)
            {
                // ZEROWANIE WARTOSCI TYMCZASOWYCH
                TemporaryGoldVault_player1 = 0;
                LiczbaPrzelewowGolda_Player1 = 0;

                if (CumulativeGoldStealingCounterP1 == 0)
                {
                    var p1coin = GameObject.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>();
                    p1coin.SetText("");
                }
            }

            
            //---------------------------------------------------------------------------------------------------------------------------

            if (LiczbaPrzelewowGolda_Player2 > 0)
            {
                // DODAWANIE GOLDA
                CurrentGold2++;
                Player2_GoldVault.text = CurrentGold2.ToString();
                LiczbaPrzelewowGolda_Player2--;
                Player2GodsManagerScript.CollorSkillButtonsIfCanBeUsed();
            }
            else if (LiczbaPrzelewowGolda_Player2 < 0)
            {
                // ODEJMOWANIE GOLDA
                CurrentGold2 = Convert.ToInt32(Player2_GoldVault.text);
                CurrentGold2--;
                Player2_GoldVault.text = CurrentGold2.ToString();
                LiczbaPrzelewowGolda_Player2++;
                Player2GodsManagerScript.CollorSkillButtonsIfCanBeUsed();
            }
            else if (LiczbaPrzelewowGolda_Player2 == 0)
            {
                // ZEROWANIE WARTOSCI TYMCZASOWYCH
                TemporaryGoldVault_player2 = 0;
                LiczbaPrzelewowGolda_Player2 = 0;

                if (CumulativeGoldStealingCounterP2 == 0)
                {
                    var p2coin = GameObject.Find("CoinTextPlayer2").GetComponent<TextMeshProUGUI>();
                    p2coin.SetText("");
                }
            }
        }
    }

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
                // DAMAGING
                int p1Currenthp = Player1ActualHPValue;
                int p1NewHpValue = p1Currenthp - 1;
                Player1_HPPoints.text = (p1NewHpValue.ToString());

                liczbaPrzelewaniaObrazen_Player1--;
            }
            else if (liczbaPrzelewaniaObrazen_Player1 < 0)
            {
                // HEALING
                int p1Currenthp = Player1ActualHPValue;
                int p1NewHpValue = p1Currenthp + 1;
                Player1_HPPoints.text = (p1NewHpValue.ToString());

                liczbaPrzelewaniaObrazen_Player1++;
            }
            else
            {
                if (liczbaPrzelewaniaObrazen_Player1 == 0)
                {
                    TemporaryIntakeDamage_Player1 = 0;
                }
            }



            if (liczbaPrzelewaniaObrazen_Player2 > 0)
            {
                int p2Currenthp = Player2ActualHPValue;
                int p2NewHpValue = p2Currenthp - 1;

                Player2_HPPoints.text = (p2NewHpValue.ToString());

                liczbaPrzelewaniaObrazen_Player2--;
            }
            else if (liczbaPrzelewaniaObrazen_Player2 < 0)
            {
                // HEALING
                int p2Currenthp = Player2ActualHPValue;
                int p2NewHpValue = p2Currenthp + 1;
                Player2_HPPoints.text = (p2NewHpValue.ToString());

                liczbaPrzelewaniaObrazen_Player2++;
            }
            else
            {
                if (liczbaPrzelewaniaObrazen_Player2 == 0)
                {
                    TemporaryIntakeDamage_Player2 = 0;
                }
            }

        }
    }

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

    void SwapRollButonWithEndTurn_OnClick(string playerName)
    {
        GameObject.Find(playerName).transform.Find("EndTurnButton").SetSiblingIndex(2);
    }

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

            GameObject.Find("ANDROID_TEST_STARTCOMBATROUTINE").GetComponent<Button>().interactable = true;
        }
    }

    public void ChangeUIToRollingMode()
    {
        if (IsBattleModeTurnOn == true)
        {
            // 0. nazwanie aktualnego etapu gry
            currentGamePhase = "Dice Rolling Mode";

            // 1. zmniejszenie battlegroundu
            var battlefieldRT = BattleField.GetComponent<RectTransform>();
            battlefieldRT.sizeDelta = new Vector2(battlefieldRT.sizeDelta.x, battlefieldRT.sizeDelta.y / 3.2f);

            // 2. wyzerowanie numeru tury i liczby losowan przez graczy
            TurnNumber = 0;
            Player1_RollingCounter = 0;
            Player2_RollingCounter = 0;

            // 3. zmiana trybu battlemade na OFF
            IsBattleModeTurnOn = false;

            // 4. odkrycie buttonków rolla i końca tury dla graczy
            GameObject.Find("Player1").transform.Find("EndTurnButton").gameObject.SetActive(true);
            GameObject.Find("Player2").transform.Find("EndTurnButton").gameObject.SetActive(true);

            GameObject.Find("Player1").transform.Find("Roll Button").gameObject.SetActive(true);
            GameObject.Find("Player2").transform.Find("Roll Button").gameObject.SetActive(true);

            // 5. odblokowanie kostek na "ręce" 
            // ----> poprzez odblokowanie 
            // ---> wrócenie sie kostek z pola bitwy
            // --> zablokowanie piknięcia kostki bez rolla
            var p1MainDices = GameObject.Find("Player1").transform.Find("DiceHolder").GetComponentsInChildren<DiceRollScript>();
            foreach (var dice in p1MainDices)
            {
                dice.IsAbleToPickup = true;
                dice.IsSentToBattlefield = false;
                dice.RollingIsCompleted = false;
            }
            var p2MainDices = GameObject.Find("Player2").transform.Find("DiceHolder").GetComponentsInChildren<DiceRollScript>();
            foreach (var dice in p2MainDices)
            {
                dice.IsAbleToPickup = true;
                dice.IsSentToBattlefield = false;
                dice.RollingIsCompleted = false;
            }

            // 6. pokazanie sie paneli "blokady tury"
            ChangePlayersTurn();

            // 7. przywrócenie opcji losowania ( zamiana miejscami z guzikiem konca tury )
            GameObject.Find("Player1").transform.Find("EndTurnButton").SetSiblingIndex(1);
            GameObject.Find("Player2").transform.Find("EndTurnButton").SetSiblingIndex(1);

            // 8. przycisk zakonczenia walki i powrotu zostane dezaktywowany
            GameObject.Find("ANDROID_TEST_ENDCOMBATANDBACKTOROLL").GetComponent<Button>().interactable = false;
            GameObject.Find("ANDROID_TEST_STARTCOMBATROUTINE").GetComponent<Button>().interactable = false;
        }
    }
  
    public void OnClick_OpenGodSkillsWindow(string playerName)
    {
        switch (playerName)
        {
            case "Player1":
                Player1GodSkillWindow.SetActive(!Player1GodSkillWindow.gameObject.activeSelf);
                Player1UseSkillTestButton.SetActive(!Player1UseSkillTestButton.gameObject.activeSelf);
                break;

            case "Player2":
                Player2GodSkillWindow.SetActive(!Player2GodSkillWindow.gameObject.activeSelf);
                Player2UseSkillTestButton.SetActive(!Player2UseSkillTestButton.gameObject.activeSelf);
                break;
        }
    }
   
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

    public void OnClick_PlayAgain()
    {
        //TODO: rozkminic inaczej i dodac przejscie do menu tez do nowej gry\
            
        Player1_HPPoints.text = "10";
        Player2_HPPoints.text = "10";

        CurrentGold1 = 0;
        CurrentGold2 = 0;

        GameObject.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>().SetText("");
        GameObject.Find("CoinTextPlayer2").GetComponent<TextMeshProUGUI>().SetText("");

        GameObject.Find("FightZone").GetComponent<CombatManager>().IndexOfCombatAction = 0;

        ChangeUIToRollingMode();  
        IsGameEnded = false;

        EndGameResultWindows.transform.Find("WIN").transform.gameObject.SetActive(false);
        EndGameResultWindows.transform.Find("LOSE").transform.gameObject.SetActive(false);

        //TODO: dodac reset aktywnego skilla.

        //TODO: przerolowanie bogów.
    }
}