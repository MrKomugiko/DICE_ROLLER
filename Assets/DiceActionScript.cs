using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceActionScript : MonoBehaviour
{
    [SerializeField] bool _addGoldFromBlessedItems;
    [SerializeField] bool _stealGoldUsingHandItem;
    [SerializeField] bool _markDiceAsUsed;
    [SerializeField] bool _markDiceAsActive;
    [SerializeField] bool _inArena;
    [SerializeField] bool _inBattlefield;
    [SerializeField] bool _markDiceAsAttacking;

    public bool AddGoldFromBlessedItems
    {
        get => _addGoldFromBlessedItems;
        set
        {
            if (value == true)
            {
                // najpierw sprawdzenie czy item jest faktycznie "blogoslawiony"
                if (this.name.Contains("Blessed"))
                {
                    // jeżeli tak -> uruchom "Animacje" i wszystko co sie z nią wiąże
                    StartCoroutine(AddGodCoin());
                }
            }
            // zresetuj wartośc na false
            _addGoldFromBlessedItems = false;
        }
    }
    public bool StealGoldUsingHandItem
    {
        get => _stealGoldUsingHandItem;
        set
        {
            if (value == true)
            {
                // najpierw sprawdzenie czy item może kraść -> jest to "Hand"
                if (this.name.Contains("Hand"))
                {
                    // jeżeli tak -> uruchom "Animacje" i wszystko co sie z nią wiąże
                    StartCoroutine(StealGodCoinFromOponent());
                }
            }
            // zresetuj wartośc na false
            _stealGoldUsingHandItem = false;
        }
    }
    public bool MarkDiceAsUsed
    {
        get => _markDiceAsUsed;
        set
        {
            if (value == true)
            {
                StartCoroutine(ChangeColor(Color.gray));
            }
            _markDiceAsUsed = false;
        }
    }
    public bool MarkDiceAsActive
    {
        get => _markDiceAsActive;
        set
        {
            if (value == true)
            {
                StartCoroutine(ChangeColor(Color.gray));
            }
            _markDiceAsActive = false;
        }
    }
    public bool InArena
    {
        get => _inArena;
        set
        {
            if (value == true)
            {
                MoveToArena(GetComponent<DiceRollScript>().DiceOwner);
            }
        }
    }
    public bool InBattlefield
    {
        get => _inBattlefield;
        set
        {
            if (value == true)
            {
                MoveToBattlefield(GetComponent<DiceRollScript>().DiceOwner);
            }
        }
    }
    public bool MarkDiceAsAttacking
    {
        get => _markDiceAsAttacking;
        set
        {
            if (value == true)
            {
                StartCoroutine(ChangeColor(Color.red));

            }
            _markDiceAsAttacking = false;
        }
    }
    
    void Update()
    {
        #region debbuging inspector function checker
        // sprawdzanie funkcji z posiomu inspektora
        if (_addGoldFromBlessedItems && this.name.Contains("Blessed"))
        {
            _addGoldFromBlessedItems = false;
            StartCoroutine(AddGodCoin());
        }

        if (_stealGoldUsingHandItem && this.name.Contains("Hand"))
        {
            _stealGoldUsingHandItem = false;
            StartCoroutine(StealGodCoinFromOponent());
        }

        if (_markDiceAsUsed == true)
        {
            _markDiceAsUsed = false;
            StartCoroutine(ChangeColor(Color.gray));
        }

        if (_markDiceAsActive == true)
        {
            _markDiceAsActive = false;
            StartCoroutine(ChangeColor(Color.white));
        }

        if (_markDiceAsActive == true)
        {
            _markDiceAsActive = false;
            StartCoroutine(ChangeColor(Color.white));
        }

        if (_inArena == true)
        {
            MoveToArena(GetComponent<DiceRollScript>().DiceOwner);
            _inArena = false;
        }

        if (_inBattlefield == true)
        {
            MoveToBattlefield(GetComponent<DiceRollScript>().DiceOwner);
            _inBattlefield = false;
        }

        if (_markDiceAsAttacking == true)
        {
            StartCoroutine(ChangeColor(Color.red));
            _markDiceAsAttacking = false;
        }
        #endregion
    }
    void TakeDamage()
    {
        string parentName = this.transform.parent.name.ToString();

        GameManager HealthVault = GameObject.Find("GameManager").transform.GetComponent<GameManager>();

        switch (parentName)
        {
            case "Player1Dices_Fight_DiceHolder":
                HealthVault.TemporaryIntakeDamage_Player2 += 1;
                break;

            case "Player2Dices_Fight_DiceHolder":
                HealthVault.TemporaryIntakeDamage_Player1 += 1;
                break;
        }
    } 
    void MoveToArena(string playerName)
    {
        CombatManager CM = GameObject.Find("FightZone").GetComponent<CombatManager>();
        GameObject container = null;
        if (playerName == "Player1")
        {
            container = CM.Player1ArenaDiceContainer;
        }
        if (playerName == "Player2")
        {
            container = CM.Player2ArenaDiceContainer;
        }
        this.gameObject.transform.SetParent(container.transform);
        _inArena = true;
    }
    void MoveToBattlefield(string playerName)
    {
        GameObject container = null;
        if (playerName == "Player1")
        {
            container = GameObject.Find("Player1Dices").gameObject;
        }
        if (playerName == "Player2")
        {
            container = GameObject.Find("Player2Dices").gameObject;
        }
        this.gameObject.transform.SetParent(container.transform);
        _inBattlefield = true;

        StartCoroutine(ChangeColor(Color.gray));
    }

    IEnumerator AddGodCoin()
    {
        _addGoldFromBlessedItems = false;
        string parentName = this.transform.parent.name.ToString();

        GameManager goldVaults = GameObject.Find("GameManager").transform.GetComponent<GameManager>();

        var p1coin = GameObject.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>();
        var p2coin = GameObject.Find("CoinTextPlayer2").GetComponent<TextMeshProUGUI>();

        // Dodawanie golda do puli i przełączanie sie kostek na kolor żółty 
        for (float i = 0f; i <= 2; i += 0.05f)
        {
            if (Math.Round(Convert.ToDecimal(i), 3) == 1)
            {
                switch (parentName)
                {
                    case "Player1Dices":
                        goldVaults.TemporaryGoldVault_player1 += 1;
                        p1coin.color = Color.yellow;
                        break;

                    case "Player2Dices":
                        goldVaults.TemporaryGoldVault_player2 += 1;
                        p2coin.color = Color.yellow;
                        break;
                }
            }

            this.GetComponent<Image>().color = Color.Lerp(Color.white, Color.yellow, i);
            yield return new WaitForSeconds(0.05f);
        }

        for (float i = 0f; i <= 1; i += 0.05f)
        {
            if (p1coin.text != "+0")
            {
                p1coin.color = Color.Lerp(Color.yellow, Color.clear, (i));
            }
            if (p2coin.text != "+0")
            {
                p2coin.color = Color.Lerp(Color.yellow, Color.clear, (i));
            }
            this.GetComponent<Image>().color = Color.Lerp(Color.yellow, Color.white, i);
            yield return new WaitForSeconds(0.05f);
        }
    }
    IEnumerator StealGodCoinFromOponent()
    {
        _stealGoldUsingHandItem = false;
        // szukanie do kogo należy obiekt kości
        string parentName = this.transform.parent.name.ToString(); 
        var p1coin = GameObject.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>();
        var p2coin = GameObject.Find("CoinTextPlayer2").GetComponent<TextMeshProUGUI>();

        GameManager goldVaults = GameObject.Find("GameManager").transform.GetComponent<GameManager>();
            yield return new WaitForSeconds(0.5f);

        // Dodawanie golda do puli i przełączanie sie kostek na kolor żółty 
        for (float i = 0f; i <= 2; i += 0.1f)
        {
            if (Math.Round(Convert.ToDecimal(i), 3) == Convert.ToDecimal(.7f))
            {
                switch (parentName)
                {
                    case "Player1Dices_Fight_DiceHolder":
                    // właśicicielem jest gracz 1, ondostanie golda, przeciwnikiem player2 jemu zaboerzemy
                        // właściciel
                        goldVaults.TemporaryGoldVault_player1 += 1;
                        p1coin.color = Color.yellow;
                        // przeciwnik
                        goldVaults.TemporaryGoldVault_player2 -= 1;
                        p2coin.color = Color.red;
                        break;

                    case "Player2Dices_Fight_DiceHolder":
                    // właśicicielem jest gracz 1, ondostanie golda, przeciwnikiem player2 jemu zaboerzemy
                        // właściciel
                        goldVaults.TemporaryGoldVault_player2 += 1;
                        p2coin.color = Color.yellow;
                        // przeciwnik
                        goldVaults.TemporaryGoldVault_player1 -= 1;
                        p1coin.color = Color.red;
                        break;
                }   
            }
            this.GetComponent<Image>().color = Color.Lerp(Color.white, Color.green, i*2);
            yield return new WaitForSeconds(0.05f);
       }
    }
    IEnumerator ChangeColor(Color color)
    {
        bool done = false;
        for (float i = 0f; i <= 1; i += 0.05f)
        {
            this.GetComponent<Image>().color = Color.Lerp(this.GetComponent<Image>().color, color, i);
            #region Color.RED => ATAK -> wysłanie obrażeń do gracza ( przeciwnika )
            if (!done)
            {
                // jakos tak w pierwszej polowie zmiany koloru zeby odjelo hp pzeciwnika
                if (color == Color.red)
                {
                    TakeDamage();
                    done = true;
                }
            }
            #endregion
            yield return new WaitForSeconds(0.05f);
        }
    }
    }