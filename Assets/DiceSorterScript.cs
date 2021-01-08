using System.Runtime.Serialization;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceSorterScript : MonoBehaviour
{
    [SerializeField] private bool PosortujKosci = false;
    [SerializeField] private List<Sprite> listOfEveryDiceFaces;

    void Update(){
        if(PosortujKosci == true){

// utworzenie listy obiektów kości
            List<GameObject> listaKosci = new List<GameObject>();
            
// policzenie ilosci kosci w kontenerze
            int diceCounter_p1 = this.GetComponentsInChildren<DiceRollScript>().Count();

// pobranie kolejnych kosci z kontenera do listy
            for (int i = 0; i < diceCounter_p1; i++)
            {
                listaKosci.Add(this.transform.GetChild(i).transform.gameObject);
            }

// debug: zformatowane wyswietlenie zawartosci utworzonej wczesniej listy
/*
            string ZawartoscListyString = "";
            foreach(var obiektKosci in listaKosci){
                ZawartoscListyString += $"[{listaKosci.IndexOf(obiektKosci)}]\tID[{obiektKosci.GetComponent<DiceRollScript>().DiceNumber}],\t{obiektKosci.name},\n";
            }
            print(
                $"Test procedury sortowania\n"+
                $"kości gracza 1:\n"+
                $"{ZawartoscListyString}\n"
                );
*/

// uruchomienie sortowania przekazujac do metody liste kosci  
// zapisanie dict przechowujacego < id kostki / index poprawnej pozycji >
            Dictionary<int,int> listaNowychIndexowKosci = SortDicesByType_V2(listaKosci);

// uruchomienie procesu rearanżacji kolejności w  kontenerze
            RearrangeDicesInContainer(listaNowychIndexowKosci,listaKosci);

// ukonczenie procesu, zablokowowanie  kolejnych iteracji 
            PosortujKosci = false;
        }
    }

    private void RearrangeDicesInContainer(Dictionary<int, int> listaNowychIndexowKosci, List<GameObject> listaKosci)
    {
        // IMPORTANT: dice number stats from 1 to 6, locations from 0 to 5 !
        // lecim od 0 do 5
        print("done");
    }

    Dictionary<int, int> SortDicesByType_V2(List<GameObject> dicesOnBattleground)
    {
        Dictionary<int, int> SortedDict = new Dictionary<int, int>();

        List<int> correctOrderDiceFaces = new List<int>(){1,2,6,4,5,3};
        /*
            1,2     Axe     Mele Attack
            6       Helmet  Mele Deffence 
            4       Bow     Ranged Attack
            5       Sheield Ranged Deffence
            3       Hand    Steal
        */
        
        int diceIndex = 0;
        
        foreach (int diceFace in correctOrderDiceFaces)
        {
            foreach (var diceObject in dicesOnBattleground)
            {
                // MELE ATTACK
                if (diceObject.GetComponent<DiceRollScript>().DiceImage.sprite.name.ElementAt(0).ToString() == diceFace.ToString())
                {
                    SortedDict.Add(diceObject.GetComponent<DiceRollScript>().DiceNumber, diceIndex);
                    print($"kostka numer [{diceObject.GetComponent<DiceRollScript>().DiceNumber}] z pozycji [{diceObject.transform.GetSiblingIndex().ToString()}] => powinna isc na miejsce [{diceIndex}]");
                    diceIndex++;
                }
            }
        }
        // < DICE_NUMBER, NEW_CORRECT_POSITION >
        return SortedDict;
    }
}
