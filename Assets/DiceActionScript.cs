using System.Runtime.InteropServices;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceActionScript : MonoBehaviour
{
    [SerializeField] private bool _addGoldFromBlessedItems;
    [SerializeField] private bool _markDiceAsUsed;
    [SerializeField] private bool _markDiceAsActive;
    
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
    void Update()
    {
        #region debbuging inspector function checker
        // sprawdzanie funkcji z posiomu inspektora
        if (_addGoldFromBlessedItems && this.name.Contains("Blessed"))
        {
            _addGoldFromBlessedItems = false;
            StartCoroutine(AddGodCoin());
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
        #endregion
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
            if (Math.Round(Convert.ToDecimal(i),3) == 1)
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
            if(p1coin.text != "+0"){
                p1coin.color = Color.Lerp(Color.yellow, Color.clear, (i));
            }
            if(p2coin.text != "+0"){
                p2coin.color = Color.Lerp(Color.yellow, Color.clear, (i));
            }
            this.GetComponent<Image>().color = Color.Lerp(Color.yellow, Color.white, i);
            yield return new WaitForSeconds(0.05f);
        }
    }
    IEnumerator ChangeColor(Color color)
    {
        for (float i = 0f; i <= 2; i += 0.05f)
        {
            this.GetComponent<Image>().color = Color.Lerp(this.GetComponent<Image>().color, color, i);
            yield return new WaitForSeconds(0.05f);
        }
    }
}