using System.Diagnostics.Tracing;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] public Player Player_1;
    [SerializeField] public Player Player_2;

    #region GENERAL   
    [SerializeField] private bool _isGameEnded = false;
    [SerializeField] public static float GameSpeedValueModifier = 100;
    [SerializeField] public GameObject EndGameResultWindows;
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

                //......Player_1.TurnBlocker.SetActive(false);
                //......Player_2.TurnBlocker.SetActive(false);

                GameObject.Find("Player1").transform.Find("EndTurnButton").gameObject.SetActive(false);
                GameObject.Find("Player2").transform.Find("EndTurnButton").gameObject.SetActive(false);

                GameObject.Find("Player1").transform.Find("Roll Button").gameObject.SetActive(false);
                GameObject.Find("Player2").transform.Find("Roll Button").gameObject.SetActive(false);
            }
        }
    }
    float time = 0.0f, time2 = 0.0f;
    [SerializeField] public string CurrentPlayer;
    //  [SerializeField] public string LastPlayerWhoRollingBeforeBattle = "";
    //  [SerializeField] public string PlayerWhoFirstStartRollingInCurrentGameSession = "";
    [SerializeField] public string LastGameWinner;

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
                Player_1.DiceManager.AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES = false;
                Player_2.DiceManager.AFTER_ROLL_AUOMATIC_SELECT_ALL_LEFT_DICES = false;
            }

            if (TurnNumber >= 4.5f)
            {
                Player_1.TurnBlocker.SetActive(false);
                Player_2.TurnBlocker.SetActive(false);
            }
        }
    }
    public bool IsGameEnded
    {
        get => _isGameEnded;
        set
        {
            _isGameEnded = value;
            //......Player_1.TurnBlocker.SetActive(false);
            //......Player_2.TurnBlocker.SetActive(false);
            if(value == true)
            {
                StartCoroutine(CoroutineWcisnijNowaGraPo2SekundachPrzerwy());
            }
        }
    }
    #endregion

    IEnumerator CoroutineWcisnijNowaGraPo2SekundachPrzerwy(){
        yield return new WaitForSecondsRealtime(0.2f);
                OnClick_PlayAgain();
    }

    public CombatManager CombatManager_Script;
    public void ShowEndGameResultWindow(string winner)
    {
        if (winner == "Player2")
        {
            AndroidLogger.Log("Wygrana gracza : "+winner,AndroidLogger.GetPlayerLogColor(winner));
            EndGameResultWindows.transform.Find("WIN").transform.gameObject.SetActive(true);
            
        }
        else
        {
            AndroidLogger.Log("Wygrana gracza : "+winner,AndroidLogger.GetPlayerLogColor(winner));
            EndGameResultWindows.transform.Find("LOSE").transform.gameObject.SetActive(true);
        }
    }

    void Start()
    {
        // pierwsze przypisanie gracza, zostanie zmieniony na następnego defacto zaczynać będzie gracz 2
        CurrentPlayer = "Player1";
        // LastPlayerWhoRollingBeforeBattle = "Player1";
        currentGamePhase = "Dice Rolling Mode";
        TurnNumber = 0;

        CombatManager_Script = GameObject.Find("FightZone").GetComponent<CombatManager>();
        // faktyczny start ze zmianą tury, zostanie rpzypisany gracz2 jako zaczynający
        ChangePlayersTurn();
    }
    void FixedUpdate()
    {
        Time.timeScale = GameSpeedValueModifier;

        ManageOrderingRollButtonsAndActivateLastRollingTurn(Player_1.RollingCounter, "Player1");
        ManageOrderingRollButtonsAndActivateLastRollingTurn(Player_2.RollingCounter, "Player2");

        time += Time.deltaTime;
        time2 += Time.deltaTime;

        TransferGoldToPlayers(ref time, interpolationPeriod);
        TransferDamageToPlayers(ref time2, interpolationPeriod);
    }

    internal static int GetPlayerGoldValue(string player)
    {
        GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>(); ;
        switch (player)
        {
            case "Player1":
                return GM.Player_1.CurrentGold_Value;

            case "Player2":
                return GM.Player_2.CurrentGold_Value;
        }

        throw new Exception("Incorrect 'player' name");
    }
    private void TransferGoldToPlayers(ref float timePassedInGame, float timeDelayinSecons)
    {
        if (timePassedInGame >= this.interpolationPeriod)
        {
            timePassedInGame = timePassedInGame - interpolationPeriod;

            Player_1.TransferGold();
            Player_2.TransferGold();
        }
    }

    private void TransferDamageToPlayers(ref float timePassedInGame, float timeDelayinSecons)
    {
        if (timePassedInGame >= this.interpolationPeriod)
        {
            timePassedInGame = timePassedInGame - interpolationPeriod;

            Player_1.TransferDamage();
            Player_2.TransferDamage();
        }
    }

    void ManageOrderingRollButtonsAndActivateLastRollingTurn(int rollingTurnNumber, string player)
    {
        if (rollingTurnNumber >= 3.0)
        {
            var player2Object = GameObject.Find(player).transform;
            var rollButtonObject = player2Object.Find("Roll Button").transform;

            if (rollingTurnNumber >= 3.0)
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
            Player_1.RollingCounter++;
            CurrentPlayer = "Player2";
        }
        else
        {
            Player_2.RollingCounter++;
            CurrentPlayer = "Player1";
        }

        if(PlayerWhoMakeFirstRollInCurrentRound == "")
        {
            // zapisanie tylko nazwy gracza ktory zaczal jako 1 w rundzie
            PlayerWhoMakeFirstRollInCurrentRound=CurrentPlayer;
        }


        

        if (!IsBattleModeTurnOn)
        {
            if (CurrentPlayer == "Player1")
            {
                Player_1.TurnBlocker.SetActive(false);
                Player_2.TurnBlocker.SetActive(true);
            }
            else
            {
                Player_1.TurnBlocker.SetActive(true);
                Player_2.TurnBlocker.SetActive(false);
            }
        }

        if(PlayerWhoMakeFirstRollinCurrentGameSession == "") PlayerWhoMakeFirstRollinCurrentGameSession = CurrentPlayer;
    }

    internal void SwapRollButonWithEndTurn_OnClick(string playerName)
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
            Player_1.ListOfDicesOnBattleground.Clear();
            Player_2.ListOfDicesOnBattleground.Clear();

            currentGamePhase = "Battle: Phase 1 -> ''sorting dices''";

            var battlefieldRT = BattleField.GetComponent<RectTransform>();
            battlefieldRT.sizeDelta = new Vector2(battlefieldRT.sizeDelta.x, battlefieldRT.sizeDelta.y * 3.2f);

            //......Player_1.TurnBlocker.SetActive(false);
            //......Player_2.TurnBlocker.SetActive(false);

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

            CombatManager_Script.StartFightCoroutine();
           // GameObject.Find("ANDROID_TEST_STARTCOMBATROUTINE").GetComponent<Button>().interactable = true;
        }
    }

    [SerializeField] public string PlayerWhoMakeFirstRollinCurrentGameSession = "";
    [SerializeField] public string PlayerWhoMakeFirstRollInCurrentRound = "";
    public void ChangeUIToRollingMode()
    {
        
            AndroidLogger.Log_Which_Player_Attack_First_and_how_many_rounds(
                whoStartGameSession:            PlayerWhoMakeFirstRollinCurrentGameSession,
                whoAttackedFirstInThisRund:     CombatManager_Script.RecentAttacker,
                whoRollinCurrentRund:           CurrentPlayer,
                numberOfRund:                   rundCounter.ToString(),
                winner:                         LastGameWinner
                );
            
            PlayerWhoMakeFirstRollInCurrentRound = "";
            rundCounter++;

        if (IsBattleModeTurnOn == true)
        {
            Player_1.DiceManager.transform.parent.GetComponentInParent<EnemyAI>().FirstRoll = true;
            Player_2.DiceManager.transform.parent.GetComponentInParent<EnemyAI>().FirstRoll = true;

            // 0. nazwanie aktualnego etapu gry
            currentGamePhase = "Dice Rolling Mode";

            // 1. zmniejszenie battlegroundu
            var battlefieldRT = BattleField.GetComponent<RectTransform>();
            battlefieldRT.sizeDelta = new Vector2(battlefieldRT.sizeDelta.x, battlefieldRT.sizeDelta.y / 3.2f);

            // 2. wyzerowanie numeru tury i liczby losowan przez graczy
            TurnNumber = 0;
            Player_1.RollingCounter = 0;
            Player_2.RollingCounter = 0;

            // 3. zmiana trybu battlemade na OFF
            IsBattleModeTurnOn = false;

            // 4. odkrycie buttonków rolla i końca tury dla graczy
            Player_1.TurnBlocker.SetActive(true);
            Player_2.TurnBlocker.SetActive(true);

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


            // 7. przywrócenie opcji losowania ( zamiana miejscami z guzikiem konca tury )
            GameObject.Find("Player1").transform.Find("EndTurnButton").SetSiblingIndex(1);
            GameObject.Find("Player2").transform.Find("EndTurnButton").SetSiblingIndex(1);

            ChangePlayersTurn();
        }
    }

    public void OnClick_OpenGodSkillsWindow(string playerName)
    {
        switch (playerName)
        {
            case "Player1":
                Player_1.GodSkillWindow.SetActive(!Player_1.GodSkillWindow.gameObject.activeSelf);
                Player_1.UseSkillTestButton.SetActive(!Player_1.UseSkillTestButton.gameObject.activeSelf);
                break;

            case "Player2":
                Player_2.GodSkillWindow.SetActive(!Player_2.GodSkillWindow.gameObject.activeSelf);
                Player_2.UseSkillTestButton.SetActive(!Player_2.UseSkillTestButton.gameObject.activeSelf);
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

    [SerializeField] int rundCounter = 1;
    public void OnClick_PlayAgain()
    {
        // wyłączamy AI zeby nie wlaczyly sie przed czasem włączymy je w roll modzie spowrotem
        if (GameObject.Find("Player1").GetComponent<EnemyAI>().IsTurnON == true) GameObject.Find("Player1").GetComponent<EnemyAI>().IsTurnON = false;
        if (GameObject.Find("Player2").GetComponent<EnemyAI>().IsTurnON == true) GameObject.Find("Player2").GetComponent<EnemyAI>().IsTurnON = false;

        CombatManager_Script.BackDicesToHand();

        Player_1.CurrentHealth_Value = 1000;
        Player_2.CurrentHealth_Value = 1000;

        Player_1.CurrentGold_Value = 0;
        Player_2.CurrentGold_Value = 0;

        Player_1.coinText_TMP.SetText("");
        Player_2.coinText_TMP.SetText("");

        // CurrentPlayer = PlayerWhoFirstStartRollingInCurrentGameSession == "Player1"?"Player2":"Player1"; // <- dzieki temu w tej rundzie zacznie druga osoba
        // PlayerWhoFirstStartRollingInCurrentGameSession = "";
        ChangeUIToRollingMode();
 //dodanie pustej linijki do logó po każdej grze / dla przejrzystosci
        AndroidLogger.Log_Which_Player_Attack_First_and_how_many_rounds();
        rundCounter = 1;
        LastGameWinner = "";
        PlayerWhoMakeFirstRollinCurrentGameSession = "";

        EndGameResultWindows.transform.Find("WIN").transform.gameObject.SetActive(false);
        EndGameResultWindows.transform.Find("LOSE").transform.gameObject.SetActive(false);

        IsGameEnded = false;
       
        GameObject.Find("Player1").GetComponent<EnemyAI>().IsTurnON = true;
        GameObject.Find("Player2").GetComponent<EnemyAI>().IsTurnON = true;

        //TODO: przerolowanie bogów.
    }
}