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

        if (_addGoldFromBlessedItems)
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
    }

    IEnumerator AddGodCoin()
    {
        _addGoldFromBlessedItems = false;

        var p1coin = GameObject.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>();
        var p2coin = GameObject.Find("CoinTextPlayer2").GetComponent<TextMeshProUGUI>();
        int collectedGoldP1 = 0, collectedGoldP2 = 0;
        for (float i = 0f; i <= 2; i += 0.05f)
        {
            this.GetComponent<Image>().color = Color.Lerp(Color.white, Color.yellow, i);
            if (i >= 1f && i < 1.05f)
            {
                if (this.transform.parent.name.ToString() == "Player1Dices")
                {
                    p1coin.color = Color.yellow;
                    collectedGoldP1 = Convert.ToInt32(p1coin.text) + 1;
                    p1coin.SetText("+" + collectedGoldP1.ToString());
                }

                if (this.transform.parent.name.ToString() == "Player2Dices")
                {
                    p2coin.color = Color.yellow;
                    collectedGoldP2 = Convert.ToInt32(p2coin.text) + 1;
                    p2coin.SetText("+" + collectedGoldP2.ToString());
                }
            }
            yield return new WaitForSeconds(0.05f);
        }

        for (float i = 0f; i <= 1.1; i += 0.05f)
        {
            p1coin.color = Color.Lerp(Color.yellow, Color.clear, (i));
            p2coin.color = Color.Lerp(Color.yellow, Color.clear, (i));
            if (i >= 0.5f && i < 0.55f)
            {
                var p1GoldMain = Convert.ToInt32(GameObject.Find("Player1").transform.Find("GoldPoints").GetComponent<TextMeshProUGUI>().text);
                if (collectedGoldP1 > 0)
                {
                    collectedGoldP1 = 1;
                }
                int goldP1 = p1GoldMain + collectedGoldP1;
                GameObject.Find("Player1").transform.Find("GoldPoints").GetComponent<TextMeshProUGUI>().SetText(goldP1.ToString());

                var p2GoldMain = Convert.ToInt32(GameObject.Find("Player2").transform.Find("GoldPoints").GetComponent<TextMeshProUGUI>().text);
                if (collectedGoldP2 > 0)
                {
                    collectedGoldP2 = 1;
                }
                int goldP2 = p2GoldMain + collectedGoldP2;
                GameObject.Find("Player2").transform.Find("GoldPoints").GetComponent<TextMeshProUGUI>().SetText(goldP2.ToString());
            }

            this.GetComponent<Image>().color = Color.Lerp(Color.yellow, Color.white, i);

            p1coin.SetText("0");
            p2coin.SetText("0");

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