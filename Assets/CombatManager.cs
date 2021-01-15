using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] int IndexOfCombatAction;

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
        1,2     Axe     Mele Attack
        6       Helmet  Mele Deffence 
        4       Bow     Ranged Attack
        5       Sheield Ranged Deffence
        3       Hand    Steal
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

        //        print(dices.Count()+"szt kostek typu "+diceType+" znalezione i dodane do listy.");
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
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1f);

        ZdejmijKostkiIZmienKolorNaSzary(attackDices);
        ZdejmijKostkiIZmienKolorNaSzary(deffenceDices);

        yield return new WaitForSeconds(2f);
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
