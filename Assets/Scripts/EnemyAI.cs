using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DiceRoller_Console;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] bool isMockedDataInitiated = false;
    [SerializeField] bool IsTurnON = false;
    [SerializeField] Player AI_Player;
    [SerializeField] Player HUMAN_Player;
    [SerializeField] bool IsRollAllowed = true;

    Dictionary<int, string> dicesDict = new Dictionary<int, string>();
    Dictionary<int, int> pickProbablityDict = new Dictionary<int, int>();
    Dictionary<int, int> pickChanceForDicesDict = new Dictionary<int, int>();

    [SerializeField] List<string> debugShowDict = new List<string>();
    private void ManualPopulateDicesInDictAndTheirProbablityToPick()
    {
        if(isMockedDataInitiated) return;
        isMockedDataInitiated = true;

        dicesDict.Add(key: 1, value: "Shield");
        pickProbablityDict.Add(key: dicesDict.ElementAt(0).Key, value: 60);

        dicesDict.Add(key: 2, value: "Axe");
        pickProbablityDict.Add(key: dicesDict.ElementAt(1).Key, value: 50);

        dicesDict.Add(key: 3, value: "Hand");
        pickProbablityDict.Add(key: dicesDict.ElementAt(2).Key, value: 45);

        dicesDict.Add(key: 4, value: "Blessed Hand");
        pickProbablityDict.Add(key: dicesDict.ElementAt(3).Key, value: 50);

        dicesDict.Add(key: 5, value: "Blessed Axe");
        pickProbablityDict.Add(key: dicesDict.ElementAt(4).Key, value: 90);

        dicesDict.Add(key: 6, value: "Bow");
        pickProbablityDict.Add(key: dicesDict.ElementAt(5).Key, value: 10);

        foreach (KeyValuePair<int, string> AI_Dice in dicesDict)
        {
            int diceID = AI_Dice.Key-1;
            debugShowDict.Add($"Dice:{diceID}\tRolled value:[{pickChanceForDicesDict.Where(p => p.Key == diceID).FirstOrDefault().Value.ToString("00")}]  Pick value: {pickProbablityDict.Values.ElementAt(diceID).ToString("00")}\tDice name: {pickProbablityDict.Keys.ElementAt(diceID)}");
        }
        
    }
    private void AutomaticPopulateDicesInDictList(List<DiceRollScript> dices)
    {
        if(isMockedDataInitiated) return;
        isMockedDataInitiated = true;

        for (int i = 1; i <= 6; i++)
        {
            dicesDict.Add(key: i, value: dices.Where(d=>d.DiceNumber == i).FirstOrDefault().name.Remove(0,2));
            //print(i + " / "+ dices.Where(d=>d.DiceNumber == i).FirstOrDefault().name.Remove(0,2));
        }
    }
    bool calculationsCompleted = false;
    private void CalculatePickingValuesForOwnedDices()
    {
        if(!isMockedDataInitiated) return;
        if(calculationsCompleted) return;
        calculationsCompleted = true;
        for (int i = 0; i < 6; i++)
        {
            int pickValue = 50; // TODO: Tutaj bedzie dziać sie magia xDD
            pickProbablityDict.Add(key: dicesDict.ElementAt(i).Key, value: pickValue);
            //print(dicesDict.ElementAt(i).Key.ToString() + " / "+ pickValue );
        }

        foreach (KeyValuePair<int, string> AI_Dice in dicesDict)
        {
            int diceID = AI_Dice.Key-1;
            debugShowDict.Add($"Dice:{diceID}\tRolled value:[{pickChanceForDicesDict.Where(p => p.Key == diceID).FirstOrDefault().Value.ToString("00")}]  Pick value: {pickProbablityDict.Values.ElementAt(diceID).ToString("00")}\tDice name: {dicesDict.Where(d=>d.Key == AI_Dice.Key).First().Value}");
        }

        foreach (string log in debugShowDict)
        {
            AndroidLogger.Log(log);
        }
        {
            
        }
    }
    void FixedUpdate()
    {
        if (IsTurnON && !AI_Player.TurnBlocker.activeSelf)
        {
            // print("AI is running...");
            RollDices();

            if (RollingIsCompleted)
            {
                GeneratePickChanceValuesForDices();
                //ManualPopulateDicesInDictAndTheirProbablityToPick();
                AutomaticPopulateDicesInDictList(AI_Player.DiceManager.Dices);
                CalculatePickingValuesForOwnedDices();
                // for test pick all dices
                for (int i = 0; i < 6; i++)
                {
                    PickDice(i);
                }

                
            
                EndTurn();
            }
        }
    }


    [SerializeField] bool itsNeedToReCalculatePickChanceValues = true;
    private void GeneratePickChanceValuesForDices()
    {
        if (!itsNeedToReCalculatePickChanceValues) return;

        for (int i = 1; i <= 6; i++)
        {
            pickChanceForDicesDict.Add(key: i, value: RandomNumberGenerator.NumberBetween(1, 100));
        }

        itsNeedToReCalculatePickChanceValues = false;
    }

    [ContextMenu("Włącz SI")] public void TurnONOFF() => IsTurnON = !IsTurnON;

    private bool RollingIsCompleted => AI_Player.DiceManager.Dices.ElementAt(0).rollingIsCompleted;
    private void RollDices()
    {
        if (!IsRollAllowed) return;

        print("Początkowe losowanie kości");
        IsRollAllowed = false;
        AI_Player.DiceManager.OnClick_ROLLDICES();
        AI_Player.GameManager.SwapRollButonWithEndTurn_OnClick(AI_Player.Name);
    }

    private void PickDice(int diceIndex)
    {
        Button diceButton = AI_Player.DiceManager.transform
        .GetChild(diceIndex)
        .GetComponent<Button>();

        if (diceButton.IsInteractable()) diceButton.onClick.Invoke();
    }

    private void EndTurn()
    {
        GameObject.Find("EndTurnButton").GetComponent<Button>().onClick.Invoke();
    }
    /*
    1. Piknij wszystkie kostki ( pokolei? )

    */
}
