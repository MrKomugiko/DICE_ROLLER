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

    [SerializeField] private bool _makeDiceAsUsed;
    public bool MarkDiceAsUsed
    {
        get => _makeDiceAsUsed;
        set
        {
            if (value == true)
            {
                StartCoroutine(ChangeColor(Color.gray));
            }
            _makeDiceAsUsed = false;
        }
    }
    [SerializeField] private bool _makeDiceAsActive;
    public bool MakeDiceAsActive
    {
        get => _makeDiceAsActive;
        set
        {
            if (value == true)
            {
                StartCoroutine(ChangeColor(Color.gray));
            }
            _makeDiceAsActive = false;
        }
    }
    void Start()
    {

    }

    void Update()
    {
        // sprawdzanie funkcji z posiomu inspektora

        /*
         * Cała procedura dodawania golda:
         *      - zmiana koloru na żółty i powrót
         *      - wyswietlenie info
         *      - zsumowanie golda w interfejsie graczy
         */

        if (_addGoldFromBlessedItems && this.name.Contains("Blessed"))
        {
            _addGoldFromBlessedItems = false;
            StartCoroutine(AddGodCoin());
        }

        /*
         * Oznaczenie kości jako aktywnej/nieaktywnej
         *      - zmiana koloru na szary lub biały
         */

        if (_makeDiceAsUsed == true)
        {
            _makeDiceAsUsed = false;
            StartCoroutine(ChangeColor(Color.gray));
        }
        if (_makeDiceAsActive == true)
        {
            _makeDiceAsActive = false;
            StartCoroutine(ChangeColor(Color.white));
        }

        /*
         * TODO: Wysłanie kostki na Arenę
         */
    }

    IEnumerator AddGodCoin()
    {
        var GOLDVault1 = GameObject.Find("GameManager").transform;
        GameManager goldVaults = GOLDVault1.GetComponent<GameManager>();

        _addGoldFromBlessedItems = false;

        var p1coin = GameObject.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>();
        var p2coin = GameObject.Find("CoinTextPlayer2").GetComponent<TextMeshProUGUI>();
        int counter = 1;
        for (float i = 0f; i <= 2; i += 0.05f)
        {
            this.GetComponent<Image>().color = Color.Lerp(Color.white, Color.yellow, i);

            if(counter == 1){
                if (this.transform.parent.name.ToString() == "Player1Dices")
                {
                    goldVaults.TemporaryGoldVault_player1+=1;
                    p1coin.color = Color.yellow;
                }

                if (this.transform.parent.name.ToString() == "Player2Dices")
                {
                    goldVaults.TemporaryGoldVault_player2+=1;
                    p2coin.color = Color.yellow;
                }
            counter --;
            }
            yield return new WaitForSeconds(0.05f);
        }

        int counter2 = 1;
        for (float i = 0f; i <= 1.1; i += 0.05f)
        {
            p1coin.color = Color.Lerp(Color.yellow, Color.clear, (i));
            p2coin.color = Color.Lerp(Color.yellow, Color.clear, (i));
            if (counter2 == 1){               
             if (this.transform.parent.name.ToString() == "Player1Dices")
                {
                    goldVaults.AddGoldToPlayerVault("Player1",1);
                }
                if (this.transform.parent.name.ToString() == "Player2Dices")
                {
                    goldVaults.AddGoldToPlayerVault("Player2",1);
                }
                counter2 --;
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