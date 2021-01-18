using DiceRoller_Console;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodsManager : MonoBehaviour
{
    [SerializeField] Text CurrentGoldText;
    [SerializeField] string TokensOwnerName;

    [SerializeField] List<God> ListOfAvailableGods;
    public int CurrentGold { get => Convert.ToInt32(CurrentGoldText.text); }

    void Start()
    {
        print($"{TokensOwnerName} current gold = {CurrentGold}");
        PopulateContainerWithGodTokens(ListOfAvailableGods);
    }

    void PopulateContainerWithGodTokens(List<God> gods)
    {
        List<int> randomGodsTokenIndexes = GenerateThreeDifferentRandomNumbers(gods.Count);
        foreach(var GodScript in this.GetComponentsInChildren<GodScript>())
        {
            // zniwelowanie powtórzeń poprzez przechowywanie indeksów w pamięci (szybsze niż sprawdzanie za kazdym razem każdy token)

            GodScript.GodObject = ListOfAvailableGods[randomGodsTokenIndexes[0]];
            randomGodsTokenIndexes.RemoveAt(0);
        }
    }

    private List<int> GenerateThreeDifferentRandomNumbers(int maxValue)
    {
        List<int> randomNumbers = new List<int>();
        
        do
        {
            int number = RandomNumberGenerator.NumberBetween(0, maxValue-1);
            if(!randomNumbers.Contains(number))
            {
                randomNumbers.Add(number);
                print($"[{TokensOwnerName}] dodano: {number}, lista zawiera {randomNumbers.Count} liczb"); 
            }    
        } while (randomNumbers.Count < 3);

        return randomNumbers;
    }


    /* 
      DONE -T-O-D-O- -0-:- -z-a-i-n-i-c-j-o-w-a-n-i-e- -l-o-s-o-w-y-c-h- -t-o-k-e-n-ó-w- -b-ó-s-t-w- -d-l-a- -g-r-a-c-z-y- 

      TODO 1: sprawdzanie czy jakaś karta jest już aktywna , czy jest na nią focus (powiększona)?
          - jeżeli nie i wsztskie są w "Standardowym" rozmiarze 
          - pierwsze kliknięcie karty powiększa ją i w tym samym czasie zmniejsza pozostałe dwie

      TODO 2: powrót kart do trybu bez żadnego focusu po kliknięciu na tło swojego panelu god tokens

      TODO 3: przełączanie focusu pomiędzy swoimi kartami
          - odwrócić poprzednią karte jednocześnie ją zmniejszając ?
          - powiększyć nową wybraną i ją odwrócić w tym samym czasie ? 
              czy po tym jak poprzednia wróci do normalności -> do sprawdzenia któe ładniej wygląda

      TODO 4: opcja "wymiany" tokena = przerolowanie go (jednorazowa opcja?)
      */
}
