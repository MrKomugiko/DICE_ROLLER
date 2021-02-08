using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DiceRoller_Console;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] List<string> debugShowDict = new List<string>();
    [SerializeField] bool isMockedDataInitiated = false;
    [SerializeField] bool IsTurnON = false;
    [SerializeField] Player AI_Player;
    [SerializeField] Player HUMAN_Player;
    [SerializeField] bool IsRollAllowed = true;
    [SerializeField] bool calculationsCompleted = false;
    [SerializeField] AIPickChanceUi UI;

    Dictionary<int, string> dicesDict = new Dictionary<int, string>();          // diceNumber <1;6> / "nazwa kostki"
    public Dictionary<int, int> pickProbablityDict = new Dictionary<int, int>();       // diceNumber <1;6> / prawdopodobieństwo bycia wybraną <1;100> 
    Dictionary<int, int> pickChanceForDicesDict = new Dictionary<int, int>();   // diceNumber <1;6> / losowa wartość do wybrania <1;100>
    void Start()
    {
        UI = this.GetComponentInChildren<AIPickChanceUi>();
    }
    private void AutomaticPopulateDicesInDictList(List<DiceRollScript> dices)
    {
        if (!RollingIsCompleted) return;
        if (isMockedDataInitiated) return;
        isMockedDataInitiated = true;
        debugShowDict.Add($"--------------------- Start new Round ---------------------");
        dicesDict.Clear();
        for (int i = 1; i <= 6; i++)
        {
            dicesDict.Add(key: i, value: dices.Where(d => d.DiceNumber == i).FirstOrDefault().name.Remove(0, 2));
            // print(i + " / "+ dices.Where(d=>d.DiceNumber == i).FirstOrDefault().name.Remove(0,2));
        }
    }
    [ContextMenu("CalculateDicesPickChance")]
    private IEnumerator CalculatePickingValuesForOwnedDices()
    {
        print("CalculatePickingValuesForOwnedDices routine is working");
        calculationsIsCompleted = false;
        if (!isMockedDataInitiated) yield return null;
        if (calculationsCompleted) yield return null;

        calculationsCompleted = true;
        foreach (KeyValuePair<int, string> dice in dicesDict)
        {
            int calculatedPickValue = 0;
            Func<bool> recalculating = () => RecalculatePickValue(dice, out calculatedPickValue);
            yield return new WaitUntil(recalculating);

            if (!pickProbablityDict.ContainsKey(dice.Key))
            {
                // jezeli nie zawiera numeru kostki więc jest to pewnie 1 tura, dodaj ją do dict'a
                pickProbablityDict.Add(key: dice.Key, value: calculatedPickValue);
            }
            else
            {
                // posiada juz ten klicz w dictie , nadpisz go nową wartością
                pickProbablityDict[dice.Key] += 15;
            }
            UI.SetPickChanceValues(diceNumber: dice.Key, dicePickChance: pickProbablityDict[dice.Key]);
        }

        UpdateLogger();
        calculationsIsCompleted = true;
    }

    private bool RecalculatePickValue(KeyValuePair<int, string> dice, out int pickValue)
    {
        print("recalculate dice min pick value");
        // Default value = 25.

        pickValue = 25;
        
        //TODO: "PROCES SPRAWDZANIA I SZACOWANIA OPŁACALNOŚCI WYBORU KOŚCI"

        return true;
    }
    private void UpdateLogger()
    {
        try
        {
            foreach (KeyValuePair<int, string> AI_Dice in dicesDict)
            {
                debugShowDict.Add(
                    $"Dice:{AI_Dice.Key}\t" +
                    $"Rolled value:[{pickChanceForDicesDict.Where(p => p.Key == AI_Dice.Key).First().Value.ToString("000")}] " +
                    $"Pick value: {pickProbablityDict.Where(p => p.Key == AI_Dice.Key).First().Value.ToString("000")}]\t" +
                    $"Dice name: {dicesDict.Where(d => d.Key == AI_Dice.Key).First().Value}");
            }
            debugShowDict.Add("--------------------- End of Turn ---------------------");
            foreach (string log in debugShowDict)
            {
                AndroidLogger.Log(log, AndroidLogger.GetPlayerLogColor(AI_Player.Name));
            }
        }
        catch (Exception ex)
        {
         //   print(ex.Message);
        }
    }

    [SerializeField] public bool FirstRoll = true;
    [SerializeField] bool SecondRoll = false;
    [SerializeField] bool ThirdRoll = false;
    [SerializeField] bool FourthRoll = false;

    IEnumerator calculatingCoroutine = null;
    void FixedUpdate()
    {
        if (IsTurnON && !AI_Player.TurnBlocker.activeSelf && FirstRoll)
        {
            RollDices();

            if (RollingIsCompleted && calculatingCoroutine == null)
            {
                calculatingCoroutine = CalculatingChance(roundNumber: 1);
                StartCoroutine(calculatingCoroutine);
            }
        }
        if (IsTurnON && !AI_Player.TurnBlocker.activeSelf && SecondRoll)
        {
            RollDices();
            if (RollingIsCompleted && calculatingCoroutine == null)
            {
                calculatingCoroutine = CalculatingChance(roundNumber: 2);
                StartCoroutine(calculatingCoroutine);
            }
        }
        if (IsTurnON && !AI_Player.TurnBlocker.activeSelf && ThirdRoll)
        {
            RollDices();

            if (RollingIsCompleted && calculatingCoroutine == null)
            {
                calculatingCoroutine = CalculatingChance(roundNumber: 3);
                StartCoroutine(CalculatingChance(roundNumber: 3));
            }
        }
        if (IsTurnON && !AI_Player.TurnBlocker.activeSelf && FourthRoll)
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

    bool calculationsIsCompleted = false;
    private IEnumerator CalculatingChance(int roundNumber)
    {
        if (roundNumber == 1)
        {
            print("CalculatingChance routine is working");
            isMockedDataInitiated = false;
            GeneratePickChanceValuesForDices();
            AutomaticPopulateDicesInDictList(AI_Player.DiceManager.Dices);
            StartCoroutine(CalculatePickingValuesForOwnedDices());
            yield return new WaitUntil(() => calculationsIsCompleted);

            List<int> numberOFPickedDices = new List<int>();
            // for test pick all dices
            foreach (KeyValuePair<int, string> dice in dicesDict)
            {
                if (CheckIfDiceShouldBePicked(diceNumber: dice.Key))
                {
                    PickDice(diceNumber: dice.Key);
                    numberOFPickedDices.Add(dice.Key);
                    yield return new WaitForSeconds(1f);
                }
            }
            RemoveUsedDicesFromDictMemory(numberOFPickedDices);
            EndTurn(roundNumber);
        }
        else
        {
            print("calculating round " + roundNumber);
            GeneratePickChanceValuesForDices();
            StartCoroutine(CalculatePickingValuesForOwnedDices());
            yield return new WaitUntil(() => calculationsIsCompleted);
            List<int> numberOFPickedDices = new List<int>();
            foreach (KeyValuePair<int, string> dice in dicesDict)
            {
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
    }

    private void RemoveUsedDicesFromDictMemory(List<int> numberOFPickedDices)
    {
        foreach (int usedDice in numberOFPickedDices)
        {
            dicesDict.Remove(usedDice);
        }
        print("Pikniętych kostek:" + numberOFPickedDices.Count() + "/ usuniecie kostek z pamieci, aktualnie zostało:" + dicesDict.Count());
    }

    private bool CheckIfDiceShouldBePicked(int diceNumber)
    {
        int dicePickRequirments = pickProbablityDict.Where(d => d.Key == diceNumber).First().Value;
        int rolledPickValue = pickChanceForDicesDict.Where(d => d.Key == diceNumber).First().Value;
        print($"CheckIfDiceShouldBePicked for dice:{diceNumber}. [{rolledPickValue}<{dicePickRequirments}]");

        if (rolledPickValue < dicePickRequirments)
        {
            return true;
        }
        return false;
    }

    [SerializeField] bool itsNeedToReCalculatePickChanceValues = true;
    private void GeneratePickChanceValuesForDices()
    {
        if (!itsNeedToReCalculatePickChanceValues) return;

        print("GeneratePickChanceValuesForDices");

        for (int i = 1; i <= 6; i++)
        {
            if (!pickChanceForDicesDict.ContainsKey(i))
            {
                pickChanceForDicesDict.Add(key: i, value: RandomNumberGenerator.NumberBetween(1, 100));
            }
            else
            {
                pickChanceForDicesDict[i] = RandomNumberGenerator.NumberBetween(1, 100);
            }
        }

        itsNeedToReCalculatePickChanceValues = false;
    }

    [ContextMenu("Włącz SI")] public void TurnONOFF() => IsTurnON = !IsTurnON;
    private bool RollingIsCompleted => AI_Player.DiceManager.Dices.ElementAt(0).rollingIsCompleted;
    private void RollDices()
    {
        if (!IsRollAllowed) return;

        AI_Player.DiceManager.OnClick_ROLLDICES();
        AI_Player.GameManager.SwapRollButonWithEndTurn_OnClick(AI_Player.Name);
        IsRollAllowed = false;
    }

    private void PickDice(int diceNumber)
    {
        print("PickDice");

        int diceIndex = diceNumber - 1;
        Button diceButton = AI_Player.DiceManager.transform
        .GetChild(diceIndex)
        .GetComponent<Button>();

        if (diceButton.IsInteractable()) diceButton.onClick.Invoke();
    }

    private void EndTurn(int turnNumber)
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
        itsNeedToReCalculatePickChanceValues = true;
        isMockedDataInitiated = true;
        calculationsCompleted = false;

        calculatingCoroutine = null;
    }

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
