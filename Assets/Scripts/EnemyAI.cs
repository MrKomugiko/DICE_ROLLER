using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DiceRoller_Console;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] List<string> debugShowDict = new List<string>();
    [SerializeField] bool isMockedDataInitiated = false;
    [SerializeField] bool IsTurnON = false;
    [SerializeField] Player AI_Player;
    [SerializeField] Player HUMAN_Player;
    [SerializeField] bool IsRollAllowed = true;
    [SerializeField] bool calculationsCompleted = false;

    Dictionary<int, string> dicesDict = new Dictionary<int, string>();          // diceNumber <1;6> / "nazwa kostki"
    Dictionary<int, int> pickProbablityDict = new Dictionary<int, int>();       // diceNumber <1;6> / prawdopodobieństwo bycia wybraną <1;100> 
    Dictionary<int, int> pickChanceForDicesDict = new Dictionary<int, int>();   // diceNumber <1;6> / losowa wartość do wybrania <1;100>
    private void AutomaticPopulateDicesInDictList(List<DiceRollScript> dices)
    {
        if(!RollingIsCompleted) return;
        if(isMockedDataInitiated) return;
        isMockedDataInitiated = true;

        for (int i = 1; i <= 6; i++)
        {
            dicesDict.Add(key: i, value: dices.Where(d=>d.DiceNumber == i).FirstOrDefault().name.Remove(0,2));
           // print(i + " / "+ dices.Where(d=>d.DiceNumber == i).FirstOrDefault().name.Remove(0,2));
        }
    }
    private void CalculatePickingValuesForOwnedDices()
    {
        if(!isMockedDataInitiated) return;
        if(calculationsCompleted) return;
        calculationsCompleted = true;
        foreach(KeyValuePair<int,string> dice in dicesDict)
        {
            int pickValue = 40; // TODO: Tutaj bedzie dziać sie magia xDD
            if(!pickProbablityDict.ContainsKey(dice.Key))
            {
                // jezeli nie zawiera numeru kostki więc jest to pewnie 1 tura, dodaj ją do dict'a
                pickProbablityDict.Add(key: dice.Key, value: pickValue);
            }
            else
            {
                pickValue = 50;
                if(pickProbablityDict[dice.Key] == pickValue)
                {
                    pickValue += 10;
                }
                // posiada juz ten klicz w dictie , nadpisz go nową wartością
                pickProbablityDict[dice.Key] = pickValue;
            }
        }

       // debugShowDict = new List<string>(); // czyszczenie wczesniejszego wpisu
        foreach (KeyValuePair<int, string> AI_Dice in dicesDict)
        {
            debugShowDict.Add(
                $"Dice:{AI_Dice.Key}\t"+
                $"Rolled value:[{pickChanceForDicesDict.Where(p => p.Key == AI_Dice.Key).First().Value.ToString("00")}] "+
                $"Pick value: {pickProbablityDict.Where(p => p.Key == AI_Dice.Key).First().Value.ToString("00")}]\t"+
                $"Dice name: {dicesDict.Where(d=>d.Key == AI_Dice.Key).First().Value}");
        }
        debugShowDict.Add("----------------------------------");
        foreach (string log in debugShowDict)
        {
            AndroidLogger.Log(log,AndroidLogger.GetPlayerLogColor(AI_Player.Name));
        }
    }
    [SerializeField] public  bool FirstRoll = true;
    [SerializeField] bool SecondRoll = false;
    [SerializeField] bool ThirdRoll = false;
    [SerializeField] bool FourthRoll = false;

    void FixedUpdate()
    {
        if (IsTurnON && !AI_Player.TurnBlocker.activeSelf && FirstRoll)
        {
            RollDices();

            if (RollingIsCompleted)
            {
                GeneratePickChanceValuesForDices();
                AutomaticPopulateDicesInDictList(AI_Player.DiceManager.Dices);
                CalculatePickingValuesForOwnedDices();
                List<int> numberOFPickedDices = new List<int>();
                // for test pick all dices
                foreach (KeyValuePair<int, string> dice in dicesDict)
                {
                    if (CheckIfDiceShouldBePicked(diceNumber: dice.Key))
                    {
                        PickDice(diceNumber: dice.Key);
                        numberOFPickedDices.Add(dice.Key);
                    }
                }
                RemoveUsedDicesFromDictMemory(numberOFPickedDices);
                EndTurn(1);
            }
        }
        if (IsTurnON && !AI_Player.TurnBlocker.activeSelf && SecondRoll)
        {
            RollDices();
            if (RollingIsCompleted)
            {
                isMockedDataInitiated = true;
                calculationsCompleted = false;
                itsNeedToReCalculatePickChanceValues = true;

                print("calculating round TWO");
                GeneratePickChanceValuesForDices();
                CalculatePickingValuesForOwnedDices();
                List<int> numberOFPickedDices = new List<int>();
                foreach (KeyValuePair<int, string> dice in dicesDict)
                {
                    if (CheckIfDiceShouldBePicked(diceNumber: dice.Key))
                    {
                        PickDice(diceNumber: dice.Key);
                        numberOFPickedDices.Add(dice.Key);
                    }
                }
                RemoveUsedDicesFromDictMemory(numberOFPickedDices);
                EndTurn(2);
            }
        }
        if (IsTurnON && !AI_Player.TurnBlocker.activeSelf && ThirdRoll)
        {
            RollDices();

            if (RollingIsCompleted)
            {
                print("calculating round THREE");
                GeneratePickChanceValuesForDices();
                CalculatePickingValuesForOwnedDices();
                List<int> numberOFPickedDices = new List<int>();
                foreach (KeyValuePair<int, string> dice in dicesDict)
                {
                    if (CheckIfDiceShouldBePicked(diceNumber: dice.Key))
                    {
                        PickDice(diceNumber: dice.Key);
                        numberOFPickedDices.Add(dice.Key);
                    }
                }
                RemoveUsedDicesFromDictMemory(numberOFPickedDices);
                EndTurn(3);
            }
        }
        if (IsTurnON && !AI_Player.TurnBlocker.activeSelf && FourthRoll)
        {
            RollDices();
            if (RollingIsCompleted)
            {   
                print("Final round FOUR - auto pick last dices");
                EndTurn(4);
            }
        }
    }

    private void RemoveUsedDicesFromDictMemory(List<int> numberOFPickedDices)
    {
        foreach(int usedDice in numberOFPickedDices)
        {
            dicesDict.Remove(usedDice);
        }
         print("Pikniętych kostek:"+numberOFPickedDices.Count()+"/ usuniecie kostek z pamieci, aktualnie zostało:"+dicesDict.Count());
    }

    private bool CheckIfDiceShouldBePicked(int diceNumber)
    {
        int dicePickRequirments = pickProbablityDict.Where(d=>d.Key == diceNumber).First().Value;
        int rolledPickValue = pickChanceForDicesDict.Where(d=>d.Key == diceNumber).First().Value; 
        print($"CheckIfDiceShouldBePicked for dice:{diceNumber}. [{rolledPickValue}<{dicePickRequirments}]");

        if(rolledPickValue<dicePickRequirments)
        {
           /* 
            * Note:
            * ~ szansa na wylosowanie kostki to 75%, 
            * ~`wylosowana wartość wynosi 60,
            * => żeby kostka zostałą wybrana akceptowalne są wszytskie wartości 
            *      poniżej lub jej równe - szansy(75%)
            *      więc jeżeli watość(60) < szansa na wybranie(75)
            *      w przeciwnym wypadku mamy gdy wartość jest większa niż szansa 
            *      czyli reszta (25%) jest pomijane i zostane na następną runde
            */

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
            if(!pickChanceForDicesDict.ContainsKey(i))
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

        if(diceButton.IsInteractable()) diceButton.onClick.Invoke();
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

    }

}
