using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CombatManager : MonoBehaviour
{
    [SerializeField] int IndexOfCombatAction = -1;

    [SerializeField] public GameObject Player1BattlefieldDiceContainer;
    [SerializeField] public GameObject Player1ArenaDiceContainer;
    [SerializeField] private List<GameObject> Player1DicesInArena;

    [SerializeField] public GameObject Player2BattlefieldDiceContainer;
    [SerializeField] public GameObject Player2ArenaDiceContainer;
    [SerializeField] private List<GameObject> Player2DicesInArena;
    void Start()
    {
        if (Player1ArenaDiceContainer == null)
        {
            print("pole GameObject dla Player1ArenaDiceContainer jest puste, " +
                "zaninicjowanie ze standardową opcją znalezienia po nazwie: " +
                "'../Battlefield/FightZone/Player1Dices_Fight_DiceHolder'.");
            Player1ArenaDiceContainer = transform.GetChild(0).transform.gameObject; // Player1Dices_Fight_DiceHolder
        }

        if (Player2ArenaDiceContainer == null)
        {
            print("pole GameObject dla Player2ArenaDiceContainer jest puste, " +
                "zaninicjowanie ze standardową opcją znalezienia po nazwie: " +
                "'../Battlefield/FightZone/Player2Dices_Fight_DiceHolder'.");
            Player2ArenaDiceContainer = transform.GetChild(1).transform.gameObject; // Player2Dices_Fight_DiceHolder
        }

        print("Combat Manager = HeadQuater Everything whats happen in Arena.");
    }

    int player1DicesCounter;
    int player2DicesCounter;
    [SerializeField] public bool readyToFight;
    
    
    [SerializeField] public GameManager GMScript;
    [SerializeField] public Text player1GOLD_TMP;
    [SerializeField] public Text player2GOLD_TMP;

        void Update()
    {
         
        var P1_Container_ObjectCount = Player1ArenaDiceContainer.transform.childCount;
        var P2_Container_ObjectCount = Player2ArenaDiceContainer.transform.childCount;

        if (player1DicesCounter != P1_Container_ObjectCount || player2DicesCounter != P2_Container_ObjectCount)
        {
            Player1DicesInArena = GetDicesFromContainer(Player1ArenaDiceContainer); // Player1Dices_Fight_DiceHolder  
            player1DicesCounter = P1_Container_ObjectCount;

            Player2DicesInArena = GetDicesFromContainer(Player2ArenaDiceContainer); // Player2Dices_Fight_DiceHolder  
            player2DicesCounter = P2_Container_ObjectCount;

            //            print("na arenie gracza 1 znajduje sie ["+player1DicesCounter+"] kostek.");
            //           print("na arenie gracza 2 znajduje sie ["+player2DicesCounter+"] kostek.");
        }

        if (IndexOfCombatAction == 1 && readyToFight)
        {
            print("atak m 1 => def m 2");
            readyToFight = false;

            // atak player 1
            List<GameObject> attack = GetDiceOfType("MeleeAttack", GetDicesFromContainer(Player1BattlefieldDiceContainer));
            WrzucKostkiNaArene(attack);

            // def player 2
            List<GameObject> deffence = GetDiceOfType("MeleeDeffence", GetDicesFromContainer(Player2BattlefieldDiceContainer));
            WrzucKostkiNaArene(deffence);

            StartCoroutine(Combat(attack, deffence));
        }
        if (IndexOfCombatAction == 2 && readyToFight)
        {
            print("atak m 2 => def m 1");
            readyToFight = false;

            // atak player 2
            List<GameObject> attack = GetDiceOfType("MeleeAttack", GetDicesFromContainer(Player2BattlefieldDiceContainer));
            WrzucKostkiNaArene(attack);

            // def player 1
            List<GameObject> deffence = GetDiceOfType("MeleeDeffence", GetDicesFromContainer(Player1BattlefieldDiceContainer));
            WrzucKostkiNaArene(deffence);

            StartCoroutine(Combat(attack, deffence));
        }
        if (IndexOfCombatAction == 3 && readyToFight)
        {
            print("atak r 1 => def r 2");

            readyToFight = false;

            // atak ranged player 1
            List<GameObject> attack = GetDiceOfType("RangedAttack", GetDicesFromContainer(Player1BattlefieldDiceContainer));
            WrzucKostkiNaArene(attack);

            // def ranged player 2
            List<GameObject> deffence = GetDiceOfType("RangedDeffence", GetDicesFromContainer(Player2BattlefieldDiceContainer));
            WrzucKostkiNaArene(deffence);

            StartCoroutine(Combat(attack, deffence));
        }
        if (IndexOfCombatAction == 4 && readyToFight)
        {
            print("atak r 2 => def r 1");

            readyToFight = false;

            // atak ranged player 2
            List<GameObject> attack = GetDiceOfType("RangedAttack", GetDicesFromContainer(Player2BattlefieldDiceContainer));
            WrzucKostkiNaArene(attack);

            // def ranged player 1
            List<GameObject> deffence = GetDiceOfType("RangedDeffence", GetDicesFromContainer(Player1BattlefieldDiceContainer));
            WrzucKostkiNaArene(deffence);

            StartCoroutine(Combat(attack, deffence));
        }
        if(IndexOfCombatAction == 5)
        {
            // na wszelki wypadek wyzerowanie pozostałlości po początku fazy przyznawania golda z blessed itemkow
             var GM = GameObject.Find("GameManager").GetComponent<GameManager>();
             GM.cumulativeGoldStealingCounterP1 = 0;
             GM.cumulativeGoldStealingCounterP2 = 0;
            
            IndexOfCombatAction++;
        }
        if ((IndexOfCombatAction == 6 || IndexOfCombatAction == 7) && readyToFight)
        {
            print("steal 1/2 <=> steal 2/1");
            var playerComtainer = IndexOfCombatAction == 6 ? Player1BattlefieldDiceContainer : Player2BattlefieldDiceContainer;
            readyToFight = false;

            var goldStealDices = GetDiceOfType("Steal", GetDicesFromContainer(playerComtainer));
            GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
             GM.cumulativeGoldStealingCounterP1 = 0;
             GM.cumulativeGoldStealingCounterP2 = 0;
            WrzucKostkiNaArene(goldStealDices);

            StartCoroutine(Steal(goldStealDices, playerComtainer.name));

        }
    }

    private void WrzucKostkiNaArene(List<GameObject> dices)
    {
        foreach (var dice in dices)
        {
            dice.GetComponent<DiceActionScript>().InArena = true;
        }
    }

    private void ZdejmijKostkiIZmienKolorNaSzary(List<GameObject> dices)
    {
        foreach (var dice in dices)
        {
            dice.GetComponent<DiceActionScript>().InBattlefield = true;
        }
    }
    List<GameObject> GetDiceOfType(string diceType, List<GameObject> playerDices)
    {
        /*
        *    1,2     Axe     Mele Attack
        *     6      Helmet  Mele Deffence 
        *     4      Bow     Ranged Attack
        *     5      Sheield Ranged Deffence
        *     3      Hand    Steal
        */

        List<GameObject> dices = new List<GameObject>();

        switch (diceType)
        {
            case "MeleeAttack":
                dices.AddRange(playerDices.Where(d => d.name.Contains("1") || d.name.Contains("2")).ToList());
                break;

            case "MeleeDeffence":
                dices.AddRange(playerDices.Where(d => d.name.Contains("6")));
                break;

            case "RangedAttack":
                dices.AddRange(playerDices.Where(d => d.name.Contains("4")));
                break;

            case "RangedDeffence":
                dices.AddRange(playerDices.Where(d => d.name.Contains("5")));
                break;

            case "Steal":
                dices.AddRange(playerDices.Where(d => d.name.Contains("3")));
                break;
        }

        return dices;
    }
    IEnumerator Combat(List<GameObject> attackDices, List<GameObject> deffenceDices)
    {
        var maxLength = attackDices.Count >= deffenceDices.Count ? attackDices.Count : deffenceDices.Count;
        for (int i = 0; i < maxLength; i++)
        {
            if (i > deffenceDices.Count - 1)
            {
                if (attackDices.Count > i)
                {
                    attackDices[i].GetComponent<DiceActionScript>().MarkDiceAsAttacking = true;
                }
            }
            else
            {
                if (attackDices.Count > 0 && attackDices.Count >= deffenceDices.Count())
                {
                    attackDices[i].GetComponent<DiceActionScript>().MarkDiceAsUsed = true;
                }
            }

            if (i <= deffenceDices.Count - 1)
            {
                if (deffenceDices.Count > 0)
                {
                    deffenceDices[i].GetComponent<DiceActionScript>().MarkDiceAsUsed = true;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1f);

        // wyzorowanie info o otryzmanych obrazeniach
        GameObject.Find("GameManager").GetComponent<GameManager>().TemporaryIntakeDamage_Player1 = 0;
        GameObject.Find("GameManager").GetComponent<GameManager>().TemporaryIntakeDamage_Player2 = 0;

        ZdejmijKostkiIZmienKolorNaSzary(attackDices);
        ZdejmijKostkiIZmienKolorNaSzary(deffenceDices);

        yield return new WaitForSeconds(0.5f);
        IndexOfCombatAction++;
        readyToFight = true;
    }
    IEnumerator Steal(List<GameObject> goldStealingDices, string playerWhoStealingName)
    {
        print(playerWhoStealingName);
        yield return new WaitForSeconds(1f);
        int player1Gold = GameObject.Find("GameManager").GetComponent<GameManager>().CurrentGold1;
        int player2Gold = GameObject.Find("GameManager").GetComponent<GameManager>().CurrentGold2;

        GameObject.Find("ANDROIDLOGGER").GetComponent<Text>().text +=$" V1 = {player1Gold} / "+GameObject.Find("GameManager").GetComponent<GameManager>().CurrentGold2;
        GameObject.Find("ANDROIDLOGGER").GetComponent<Text>().text +=$" V2 = {player1Gold} / "+player2GOLD_TMP.text+"\n";
 
        int maxOponentAvailableGoldToSteal = 0;//  = playerWhoStealingName == "Player1Dices" ? player2Gold : player1Gold;
            switch (playerWhoStealingName)
            {
                case "Player1Dices":
                //maxOponentAvailableGoldToSteal = player2Gold; 
                maxOponentAvailableGoldToSteal = Convert.ToInt32(player2GOLD_TMP.text);
                break;

                case "Player2Dices":
                //maxOponentAvailableGoldToSteal = player1Gold;
                maxOponentAvailableGoldToSteal = Convert.ToInt32(player1GOLD_TMP.text);
                break;
            }
         GameObject.Find("ANDROIDLOGGER").GetComponent<Text>().text +=$"oponent gold:"+maxOponentAvailableGoldToSteal+"\n";
        for(int i = 1; i <= goldStealingDices.Count;i++)
            {
                if(maxOponentAvailableGoldToSteal>0)
                {
                    GameObject.Find("ANDROIDLOGGER").GetComponent<Text>().text += "Available gold to steal = "+maxOponentAvailableGoldToSteal+"\n";
                    goldStealingDices[i-1].GetComponent<DiceActionScript>().StealGoldUsingHandItem = true;
                    yield return new WaitForSeconds(1f);
                    maxOponentAvailableGoldToSteal--;
                }
                else
                {
                    print("nie mozesz krać od bankruta xD");
                    GameObject.Find("ANDROIDLOGGER").GetComponent<Text>().text += "Nie możesz kraść od bankruta. "+maxOponentAvailableGoldToSteal+"\n";
                    continue;
                }
            }

        yield return new WaitForSeconds(1f);

        ZdejmijKostkiIZmienKolorNaSzary(goldStealingDices);

        GameObject.Find("GameManager").GetComponent<GameManager>().cumulativeGoldStealingCounterP1 = 0;
        GameObject.Find("GameManager").GetComponent<GameManager>().cumulativeGoldStealingCounterP2 = 0;

        IndexOfCombatAction++;
        readyToFight = true;
    }

    List<GameObject> GetDicesFromContainer(GameObject diceContainer)
    {
        List<GameObject> listOfDiceObjects = new List<GameObject>();
        foreach (var diceObject in diceContainer.GetComponentsInChildren<DiceRollScript>())
        {
            listOfDiceObjects.Add(diceObject.transform.gameObject);
        }
        return listOfDiceObjects;
    }



    public void ANDROID_BUTTON_START_COMBAT_ROUTINE()
    {
        IndexOfCombatAction = 1;
        readyToFight = true;
    }
}
