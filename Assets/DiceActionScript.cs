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
            _addGoldFromBlessedItems = value;
            if (value == true)
            {
                // najpierw sprawdzenie czy item jest faktycznie "blogoslawiony"
                if(this.name.Contains("Blessed")){
                    StartCoroutine(AddGodCoin());
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // if(this.name.Contains("blessed")){
        //     AddGoldFromBlessedItems = true;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        // debug
        // if (AddGoldFromBlessedItems == true)
        // {
        //     StartCoroutine(AddGodCoin());
        // }
    }

    IEnumerator AddGodCoin()
    {
        print("Test poczatek coorutine");
        _addGoldFromBlessedItems = false;

        var p1coin = GameObject.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>();
        var p2coin = GameObject.Find("CoinTextPlayer2").GetComponent<TextMeshProUGUI>();
        int collectedGoldP1 = 0, collectedGoldP2 = 0;
        for (float i = 0f; i <= 2; i += 0.05f)
        {
            //print(i.ToString());
            this.GetComponent<Image>().color = Color.Lerp(Color.white, Color.yellow, i);
            if (i >= 1f && i < 1.05f)
            {
                //                print(i);
                // print("minela 1 sekunda czas uaktywnic tekst info z podsumowaniem przy punktach");
                if (this.transform.parent.name.ToString() == "Player1Dices")
                {
                    // print(this.transform.parent.name.ToString());
                    p1coin.color = Color.yellow;
                    collectedGoldP1 = Convert.ToInt32(p1coin.text) + 1;
                    // print(currentCoins);
                    p1coin.SetText("+" + collectedGoldP1.ToString());
                }

                if (this.transform.parent.name.ToString() == "Player2Dices")
                {
                    // print(this.transform.parent.name.ToString());
                    p2coin.color = Color.yellow;
                    collectedGoldP2 = Convert.ToInt32(p2coin.text) + 1;
                    // print(currentCoins);
                    p2coin.SetText("+" + collectedGoldP2.ToString());
                }
            }
            yield return new WaitForSeconds(0.05f);
        }

        for (float i = 0f; i <= 1.1; i += 0.05f)
        {
             print(i.ToString());
            //print(i.ToString()+" => "+wynikiLosowania[i].ToString());
            p1coin.color = Color.Lerp(Color.yellow, Color.clear, (i));
            p2coin.color = Color.Lerp(Color.yellow, Color.clear, (i));
            if (i >= 0.5f && i <= 0.55f)
            {
                 print(i.ToString()+"???");
                var p1GoldMain = Convert.ToInt32(GameObject.Find("Player1").transform.Find("GoldPoints").GetComponent<TextMeshProUGUI>().text);
                int goldP1 = p1GoldMain + collectedGoldP1;
                GameObject.Find("Player1").transform.Find("GoldPoints").GetComponent<TextMeshProUGUI>().SetText(goldP1.ToString());

                var p2GoldMain = Convert.ToInt32(GameObject.Find("Player2").transform.Find("GoldPoints").GetComponent<TextMeshProUGUI>().text);
                int goldP2 = p2GoldMain + collectedGoldP2;
                GameObject.Find("Player2").transform.Find("GoldPoints").GetComponent<TextMeshProUGUI>().SetText(goldP2.ToString());

            }
            
            this.GetComponent<Image>().color = Color.Lerp(Color.yellow, Color.white, i);
            yield return new WaitForSeconds(0.05f);
            p1coin.SetText("0");
            p2coin.SetText("0");
            print("koniec");
        }
    }
}
