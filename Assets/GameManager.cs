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

    private static int goldBonusesCounter_P1;
    private static int goldBonusesCounter_P2;
    [SerializeField] private static int player1TotalGoldFavorEaned;
    [SerializeField] private static int player2TotalGoldFavorEaned;

    public static int Player1TotalGoldFavorEaned
    {
        get => player1TotalGoldFavorEaned;
        set
        {
            goldBonusesCounter_P1++;
            if (goldBonusesCounter_P1 == GameObject.Find("Battlefield").transform.Find("Player1Dices").transform.GetComponentsInChildren<DiceActionScript>().Where(d => d.name.Contains("Blessed") == true).Count())
            {
                player2TotalGoldFavorEaned += value;
                UpdatePlayersGoldStat(player1TotalGoldFavorEaned, goldBonusesCounter_P1, "Player1");
                goldBonusesCounter_P1 = 0;
            }
        }
    }
    public static int Player2TotalGoldFavorEaned
    {
        get => player2TotalGoldFavorEaned;
        set
        {
            goldBonusesCounter_P2++;
            if (goldBonusesCounter_P2 == GameObject.Find("Battlefield").transform.Find("Player2Dices").transform.GetComponentsInChildren<DiceActionScript>().Where(d => d.name.Contains("Blessed") == true).Count())
            {
                player2TotalGoldFavorEaned += value;
                UpdatePlayersGoldStat(player2TotalGoldFavorEaned, goldBonusesCounter_P2, "Player2");
                goldBonusesCounter_P2 = 0;
            }
        }
    }

    public static void UpdatePlayersGoldStat(int totalValue, int value, string player)
    {
        var bf = GameObject.Find("Battlefield");
        // policzenie ile blessed obiektów jest na arenie

        // sprawdzenie czy każda kostka dodała swoją wartośc do puli
        if (player == "Player1")
        {
            // aktualizowanie znaczników +xx 
            bf.transform.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>().SetText("+" + value.ToString());
            // aktualizowanie głównych wartości golda dla graczy
            GameObject.Find("Player1").transform.Find("GoldPoints").GetComponent<TextMeshProUGUI>().SetText(totalValue.ToString());
        }
        else
        {
            bf.transform.Find("CoinTextPlayer2").GetComponent<TextMeshProUGUI>().SetText("+" + Player2TotalGoldFavorEaned.ToString());

            GameObject.Find("Player2").transform.Find("GoldPoints").GetComponent<TextMeshProUGUI>().SetText(Player2TotalGoldFavorEaned.ToString());
        }
    }
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

    [SerializeField] float _turnNumber;

    [SerializeField] GameObject Player1TurnBlocker;
    [SerializeField] int Player1_RollingCounter;
    [SerializeField] bool Player1_LastRollWithAutomaticWithdraw;


    [SerializeField] GameObject Player2TurnBlocker;
    [SerializeField] int Player2_RollingCounter;
    [SerializeField] bool Player2_LastRollWithAutomaticWithdraw;


    [SerializeField] string CurrentPlayer;

    private float time = 0.0f;
    [SerializeField] public float interpolationPeriod = .5f;
    [SerializeField] private int currentGold1 = 0;
    [SerializeField] TextMeshProUGUI Player1_GoldVault;
    [SerializeField] private int currentGold2 = 0;
    [SerializeField] TextMeshProUGUI Player2_GoldVault;

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

            if (liczbaOperacjiDodaniaGolda_player1 > 0)
            {
                if (liczbaOperacjiDodaniaGolda_player1 == 1)
                {
                    TemporaryGoldVault_player1 = 0;
                }
                currentGold1++;
                Player1_GoldVault.SetText(currentGold1.ToString());
                liczbaOperacjiDodaniaGolda_player1--;
            }

            if (liczbaOperacjiDodaniaGolda_player2 > 0)
            {
                if (liczbaOperacjiDodaniaGolda_player2 == 1)
                {
                    TemporaryGoldVault_player2 = 0;
                }
                currentGold2++;
                Player2_GoldVault.SetText(currentGold2.ToString());
                liczbaOperacjiDodaniaGolda_player2--;
            }

        }
    }
    private void ManageOrderingRollButtonsAndActivateLastRollingTurn(int rollingTurnNumber, string player)
    {
        if (rollingTurnNumber >= 3.0)
        {
            var player2Object = GameObject.Find("GameCanvas").transform.Find(player).transform;
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
    [SerializeField] public static List<Image> OnBattlefield_Dice_Player1 = new List<Image>();
    [SerializeField] public static List<Image> OnBattlefield_Dice_Player2 = new List<Image>();
    void ChangeUIToBattleMode()
    {
        if (isBattleModeTurnOn == false)
        {
            currentGamePhase = "Battle: Phase 1 -> ''sorting dices''";
            // DONE zmiana wielkości areny przelicznik 3.2x wysokosc
            var battlefieldRT = BattleField.GetComponent<RectTransform>();
            battlefieldRT.sizeDelta = new Vector2(battlefieldRT.sizeDelta.x, battlefieldRT.sizeDelta.y * 3.2f);

            //DONE ukrycie paneli przyciemniajacych - sygnalizowanie ktory gracz ma ture
            Player1TurnBlocker.SetActive(false);
            Player2TurnBlocker.SetActive(false);

            // DONE ? albo w trakcie )1 posortowanie kosci na planszy ? deff / attack / steal

            // wybranie skilla bozka jezeli to mozliwe

            //  posortuj kosci według typu
            //    print("player 1 and 2 sorted start");

            // GameObject.Find("Player1Dices").GetComponent<DiceSorterScript>().NeedToSort = true;
            // GameObject.Find("Player2Dices").GetComponent<DiceSorterScript>().NeedToSort = true;

            // przyzna kazzdej ze strony golda
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
            // print(dice.name+" player1");
            // a skrypt w kostce sprawdzi do kogo nalezy i czy jest "pobłogosławiona"
            //  oraz zostanie wywołana animacja i podsumowanie
            //  print(dice.GetComponentInParent<DiceActionScript>().name+ "Player 1 dice");
            //     dice.GetComponentInParent<DiceActionScript>().AddGoldFromBlessedItems = true; 
            isBattleModeTurnOn = true;
        }

        // foreach (var dice in GameObject.Find("Player2Dices").GetComponent<DiceSorterScript>().DicesOnBattleground)
        // {
        //    // print(dice.name+" player2");
        //     // a skrypt w kostce sprawdzi do kogo nalezy i czy jest "pobłogosławiona"
        //     //  oraz zostanie wywołana animacja i podsumowanie
        //     dice.GetComponentInParent<DiceActionScript>().AddGoldFromBlessedItems = true; 

        // }


        // pojedyńcze atakowanie, jeżeli napotka swojego defa to przeciwnik nie otrzymuje obrażeń
        // - helm to obona na topór
        // - tarcza obona na łuk
        // atak 2giego gracza

        // kradziez gracza 1 , potem 2
        //throw new NotImplementedException();

    }
    IEnumerable<DiceActionScript> blessedDP1, blessedDP2 = null;
    [SerializeField] private int _temporaryGoldVault_player1;
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
        }
    }

    [SerializeField] private int _temporaryGoldVault_player2;
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
        }
    }
    private int liczbaOperacjiDodaniaGolda_player1;
    private int liczbaOperacjiDodaniaGolda_player2;
    public void AddGoldToPlayerVault(string playerName, int value)
    {
        if (playerName == "Player1")
        {
            liczbaOperacjiDodaniaGolda_player1 += value;
        }
        else
        {
            liczbaOperacjiDodaniaGolda_player2 += value;
        }
    }



















    public void ANDROID_AgainAddGold()
    {
        var dices_P1 = GameObject.Find("Player1Dices").GetComponentsInChildren<DiceActionScript>();
        blessedDP1 = dices_P1.Where(d => d.name.Contains("Blessed"));
        var dices_P2 = GameObject.Find("Player2Dices").GetComponentsInChildren<DiceActionScript>();
        blessedDP2 = dices_P2.Where(d => d.name.Contains("Blessed"));

        GameObject.Find("ANDROID_TEST_GOLDBUTTON").GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>()
           .SetText($"{blessedDP1.Count()} <------> {blessedDP2.Count()}");

        GameObject.Find("ANDROID_TEST_GOLDBUTTON").GetComponent<Button>().interactable = false;
        if (dices_P1.Count() == 6 || dices_P2.Count() == 6)
        {
            GameObject.Find("ANDROID_TEST_GOLDBUTTON").GetComponent<Button>().interactable = true;
        }
        foreach (var dice in dices_P1)
        {
            dice.AddGoldFromBlessedItems = true;
        }
        foreach (var dice in dices_P2)
        {
            dice.AddGoldFromBlessedItems = true;
        }
    }
}

