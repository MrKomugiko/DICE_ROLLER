using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DiceRoller_Console;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] Player AI_Player, ENEMY_Player;
    [SerializeField] AIPickChanceUi UI;

    [SerializeField] public bool FirstRoll, SecondRoll, ThirdRoll, FourthRoll;
    [SerializeField] bool isMockedDataInitiated, IsTurnON, calculatingNewPickValuesIsCompleted, IsRollAllowed = true, itsNeedToReCalculateRandomPickingValues = true;

    Dictionary<int, string> dicesDict = new Dictionary<int, string>();              // diceNumber <1;6> / "nazwa kostki" 
    Dictionary<int, string> actualLeftDicesOnHand = new Dictionary<int, string>();  // COPY of diceDict
    Dictionary<int, int> pickProbablityDict = new Dictionary<int, int>();           // diceNumber <1;6> / prawdopodobieństwo bycia wybraną <1;100> 
    Dictionary<int, int> randomPickValueForDices = new Dictionary<int, int>();       // diceNumber <1;6> / losowa wartość do wybrania <1;100>

    [SerializeField] List<string> debugShowDict = new List<string>();
    List<DiceRollScript> GetCurrentEnemyDicesInBattlefield => ENEMY_Player.ListOfDicesOnBattleground;
    List<DiceRollScript> GetCurrentDicesInBattlefield => AI_Player.ListOfDicesOnBattleground;
    bool RollingIsCompleted => AI_Player.DiceManager.Dices.ElementAt(0).rollingIsCompleted;

    IEnumerator calculatingCoroutine = null;
    void Start()
    {
        UI = this.GetComponentInChildren<AIPickChanceUi>();
    }
    void FixedUpdate()
    {
        if (IsTurnON && !AI_Player.TurnBlocker.activeSelf)
        {
            if (FirstRoll)
            {
                RollDices();
                if (RollingIsCompleted && calculatingCoroutine == null)
                {
                    isMockedDataInitiated = false;
                    pickProbablityDict.Clear();
                    AutomaticPopulateDicesInDictList(AI_Player.DiceManager.Dices);
                    calculatingCoroutine = CalculatingChance(roundNumber: 1);
                    StartCoroutine(calculatingCoroutine);
                }
            }
            if (SecondRoll)
            {
                RollDices();
                if (RollingIsCompleted && calculatingCoroutine == null)
                {
                    calculatingCoroutine = CalculatingChance(roundNumber: 2);
                    StartCoroutine(calculatingCoroutine);
                }
            }
            if (ThirdRoll)
            {
                RollDices();
                if (RollingIsCompleted && calculatingCoroutine == null)
                {
                    calculatingCoroutine = CalculatingChance(roundNumber: 3);
                    StartCoroutine(CalculatingChance(roundNumber: 3));
                }
            }
            if (FourthRoll)
            {
                RollDices();
                if (RollingIsCompleted)
                {
                    foreach (KeyValuePair<int, string> AI_Dice in dicesDict)
                    {
                        debugShowDict.Add(
                            $"Dice:{AI_Dice.Key}\t" +
                            $"Dice name: {dicesDict.Where(d => d.Key == AI_Dice.Key).First().Value}" +
                            $" -> Automatic last pick.");
                    }
                    print("Final round FOUR - auto pick last dices");
                    EndTurn(4);
                }
            }
        }
    }

    void AutomaticPopulateDicesInDictList(List<DiceRollScript> dices)
    {
        if (!RollingIsCompleted) return;
        if (isMockedDataInitiated) return;
        print("test 4.1 - populate dict with owned dices");
        isMockedDataInitiated = true;
        debugShowDict.Add($"--------------------- Start new Round ---------------------");
        dicesDict.Clear();
        for (int i = 1; i <= 6; i++)
        {
            dicesDict.Add(key: i, value: dices.Where(d => d.DiceNumber == i).FirstOrDefault().name.Remove(0, 2));
            // print(i + " / "+ dices.Where(d=>d.DiceNumber == i).FirstOrDefault().name.Remove(0,2));
        }
    }
    void UpdateLogger(KeyValuePair<int, string> currentCheckingDice)
    {
        debugShowDict.Add(
            $"Dice:{currentCheckingDice.Key}\t" +
            $"Rolled value:[{randomPickValueForDices.Where(p => p.Key == currentCheckingDice.Key).First().Value.ToString("000")}] " +
            $"Pick value: {pickProbablityDict.Where(p => p.Key == currentCheckingDice.Key).First().Value.ToString("000")}]\t" +
            $"Dice name: {dicesDict.Where(d => d.Key == currentCheckingDice.Key).First().Value}");

        AndroidLogger.Log(debugShowDict.Last(), AndroidLogger.GetPlayerLogColor(AI_Player.Name));
    }
    IEnumerator CalculatingChance(int roundNumber)
    {
        PopulateDictWithRandomPickingValue();
        List<int> numberOFPickedDices = new List<int>();
        actualLeftDicesOnHand.Clear();
        foreach (var dice in dicesDict)
        {
            // MAKE A COPY ( not a refference ) to a current owned dices on hand
            // now if i change dhis dictionary , its wont make and error says 
            // enumerator is changed, end of executing, and i can later check 
            // what dices i have in pool to choose 
            actualLeftDicesOnHand.Add(dice.Key, dice.Value);
        }

        foreach (KeyValuePair<int, string> dice in dicesDict)
        {
            foreach (int usedDice in numberOFPickedDices)
            {
                // Deleting already picked dices from copied dictionary
                actualLeftDicesOnHand.Remove(usedDice);
            }

            // execute for every possesed dice one by one 
            var currentCheckingDice = actualLeftDicesOnHand.Where(d => d.Key == dice.Key).FirstOrDefault();
            CalculatePickingValuesForOwnedDices(currentCheckingDice);
            StartCoroutine(CalculatePickingValuesForOwnedDices(currentCheckingDice));
           yield return new WaitUntil(() => calculatingNewPickValuesIsCompleted);

            if (CheckIfDiceShouldBePicked(diceNumber: dice.Key))
            {
                PickDice(diceNumber: dice.Key);
                numberOFPickedDices.Add(dice.Key);
            }
            yield return new WaitForSeconds(1f);
        }

        RemoveUsedDicesFromDictMemory(numberOFPickedDices);
        EndTurn(roundNumber);
    }
    IEnumerator CalculatePickingValuesForOwnedDices(KeyValuePair<int, string> currentCheckingDice)
    {
        calculatingNewPickValuesIsCompleted = false;
        if (!isMockedDataInitiated) yield return null;
        if (calculatingNewPickValuesIsCompleted) yield return null;

        int calculatedPickValue = 0;
        RecalculatePickWeightValueBasedOnLogic(currentCheckingDice, out calculatedPickValue);

        if (!pickProbablityDict.ContainsKey(currentCheckingDice.Key))
        {
            // jezeli nie zawiera numeru kostki więc jest to pewnie 1 tura, dodaj ją do dict'a
            pickProbablityDict.Add(key: currentCheckingDice.Key, value: calculatedPickValue);
        }
        else
        {
            // posiada juz ten klicz w dictie , nadpisz go nową wartością
            pickProbablityDict[currentCheckingDice.Key] = calculatedPickValue;
        }
        UI.SetPickChanceValues(diceNumber: currentCheckingDice.Key, dicePickChance: pickProbablityDict[currentCheckingDice.Key]);

        UpdateLogger(currentCheckingDice);
        calculatingNewPickValuesIsCompleted = true;
    }
    bool RecalculatePickWeightValueBasedOnLogic(KeyValuePair<int, string> dice, out int pickValue)
    {
        // Default value = 25.

        pickValue = RandomNumberGenerator.NumberBetween(1,100);

        // TODO: "PROCES SPRAWDZANIA I SZACOWANIA OPŁACALNOŚCI WYBORU KOŚCI"
        //      uzyj actual left dices zeby przeszukac czy masz na ręce jakieś inne "może lepsze kości" 
        //      i ile sztuk => w razie planowania obrony lub ataku

        print(GetCurrentDicesInBattlefield.Count());
        print(GetCurrentEnemyDicesInBattlefield.Count());

        print("obliczanie opłacalności kości: " + dice.Value + " łącznie na ręce zostało jeszcze " + actualLeftDicesOnHand.Count() + " oplaca się piknąć tą kość w " + pickValue + "%");
        return true;
    }
    void RemoveUsedDicesFromDictMemory(List<int> numberOFPickedDices)
    {
        foreach (int usedDice in numberOFPickedDices)
        {
            dicesDict.Remove(usedDice);
        }
    }
    bool CheckIfDiceShouldBePicked(int diceNumber)
    {
        int dicePickRequirments = pickProbablityDict.Where(d => d.Key == diceNumber).First().Value;
        int rolledPickValue = randomPickValueForDices.Where(d => d.Key == diceNumber).First().Value;

        if (rolledPickValue < dicePickRequirments)
        {
            return true;
        }
        return false;
    }
    void PopulateDictWithRandomPickingValue()
    {
        if (!itsNeedToReCalculateRandomPickingValues) return;

print("dodanie losowych wag pikniecia");
        for (int i = 1; i <= 6; i++)
        {
            if (!randomPickValueForDices.ContainsKey(i))
            {
                randomPickValueForDices.Add(key: i, value: RandomNumberGenerator.NumberBetween(1, 100));
            }
            else
            {
                randomPickValueForDices[i] = RandomNumberGenerator.NumberBetween(1, 100);
            }
        }

        itsNeedToReCalculateRandomPickingValues = false;
    }
    void RollDices()
    {
        if (!IsRollAllowed) return;

        AI_Player.DiceManager.OnClick_ROLLDICES();
        AI_Player.GameManager.SwapRollButonWithEndTurn_OnClick(AI_Player.Name);
        IsRollAllowed = false;
    }
    void PickDice(int diceNumber)
    {
        print("PickDice");

        int diceIndex = diceNumber - 1;
        Button diceButton = AI_Player.DiceManager.transform
        .GetChild(diceIndex)
        .GetComponent<Button>();

        if (diceButton.IsInteractable()) diceButton.onClick.Invoke();
    }
    void EndTurn(int turnNumber)
    {
        switch (turnNumber)
        {
            case 1:
                FirstRoll = false;
                SecondRoll = true;
                break;

            case 2:
                SecondRoll = false;
                ThirdRoll = true;
                break;

            case 3:
                ThirdRoll = false;
                FourthRoll = true;
                break;

            case 4:
                FourthRoll = false;
                break;
        }

        print("EndTurn");
        this.transform.Find("EndTurnButton").GetComponent<Button>().onClick.Invoke();

        IsRollAllowed = true;
        itsNeedToReCalculateRandomPickingValues = true;
        isMockedDataInitiated = true;
        calculatingNewPickValuesIsCompleted = false;

        calculatingCoroutine = null;
    }
    void TurnONOFF() => IsTurnON = !IsTurnON;
    
    public void OnClick_TurnOnAI()
    {
        TurnONOFF();
        if (IsTurnON)
        {
            transform.Find("AIIcon_Button").GetComponent<Image>().color = new Color32(0, 255, 0, 255);
        }
        else
        {
            transform.Find("AIIcon_Button").GetComponent<Image>().color = new Color32(255, 0, 0, 128);
        }
    }
}
