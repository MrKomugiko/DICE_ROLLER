using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DiceSorterScript : MonoBehaviour
{
    [SerializeField] public bool PosortujKosci = false;
    [SerializeField] private List<Sprite> listOfEveryDiceFaces;
    [SerializeField] List<GameObject> listaKosci;

    void Update(){
        if(PosortujKosci == true){

            // utworzenie listy obiektów kości
            listaKosci= new List<GameObject>();
            // policzenie ilosci kosci w kontenerze
            int diceCounter_p1 = this.GetComponentsInChildren<DiceRollScript>().Count();

            // pobranie kolejnych kosci z kontenera do listy
            for (int i = 0; i < diceCounter_p1; i++)
            {
                listaKosci.Add(this.transform.GetChild(i).transform.gameObject);
            }

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
        foreach (KeyValuePair<int, int> posortowanapozycja in listaNowychIndexowKosci)
        {
            print($"[{posortowanapozycja.Key}][{posortowanapozycja.Value}]");
        }

        // wyciągnięcie kontenera z obiektów
        GameObject diceContainer = GameObject.Find(listaKosci[0].transform.parent.name);

        foreach (KeyValuePair<int, int> poprawnapozycja in listaNowychIndexowKosci)
        {
            foreach (GameObject kosc in listaKosci)
            {
                if (kosc.GetComponent<DiceRollScript>().DiceNumber == poprawnapozycja.Key)
                {
                    kosc.transform.SetSiblingIndex(poprawnapozycja.Value + 1);
                    print(kosc.transform.name + "diceNumber = " + poprawnapozycja.Key + " Została przeniesiona na pozycje " + (poprawnapozycja.Value + 1));
                }
            }
        }
    }

    Dictionary<int, int> SortDicesByType_V2(List<GameObject> dicesOnBattleground)
    {
        Dictionary<int, int> SortedDict = new Dictionary<int, int>();

        /*
            1,2     Axe     Mele Attack
            6       Helmet  Mele Deffence 
            4       Bow     Ranged Attack
            5       Sheield Ranged Deffence
            3       Hand    Steal
        */
        List<int> correctOrderDiceFaces = new List<int>(){1,2,6,4,5,3};
        
        int diceIndex = 0;
        
        foreach (int diceOrderNumber in correctOrderDiceFaces)
        {
            foreach (var diceObject in dicesOnBattleground)
            {
                if (diceObject.GetComponent<DiceRollScript>().DiceImage.sprite.name.ElementAt(0).ToString() == diceOrderNumber.ToString())
                {
                    SortedDict.Add(diceObject.GetComponent<DiceRollScript>().DiceNumber, diceIndex);
                    diceIndex++;
                    continue;
                }
            }
        }

        return SortedDict;
    }
}
