using System.Diagnostics;
using System.Linq;
using DiceRoller_Console;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodsManager : MonoBehaviour
{
    [SerializeField] Text _currentGoldText;
    [SerializeField] string _tokensOwnerName;

    [SerializeField] List<God> _listOfAvailableGodsTotems;
    [SerializeField] List<GodScript> _godCardsInContainer;
    [SerializeField] List<CardScript> _listOfAllCards;
    public int CurrentGold { get => Convert.ToInt32(_currentGoldText.text); }
    public List<CardScript> ListOfAllCards { 
        get => _listOfAllCards; 
        set 
        { 
            _listOfAllCards = value;
            print("liczba kart: "+value.Count); 
        }
    }
    void Awake()
    {
        _godCardsInContainer = GetComponentsInChildren<GodScript>().ToList();
    }

    void Start()
    {
        print($"{_tokensOwnerName} | current gold = {CurrentGold}");
        print($"{_tokensOwnerName} | Rozpoczęscie rozlokowywania toemów w kartach gracza");
        PopulateContainerWithGodTokens(_listOfAvailableGodsTotems);
        print($"{_tokensOwnerName} | Zakończenie ustawien początkowych.");
        ListOfAllCards = this.GetComponentsInChildren<CardScript>().ToList();
    }

    void PopulateContainerWithGodTokens(List<God> godTotems)
    {
        print($"{_tokensOwnerName} | aktualnie możliwych totemów:"+godTotems.Count);
        List<int> randomGodsTokenIndexes = GenerateThreeDifferentRandomNumbers(godTotems.Count);
        int index = 0;
        foreach(GodScript godCard in _godCardsInContainer)
        {
            godCard.SelfConfigure(godTotems[randomGodsTokenIndexes[index]]);
            index++;
        }
        
        print($"{_tokensOwnerName} | nazwy bogów dodanych do kart:[{_godCardsInContainer[0].GodObject.Name}] [{_godCardsInContainer[1].GodObject.Name}] [{_godCardsInContainer[2].GodObject.Name}] ");
    }

    private List<int> GenerateThreeDifferentRandomNumbers(int maxValue)
    {
        print($"{_tokensOwnerName} | Losowanie trzech totemów.");
        List<int> randomNumbers = new List<int>();
        
        do
        {
            int number = RandomNumberGenerator.NumberBetween(0, maxValue-1);
            if(!randomNumbers.Contains(number))
            {
                randomNumbers.Add(number);
            }    
        } while (randomNumbers.Count < 3);

        print($"{_tokensOwnerName} | indexy totemów: [{randomNumbers[0]}][{randomNumbers[1]}][{randomNumbers[2]}]");
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
