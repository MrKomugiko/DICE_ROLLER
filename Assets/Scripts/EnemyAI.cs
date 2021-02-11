using System.Data;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DiceRoller_Console;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] Player AI_Player, ENEMY_Player;
    [SerializeField] AIPickChanceUi UI;

    [SerializeField] public bool FirstRoll, SecondRoll, ThirdRoll, FourthRoll, IsTurnON;
    [SerializeField] bool isMockedDataInitiated, calculatingNewPickValuesIsCompleted, IsRollAllowed = true, itsNeedToReCalculateRandomPickingValues = true;

    Dictionary<int, string> dicesDict = new Dictionary<int, string>();              // diceNumber <1;6> / "nazwa kostki"
    Dictionary<int, string> actualLeftDicesOnHand = new Dictionary<int, string>();  // COPY of diceDict
    Dictionary<int, int> pickProbablityDict = new Dictionary<int, int>();           // diceNumber <1;6> / prawdopodobieństwo bycia wybraną <1;100>
    Dictionary<int, int> randomPickValueForDices = new Dictionary<int, int>();       // diceNumber <1;6> / losowa wartość do wybrania <1;100>

    [SerializeField] List<string> debugShowDict = new List<string>();
    List<DiceRollScript> GetCurrentEnemyDicesInBattlefield => ENEMY_Player.ListOfDicesOnBattleground;
    List<DiceRollScript> GetCurrentDicesInBattlefield => AI_Player.ListOfDicesOnBattleground;
    bool RollingIsCompleted => AI_Player.DiceManager.Dices.ElementAt(3).rollingIsCompleted;
    IEnumerator calculatingCoroutine = null;

    void Start()
    {
        UI = this.GetComponentInChildren<AIPickChanceUi>();
        StartCoroutine(AI_Player.LoadSkillsData());
    }
    bool isSkillSelected = false;
    void FixedUpdate()
    {
        if (IsTurnON && !AI_Player.TurnBlocker.activeSelf)
        {
            if (FirstRoll)
            {
                RollDices();
                if (RollingIsCompleted && calculatingCoroutine == null)
                {
                    isMockedDataInitiated = false;
                    AutomaticPopulateDicesInDictList(AI_Player.DiceManager.Dices);
                    calculatingCoroutine = CalculatingChance(roundNumber: 1);
                    StartCoroutine(calculatingCoroutine);
                }
            }
            if (SecondRoll)
            {
                RollDices();
                if (RollingIsCompleted && calculatingCoroutine == null)
                {
                    calculatingCoroutine = CalculatingChance(roundNumber: 2);
                    StartCoroutine(calculatingCoroutine);
                }
            }
            if (ThirdRoll)
            {
                RollDices();
                if (RollingIsCompleted && calculatingCoroutine == null)
                {
                    calculatingCoroutine = CalculatingChance(roundNumber: 3);
                    StartCoroutine(calculatingCoroutine);
                }
            }
            if (FourthRoll)
            {
                RollDices();
                if (RollingIsCompleted && calculatingCoroutine == null)
                {
                    calculatingCoroutine = CalculatingChance(roundNumber: 4);
                    StartCoroutine(calculatingCoroutine);
                    // ile.AppendAllText("AI_logs",$"ENEMY_PickedDices = [ {String.Join(", ",ENEMY_PickedDices)} ] \n");
                    foreach (KeyValuePair<int, string> AI_Dice in dicesDict)
                    {
                        debugShowDict.Add(
                            $"Dice:{AI_Dice.Key}\t" +
                            $"Dice name: {dicesDict.Where(d => d.Key == AI_Dice.Key).First().Value}" +
                            $" -> Automatic last pick.");
                    }
                   print("Final round FOUR - auto pick last dices");
                    // EndTurn(4);
                }
            }
        }
        
        if(!isSkillSelected && AI_Player.skillsLoades && IsTurnON)
        {
            isSkillSelected = true; 
            if (AI_Player.CurrentGold_Value >= 8)
            {
                AI_Player.SelectLevel1Skill("Thor",2);
            }
        }
    }
    void AutomaticPopulateDicesInDictList(List<DiceRollScript> dices)
    {
        if (!RollingIsCompleted) return;
        if (isMockedDataInitiated) return;
        isMockedDataInitiated = true;
        debugShowDict.Add($"--------------------- Start new Round ---------------------");
        dicesDict.Clear();
        for (int i = 1; i <= 6; i++)
        {
            dicesDict.Add(key: i, value: dices.Where(d => d.DiceNumber == i && d.IsSentToBattlefield == false).FirstOrDefault().name.Remove(0, 2));
            //print(i + " / "+ dices.Where(d=>d.DiceNumber == i).FirstOrDefault().name.Remove(0,2));
        }
    }
    void UpdateLogger(KeyValuePair<int, string> currentCheckingDice)
    {
        debugShowDict.Add(
            $"Dice:{currentCheckingDice.Key}\t" +
            $"Rolled value:[{randomPickValueForDices.Where(p => p.Key == currentCheckingDice.Key).First().Value}] " +
            $"Pick value: {pickProbablityDict.Where(p => p.Key == currentCheckingDice.Key).First().Value}]\t" +
            $"Dice name: {dicesDict.Where(d => d.Key == currentCheckingDice.Key).First().Value}");

        //AndroidLogger.Log(debugShowDict.Last(), AndroidLogger.GetPlayerLogColor(AI_Player.Name));
    }
    List<string> AI_DicesLeftsInHand, AI_PickedDices, ENEMY_PickedDices;
    int ENEMY_Picked_axesCounter => ENEMY_PickedDices.Where(d => d.Contains("Axe")).Count();
    int ENEMY_Picked_bowsCounter => ENEMY_PickedDices.Where(d => d.Contains("Bow")).Count();
    int ENEMY_Picked_shieldsCounter => ENEMY_PickedDices.Where(d => d.Contains("Shield")).Count();
    int ENEMY_Picked_helmetsCounter => ENEMY_PickedDices.Where(d => d.Contains("Helmet")).Count();
    int ENEMY_Picked_handsCounter => ENEMY_PickedDices.Where(d => d.Contains("Hand")).Count();
    List<int> numberOFPickedDices = new List<int>();

    IEnumerator CalculatingChance(int roundNumber)
    {
        PopulateDictWithRandomPickingValue();

        if (dicesDict != null)
        {
            dicesDict = new Dictionary<int, string>();
            for (int i = 1; i <= 6; i++)
            {
                dicesDict.Add(key: i, value: AI_Player.DiceManager.Dices.Where(d => d.DiceNumber == i).FirstOrDefault().DiceImage.name.Remove(0, 2));
            }
           print("uaktualniono liste kosci");
        }

        ENEMY_PickedDices = GetListDiceNames(GetCurrentEnemyDicesInBattlefield);
        AI_PickedDices = GetListDiceNames(GetCurrentDicesInBattlefield);

        actualLeftDicesOnHand.Clear();
        foreach (var dice in dicesDict)
        {
            if (numberOFPickedDices.Contains(dice.Key)) continue;
            actualLeftDicesOnHand.Add(dice.Key, dice.Value);
        }
        //AI_DicesLeftsInHand = GetListDiceNames(actualLeftDicesOnHand);

        foreach (KeyValuePair<int, string> dice in dicesDict)
        {
            foreach (var diceIdOnBattleground in numberOFPickedDices)
            {
                if (actualLeftDicesOnHand.ContainsKey(diceIdOnBattleground))
                {
                    actualLeftDicesOnHand.Remove(diceIdOnBattleground);
                };
            }

            AI_DicesLeftsInHand = GetListDiceNames(actualLeftDicesOnHand);

            // foreach (int usedDice in numberOFPickedDices)
            // {
            //     // Deleting already picked dices from copied dictionary
            //     KeyValuePair<int,string> oldDice = actualLeftDicesOnHand.Where(d=>d.Key == usedDice).FirstOrDefault();
            //     if(oldDice.Value != null) actualLeftDicesOnHand.Remove(oldDice.Key);
            // }
            // Debug.LogWarning("actualLeftDicesOnHand po edycji: "+string.Join(" / ",actualLeftDicesOnHand.Values));
            // execute for every possesed dice one by one
            // var currentCheckingDice = actualLeftDicesOnHand.Where(d => d.Key == dice.Key).FirstOrDefault();
            // if(currentCheckingDice.Value == null) continue;

            //CalculatePickingValuesForOwnedDices(dice);
            // Debug.LogError("czy znajdje sie tam jeszcze kostka "+dice.Key +" => " +actualLeftDicesOnHand.ContainsKey(dice.Key));
            if (actualLeftDicesOnHand.ContainsKey(dice.Key))
            {
               print(AI_DicesLeftsInHand.Count() + " kostek na ręce");
                StartCoroutine(CalculatePickingValuesForOwnedDices(actualLeftDicesOnHand.Where(d => d.Key == dice.Key).First()));
                yield return new WaitUntil(() => calculatingNewPickValuesIsCompleted);

                if (CheckIfDiceShouldBePicked(diceNumber: dice.Key))
                {
                    PickDice(diceNumber: dice.Key);
                    numberOFPickedDices.Add(dice.Key);
                   print("Kosc została wybrana: " + dice.Value);
                    AI_PickedDices = GetListDiceNames(GetCurrentDicesInBattlefield);
                }
                yield return new WaitForSeconds(1f);
            }
        }

        // AI_PickedDices = GetListDiceNames(GetCurrentDicesInBattlefield);
        // Debug.LogWarning("numberOFPickedDices: "+string.Join(" / ",numberOFPickedDices));

        EndTurn(roundNumber);
    }
    IEnumerator CalculatePickingValuesForOwnedDices(KeyValuePair<int, string> currentCheckingDice)
    {
       print("Calculate picking value weight for dice: [ " + currentCheckingDice.Key + " | " + currentCheckingDice.Value + " ]");
        calculatingNewPickValuesIsCompleted = false;
        if (!isMockedDataInitiated) yield return null;
        if (calculatingNewPickValuesIsCompleted) yield return null;

        int calculatedPickValue = 0;
        RecalculatePickWeightValueBasedOnLogic(currentCheckingDice, out calculatedPickValue);
        yield return new WaitUntil(() => calculatedPickValue != 0);
        if (!pickProbablityDict.ContainsKey(currentCheckingDice.Key))
        {
            // jezeli nie zawiera numeru kostki więc jest to pewnie 1 tura, dodaj ją do dict'a
            pickProbablityDict.Add(key: currentCheckingDice.Key, value: calculatedPickValue);
        }
        else
        {
            // posiada juz ten klicz w dictie , nadpisz go nową wartością
            pickProbablityDict[currentCheckingDice.Key] = calculatedPickValue;
        }
        UI.SetPickChanceValues(diceNumber: currentCheckingDice.Key, dicePickChance: pickProbablityDict[currentCheckingDice.Key]);

        UpdateLogger(currentCheckingDice);
        calculatingNewPickValuesIsCompleted = true;
    }
    bool RecalculatePickWeightValueBasedOnLogic(KeyValuePair<int, string> dice, out int pickValue)
    {
        int AI_Picked_axesCounter = AI_PickedDices.Where(d => d.Contains("Axe")).Count();
        int AI_Picked_bowsCounter = AI_PickedDices.Where(d => d.Contains("Bow")).Count();
        int AI_Picked_shieldsCounter = AI_PickedDices.Where(d => d.Contains("Shield")).Count();
        int AI_Picked_helmetsCounter = AI_PickedDices.Where(d => d.Contains("Helmet")).Count();
        int AI_Picked_handsCounter = AI_PickedDices.Where(d => d.Contains("Hand")).Count();

        int AI_Available_axesCounter = AI_DicesLeftsInHand.Where(d => d.Contains("Axe")).Count();
        int AI_Available_bowsCounter = AI_DicesLeftsInHand.Where(d => d.Contains("Bow")).Count();
        int AI_Available_shieldsCounter = AI_DicesLeftsInHand.Where(d => d.Contains("Shield")).Count();
        int AI_Available_helmetsCounter = AI_DicesLeftsInHand.Where(d => d.Contains("Helmet")).Count();
        int AI_Available_handsCounter = AI_DicesLeftsInHand.Where(d => d.Contains("Hand")).Count();

        //print("====================================");
        //print("AI_Picked_axesCounter "+AI_Picked_axesCounter);
        //print("AI_Picked_bowsCounter "+AI_Picked_bowsCounter);
        //print("AI_Picked_shieldsCounter "+AI_Picked_shieldsCounter);
        //print("AI_Picked_helmetsCounter "+AI_Picked_helmetsCounter);
        //print("AI_Picked_handsCounter "+AI_Picked_handsCounter);

        //print("AI_Available_axesCounter "+AI_Available_axesCounter);
        //print("AI_Available_bowsCounter "+AI_Available_bowsCounter);
        //print("AI_Available_shieldsCounter "+AI_Available_shieldsCounter);
        //print("AI_Available_helmetsCounter "+AI_Available_helmetsCounter);
        //print("AI_Available_handsCounter "+AI_Available_handsCounter);
        //print("====================================");

        pickValue = RandomNumberGenerator.NumberBetween(1, 75);

        // TODO: "PROCES SPRAWDZANIA I SZACOWANIA OPŁACALNOŚCI WYBORU KOŚCI"
        //      uzyj actual left dices zeby przeszukac czy masz na ręce jakieś inne "może lepsze kości"
        //      i ile sztuk => w razie planowania obrony lub ataku
        //
        string dicename = dice.Value;
        bool isBlessed = false;
        if (dicename.Contains("Blessed"))
        {
            dicename = dicename.Replace("Blessed", "");
            isBlessed = true;
        }

        bool IfAvailableSameDiceButBlessed = AI_DicesLeftsInHand.Where(d => d.Contains("Blessed" + dicename)).Any();

        if (dice.Value.Contains("Helmet") || dice.Value.Contains("Shield"))
        {
           print($"ta kostka to {dicename} i może blokować {(dicename == "Helmet" ? "Axe" : "Bow")} przeciwnika");
            if (calculateHowMuchYouNeedToDeffenceYourself() >= 75)
            {
               print($"'Prawdopodobnie' powinieneś się bronić przed nadchodzącym atakiem");
                if (CheckIfOpponentHaveMoreAttackDicesToCover(deffenceDiceTypeName: dicename))
                {
                   print($"jezeli przeciwnik ma jeszcze jakieś {(dicename == "Helmet" ? "Axe" : "Bow")} do zablokowania");
                    if (CheckIfYouHaveMoreDeffenceDicesThanEnemyIncomeAttack(deffenceDiceTypeName: dicename))
                    {
                       print($"jeżeli masz dostępne {dicename} w ilości > większej niż pozostałe {(dicename == "Helmet" ? "Axe" : "Bow")} do zablokowania ");
                        if (IfAvailableSameDiceButBlessed)
                        {
                           print($"Posiadasz jeszcze inną kostke {dicename} która jest typu blessed.");
                            if (isBlessed)
                            {
                               print($"to jest kostka {dicename} typu blessed więc szansa na jej wybranie to 100%");
                                pickValue = 100;
                            }
                            else
                            {
                               print($"jeżeli masz do dyspozycji inny {dicename} typu blessed, aktualny zmniejsza szanse piknięcia o 25%");
                                pickValue -= 25;
                            }
                        }
                        else
                        {
                           print($"nie masz innych kości {dicename} typu blessed więc klasyczna ma 100% szansy na wybranie");
                            pickValue = 100;
                        }
                    }
                    else
                    {
                       print($"NIE MASZ dostępnych {dicename} w ilości > większej niż pozostałe {(dicename == "Helmet" ? "Axe" : "Bow")} do zablokowania ");
                       print($"więc bierzesz aktualny {dicename} na 100% szansy");
                        pickValue = 100;
                    }
                }
                else
                {
                   print($"przeciwnik nie ma już żadnych {(dicename == "Helmet" ? "Axe" : "Bow")} do zablokowania, ta kostka {dicename} ma 1% szansy na piknięcie");
                    pickValue = 1;
                }
            }
            else
            {
               print("'Prawdopodobnie' jesteś w stanie przyjąć nadchodzący atak na klate ;d");
            }
        }
        if (dice.Value.Contains("Hand"))
        {
           print("jest to kostka rąsi i służy do kradzieży");

            if (calculateHowMuchYouNeedToDeffenceYourself() >= 50)
            {
               print("jest duże parcie na obrone, łapki są ci teraz nie potrzebne, liczymy ze w nastepnej turze dropnie wiecej defa");
                pickValue = 1;
            }
            else
            {
                if (ENEMY_Player.CurrentGold_Value > 0)
                {
                   print("przeciwnik ma golda któego można ukraść");
                    if (isBlessed)
                    {
                       print("kostka rąsi na dodatek typu blessed, szansa na piknięcie +15%");
                        pickValue += 15;
                    }
                    else
                    {
                       print("kostka rąsi klasyczna, losowa wartośc picku");
                        pickValue = pickValue;
                    }
                }
                else
                {
                   print("przeciwnik nie ma nic co można ukraść, szansa wyboru łapki 1%");
                    pickValue = 1;
                }
            }
        }
        if (dice.Value.Contains("Axe"))
        {
            pickValue += 15;
        }
        if (dice.Value.Contains("Bow"))
        {
            pickValue += 15;
        }

        print("obliczanie opłacalności kości zakończone: " + dice.Value + " łącznie na ręce zostało jeszcze " + actualLeftDicesOnHand.Count() + " oplaca się piknąć tą kość w " + pickValue + "%");
        return true;

        // KROKI NIEZBĘDNE DO IDEALNEGO PICKA :d
        //  1. Czy przeciwnik ma wrzucone kości na arenie ?
        //  2. Czy masz wrzucone jakieś kości na arene ?
        //  3. Czy masz więcej lub rowno hp co przeciwnik ?
        //  4. Czy przeciwnik ma golda lub będzie mieć uwzględniając aktualne blessed kostki
        //  5. Czy jesteś w stanie przebić się przez obrone przeciwnika minimum na 1 dmg
        //      z aktualnie posiadanymi kostkami?
        //  6. Czy masz jakieś odpowiednie do ataku lub obrony blessed kostki?
        //  7. Czy przeciwnik ma dużo golda ?
        //  8. Czy jezeli nie jesteś w stanie się obronić i stracisz dużo hp, możesz dobrać kostki
        //      blessed lub zwiększyć szanse na piknięcie łapki w zamian użycia god skilla = heal
        //  9. Czy masz duzo wiecej hp niz przeciwnik i możesz skupić sie na zbieraniu golda żeby wykończyć
        //      przeciwnika skillem boga = atak
        bool CheckIfYouHaveMoreDeffenceDicesThanEnemyIncomeAttack(string deffenceDiceTypeName)
        {
            switch (deffenceDiceTypeName)
            {
                case "Shield":
                    if (AI_Available_shieldsCounter > (ENEMY_Picked_bowsCounter - AI_Picked_shieldsCounter)) return true;
                    break;

                case "Helmet":
                    if (AI_Available_helmetsCounter > (ENEMY_Picked_axesCounter - AI_Picked_helmetsCounter)) return true;
                    break;
            }
            return false;
        }
        bool CheckIfOpponentHaveMoreAttackDicesToCover(string deffenceDiceTypeName)
        {
            switch (deffenceDiceTypeName)
            {
                case "Shield":
                    if (ENEMY_Picked_bowsCounter - AI_Picked_shieldsCounter > 0) return true;
                    break;

                case "Helmet":
                    if (ENEMY_Picked_axesCounter - AI_Picked_helmetsCounter > 0) return true;
                    break;
            }
            return false;
        }
        int calculateHowMuchYouNeedToDeffenceYourself()
        {
            int deffProbality = 45;
            int healthDifference = ENEMY_Player.CurrentHealth_Value - AI_Player.CurrentHealth_Value;

            print("Różnica twojego hp do przeciwnika wynosi: " + healthDifference);

            // UWZGLĘDNIENIE RÓŻNICY W PUNKTACH HP
            ChangeDeffPRobablityDeppendOfHealthDifferences(ref deffProbality, healthDifference);

            // UWZGLĘDNIENIE AKTUALNEJ WARTOŚĆI HP
            ChangeDeffPRobablityDeppendOfCurrentHealth(ref deffProbality, ENEMY_Player.CurrentHealth_Value, AI_Player.CurrentHealth_Value);

            // UWZGLĘDNIENIE NADCHODZĄCEGO ATAKU - POZIOMU ZAGROŻENIA
            ChangeDeffProbablityDeppendOfEnemyAttackIncomming(ref deffProbality, AI_Player.CurrentHealth_Value, ENEMY_Picked_axesCounter, ENEMY_Picked_bowsCounter);

            if (deffProbality > 0)
            {

                print("ostatecznie obrona opłaca się na ok " + deffProbality + "%");
            }
            return deffProbality;

            static void ChangeDeffPRobablityDeppendOfHealthDifferences(ref int deffProbality, int healthDifference)
            {
                int chance;
                if (healthDifference >= 3)
                {
                    chance = RandomNumberGenerator.NumberBetween(25, 60);
                    deffProbality += chance;
                   print($"przeciwnik ma 3hp i więcej niż ty, szansa na obrone diametralnie wzrasta o {chance}%.[25-60%]");
                }
                else if (healthDifference < 3 && healthDifference > -3)
                {
                    chance = RandomNumberGenerator.NumberBetween(1, 25);
                    deffProbality += chance;
                   print($"masz w miare tyle samo hp co przeciwnik +2/-2, szansa na obrone ulega nieznacznej zmianie {chance}%.[1-25%]");
                }
                else if (healthDifference <= 3)
                {
                    chance = RandomNumberGenerator.NumberBetween(1, 25);
                    deffProbality -= chance;
                   print($"masz znacząco więcej hp niż przeciwnik +3 i wiecej, szansa na obrone zmniejsza sie {chance}%.[1-25%]");
                }
            }

            static void ChangeDeffPRobablityDeppendOfCurrentHealth(ref int deffProbality, int Enemy_HP, int AI_HP)
            {
                int chance = 0;
                if (Enemy_HP < 3)
                {
                    chance = RandomNumberGenerator.NumberBetween(10, 40);
                    deffProbality += chance;
                   print($"Przeciwnik ma mniej niż 4 hp, czas go dobić, szansa na obrone spada o {chance}%. [10-40%]");
                }

                if (AI_HP < 4)
                {
                    chance = RandomNumberGenerator.NumberBetween(25, 50);
                    deffProbality += chance;
                   print($"Masz mniej niż 4 hp, czas przyjąc postawe deffensywną, szansa na obrone wzrasta o {chance}%. [25-50%]");
                }
            }

            static void ChangeDeffProbablityDeppendOfEnemyAttackIncomming(ref int deffProbality, int AI_HP, int ENEMY_axesCounter, int ENEMY_bowsCounter)
            {
                int chance = 0;
                if (ENEMY_axesCounter + ENEMY_bowsCounter <= 2)
                {
                    chance = RandomNumberGenerator.NumberBetween(1, 25);
                    deffProbality -= chance;
                   print($"PRzeciwnik posiada na ręce mniej niż / równo 2 kości ataku, obrona nie jest tak potrzebna, zmniejszenie szansy o {chance}%. [1-25%]");
                }

                if (ENEMY_axesCounter + ENEMY_bowsCounter > 2)
                {
                    chance = RandomNumberGenerator.NumberBetween(10, 30);
                    deffProbality += chance;
                   print($"Przeciwnik posiada na ręce więcej niż 2 kości ataku, obrona jest potrzebna, zwiększenie szansy o {chance}%. [10-30%]");
                }
                if (ENEMY_axesCounter + ENEMY_bowsCounter > 4)
                {
                    chance = RandomNumberGenerator.NumberBetween(30, 50);
                    deffProbality += chance;
                   print($"Przeciwnik ma zdecydowanie za dużo kości ataku na polu, obrona jest wymagana !, zwiększenie szansy o kolejne  {chance}%. [30-50%]");
                }

                if (AI_HP - (ENEMY_axesCounter + ENEMY_bowsCounter) <= 0)
                {
                    deffProbality = 100;
                   print($"Nadchodzący atak jest w stanie cie pokonać, obrona jest wymagana w 100%. [100%]");
                }

                if ((ENEMY_axesCounter + ENEMY_bowsCounter) == 0)
                {
                    deffProbality = 0;
                   print($"PRzeciwnik cie nie atakuje, obrona nie jest konieczna. [0%]");
                }

            }
        }
    }
    List<String> GetListDiceNames(List<DiceRollScript> playerDices) => new List<string>(playerDices.Select(d => d.DiceImage.name.Remove(0, 2)).ToList());
    List<String> GetListDiceNames(Dictionary<int, string> playerDices) => new List<string>(playerDices.Select(d => d.Value).ToList());

    bool CheckIfDiceShouldBePicked(int diceNumber)
    {
        int dicePickRequirments = pickProbablityDict.Where(d => d.Key == diceNumber).First().Value;
        int rolledPickValue = randomPickValueForDices.Where(d => d.Key == diceNumber).First().Value;

        if (rolledPickValue < dicePickRequirments)
        {
            return true;
        }
        return false;
    }
    void PopulateDictWithRandomPickingValue()
    {
        if (!itsNeedToReCalculateRandomPickingValues) return;

        for (int i = 1; i <= 6; i++)
        {
            if (!randomPickValueForDices.ContainsKey(i))
            {
                randomPickValueForDices.Add(key: i, value: RandomNumberGenerator.NumberBetween(1, 100));
            }
            else
            {
                randomPickValueForDices[i] = RandomNumberGenerator.NumberBetween(1, 100);
            }
        }

        itsNeedToReCalculateRandomPickingValues = false;
    }
    void RollDices()
    {
        if (!IsRollAllowed) return;

        AI_Player.DiceManager.OnClick_ROLLDICES();
        AI_Player.GameManager.SwapRollButonWithEndTurn_OnClick(AI_Player.Name);
        IsRollAllowed = false;
    }
    void PickDice(int diceNumber)
    {
        int diceIndex = diceNumber - 1;
        Button diceButton = AI_Player.DiceManager.transform
        .GetChild(diceIndex)
        .GetComponent<Button>();

        if (diceButton.IsInteractable()) diceButton.onClick.Invoke();
    }
    void EndTurn(int turnNumber)
    {
        AI_DicesLeftsInHand = GetListDiceNames(actualLeftDicesOnHand);
        ///*  File.AppendAllText("AI_logs", */ AndroidLogger.Log($"AI_DicesInHand = [ {String.Join(", ",AI_DicesLeftsInHand)} ] \n");
        // /*  File.AppendAllText("AI_logs", */ AndroidLogger.Log($"AI_PickedDices = [ {String.Join(", ",AI_PickedDices)} ] \n");
        // /*  File.AppendAllText("AI_logs", */ AndroidLogger.Log($"ENEMY_PickedDices = [ {String.Join(", ",ENEMY_PickedDices)} ] \n");

        switch (turnNumber)
        {
            case 1:
                FirstRoll = false;
                SecondRoll = true;
                break;

            case 2:
                SecondRoll = false;
                ThirdRoll = true;
                break;

            case 3:
                ThirdRoll = false;
                FourthRoll = true;
                break;

            case 4:
                FourthRoll = false;
                actualLeftDicesOnHand.Clear();
                numberOFPickedDices.Clear();
                break;
        }


       print("EndTurn");
        this.transform.Find("EndTurnButton").GetComponent<Button>().onClick.Invoke();
        IsRollAllowed = true;
        itsNeedToReCalculateRandomPickingValues = true;
        isMockedDataInitiated = true;
        calculatingNewPickValuesIsCompleted = false;

        calculatingCoroutine = null;
    }
    public void TurnONOFF() => IsTurnON = !IsTurnON;
    public void OnClick_TurnOnAI()
    {
        TurnONOFF();
        if (IsTurnON)
        {
            transform.Find("AIIcon_Button").GetComponent<Image>().color = new Color32(0, 255, 0, 255);
        }
        else
        {
            transform.Find("AIIcon_Button").GetComponent<Image>().color = new Color32(255, 0, 0, 128);
        }
    }




}
