using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    [SerializeField] public GameObject Player1ArenaDiceContainer;
    [SerializeField] public GameObject Player2ArenaDiceContainer;

    [SerializeField] GameObject Player1BattlefieldDiceContainer;
    [SerializeField] GameObject Player2BattlefieldDiceContainer;
    [SerializeField] public int IndexOfCombatAction = 0;
    [SerializeField] bool readyToFight;
    [SerializeField] GameManager GM_Script;

    void Start()
    {
        GameObject.Find("ANDROID_TEST_ENDCOMBATANDBACKTOROLL").GetComponent<Button>().interactable = false;

        GM_Script = GameObject.Find("GameManager").GetComponent<GameManager>();
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
    }
    void Update()
    {    
        // WHOLE COMBAT ROUTINE (6 steps of combat):
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
        if ((IndexOfCombatAction == 5 || IndexOfCombatAction == 6) && readyToFight)
        {
            GM_Script.CumulativeGoldStealingCounterP1 = 0;
            GM_Script.CumulativeGoldStealingCounterP2 = 0;

            print("steal 1/2 <=> steal 2/1");
            var playerComtainer = IndexOfCombatAction == 5 ? Player1BattlefieldDiceContainer : Player2BattlefieldDiceContainer;
            readyToFight = false;

            var goldStealDices = GetDiceOfType("Steal", GetDicesFromContainer(playerComtainer));

            WrzucKostkiNaArene(goldStealDices);

            StartCoroutine(Steal(goldStealDices, playerComtainer.name));
        }
        if (IndexOfCombatAction == 7 && readyToFight)
        {
           GameObject.Find("ANDROID_TEST_ENDCOMBATANDBACKTOROLL").GetComponent<Button>().interactable = true;
           IndexOfCombatAction++;
        }        
    }
    void WrzucKostkiNaArene(List<GameObject> dices)
    {
        foreach (var dice in dices)
        {
            dice.GetComponent<DiceActionScript>().InArena = true;
        }
    }
    void ZdejmijKostkiIZmienKolorNaSzary(List<GameObject> dices)
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
    List<GameObject> GetDicesFromContainer(GameObject diceContainer)
    {
        List<GameObject> listOfDiceObjects = new List<GameObject>();
        foreach (var diceObject in diceContainer.GetComponentsInChildren<DiceRollScript>())
        {
            listOfDiceObjects.Add(diceObject.transform.gameObject);
        }
        return listOfDiceObjects;
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
        GM_Script.TemporaryIntakeDamage_Player1 = 0;
        GM_Script.TemporaryIntakeDamage_Player2 = 0;

        ZdejmijKostkiIZmienKolorNaSzary(attackDices);
        ZdejmijKostkiIZmienKolorNaSzary(deffenceDices);

        yield return new WaitForSeconds(0.5f);

        IndexOfCombatAction++;
        readyToFight = true;
    }
    IEnumerator Steal(List<GameObject> goldStealingDices, string playerWhoStealingName)
    {
        int player1Gold = GM_Script.CurrentGold1;
        int player2Gold = GM_Script.CurrentGold2;

        int maxOponentAvailableGoldToSteal = playerWhoStealingName == "Player1Dices" ? player2Gold : player1Gold;

        for(int i = 0; i < goldStealingDices.Count;i++)
            {
                if(maxOponentAvailableGoldToSteal>0)
                {
                    goldStealingDices[i].GetComponent<DiceActionScript>().StealGoldUsingHandItem = true;
                    yield return new WaitForSeconds(.5f);
                    maxOponentAvailableGoldToSteal--;
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
                yield return new WaitForSeconds(0.5f);
            }

        yield return new WaitForSeconds(0.5f);

        GM_Script.CumulativeGoldStealingCounterP1 = 0;
        GM_Script.CumulativeGoldStealingCounterP2 = 0;

        ZdejmijKostkiIZmienKolorNaSzary(goldStealingDices);
        yield return new WaitForSeconds(0.5f);



        IndexOfCombatAction++;
        readyToFight = true;
    }
    
    public void ANDROID_BUTTON_START_COMBAT_ROUTINE()
    {
        IndexOfCombatAction = 1;
        readyToFight = true;
    }

    public void ANDROID_BUTTON_END_COMBAT_AND_BACK_TO_ROLL()
    {
        GM_Script.ChangeUIToRollingMode();
        IndexOfCombatAction = 0;
    }
}
