using System.Data.Common;
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
    [SerializeField] public Player AI_Player;
    [SerializeField] Player ENEMY_Player;
    [SerializeField] AIPickChanceUi UI;

    [SerializeField] public bool FirstRoll, SecondRoll, ThirdRoll, FourthRoll, IsTurnON;
    [SerializeField] bool isMockedDataInitiated, calculatingNewPickValuesIsCompleted, itsNeedToReCalculateRandomPickingValues = true;
    [SerializeField] public bool IsRollAllowed = true;
    Dictionary<int, string> dicesDict = new Dictionary<int, string>();              // diceNumber <1;6> / "nazwa kostki"
    Dictionary<int, string> actualLeftDicesOnHand = new Dictionary<int, string>();  // COPY of diceDict
    Dictionary<int, int> pickProbablityDict = new Dictionary<int, int>();           // diceNumber <1;6> / prawdopodobieństwo bycia wybraną <1;100>
    Dictionary<int, int> randomPickValueForDices = new Dictionary<int, int>();       // diceNumber <1;6> / losowa wartość do wybrania <1;100>

    [SerializeField] List<string> debugShowDict = new List<string>();
    List<DiceRollScript> GetCurrentEnemyDicesInBattlefield => ENEMY_Player.ListOfDicesOnBattleground;
    List<DiceRollScript> GetCurrentDicesInBattlefield => AI_Player.ListOfDicesOnBattleground;
    bool RollingIsCompleted => AI_Player.DiceManager.Dices.Where(d => d.RollingIsCompleted == false).Any() ? false : true;
    IEnumerator calculatingCoroutine = null;
    [SerializeField] bool needToRollDices = true;
    [SerializeField] bool isSkillSelected = false, THOR = true, IDUN = true;
    [SerializeField] bool isEndGameWindowsIsClosed => AI_Player.GameManager.EndGameResultWindows.transform.GetChild(0).gameObject.activeSelf == false && AI_Player.GameManager.EndGameResultWindows.transform.GetChild(1).gameObject.activeSelf == false;

    void Start()
    {
        UI = this.GetComponentInChildren<AIPickChanceUi>();
        StartCoroutine(AI_Player.LoadSkillsData());
    }

    void FixedUpdate()
    {
   
        if (isEndGameWindowsIsClosed)
        {
            if (IsTurnON && !AI_Player.TurnBlocker.activeSelf)
            {
                if (FirstRoll)
                {
                    //FirstRoll = false;
                    StartCoroutine(WykonanieRund_1());
                }
                else if (SecondRoll)
                {
                    //SecondRoll = false;
                    StartCoroutine(WykonaniemRund_2_3_4(2));
                }
                else if (ThirdRoll)
                {
                    //ThirdRoll = false;
                    StartCoroutine(WykonaniemRund_2_3_4(3));
                }
                else if (FourthRoll)
                {
                    //FourthRoll = false;
                    StartCoroutine(WykonaniemRund_2_3_4(4));
                }
            }
        }
    }

    IEnumerator WykonanieRund_1()
    {
        FirstRoll = false;
        
        var whostartCombat = AI_Player.GameManager.CombatManager_Script.RecentAttacker;
        // z automatu 1 graczem jest player 2, więc 1 atakującym jest gracz 1 ( przeciwnik ) 
        // jeżeli wartość recentattacker jest pusta, czyli nie obyłą sie jeszcze zadna walka
        // zaczyna gracz 2;
        whostartCombat = whostartCombat==""?"Player2":whostartCombat;

        // zmiana nazwy ostatniego przciwnika na nazwe przeciwnika aktualnie zaczynającego woalke
        whostartCombat = whostartCombat=="Player1"?"Player2":"Player1";

      //  print("sprawdzenie kto komu pierwszy zacznie podkradać golda, zaczyna "+whostartCombat);

        yield return new WaitUntil(() => needToRollDices);
        RollDices();

        yield return new WaitUntil(() => (RollingIsCompleted && calculatingCoroutine == null));
        PrzejdzPrzezProcesILogikeDlaRundy_1();
    }
    IEnumerator WykonaniemRund_2_3_4(int rundNumber)
    {
        SecondRoll = false;
        ThirdRoll = false;
        FourthRoll = false;

        yield return new WaitUntil(() => needToRollDices);
        RollDices();

        yield return new WaitUntil(() => (RollingIsCompleted && calculatingCoroutine == null));
        PrzejdzPrzezProcesILogikeDlaRund_2_3_4(rundNumber);
    }

    private void PrzejdzPrzezProcesILogikeDlaRundy_1()
    {
        isSkillSelected = false;
        isMockedDataInitiated = false;
        try{
            AutomaticPopulateDicesInDictList(AI_Player.DiceManager.Dices);
        }
        catch (Exception ex)
        {
            print("o to tu chodzi ?... nie wiem ["+ex.Message+"]");
        }
        calculatingCoroutine = CalculatingChance(roundNumber: 1);
        StartCoroutine(calculatingCoroutine);
    }
    private void PrzejdzPrzezProcesILogikeDlaRund_2_3_4(int rundNumber)
    {
        if (rundNumber != 4) isSkillSelected = false; //  4 rundzie nie ma szansy juz zmienic skila 

        calculatingCoroutine = CalculatingChance(rundNumber);
        StartCoroutine(calculatingCoroutine);
    }

    private void TrySelectAnyAvaiableSkillFromGod(string godName)
    {
        if(godName == "Thor" && !THOR) return;
        if(godName == "Idun" && !IDUN) return; 

        int choosenSkillLevel = 0;
        if (!isSkillSelected && AI_Player.skillsLoades)
        {
            int avaiableGold = AI_Player.CurrentGold_Value;
            
            if (godName == "Thor")
            {
                if (avaiableGold >= 4)
                {
                    //AndroidLogger.Log("mozna wybrac skill lvl 1 aktualnie mam " + AI_Player.CurrentGold_Value + " Golda");
                    choosenSkillLevel = 1;
                }
                if (avaiableGold >= 8)
                {
                    //AndroidLogger.Log("mozna wybrac skill lvl 2 aktualnie mam " + AI_Player.CurrentGold_Value + " Golda");
                    choosenSkillLevel = 2;
                }
                if (avaiableGold >= 12)
                {
                    //AndroidLogger.Log("mozna wybrac skill lvl 3 aktualnie mam " + AI_Player.CurrentGold_Value + " Golda");
                    choosenSkillLevel = 3;
                }

                isSkillSelected = choosenSkillLevel > 0 ? true : false;
                if (choosenSkillLevel > 0) AndroidLogger.Log("Wybrano skill thora na posiomie " + choosenSkillLevel, AndroidLogger.GetPlayerLogColor(AI_Player.Name));
                AI_Player.SelectLevel1Skill(godName, choosenSkillLevel);
            }            
            if (godName == "Idun")
            {
                if (avaiableGold >= 4)
                {
                    //AndroidLogger.Log("mozna wybrac skill lvl 1 aktualnie mam " + AI_Player.CurrentGold_Value + " Golda");
                    choosenSkillLevel = 1;
                }
                if (avaiableGold >= 7)
                {
                    //AndroidLogger.Log("mozna wybrac skill lvl 2 aktualnie mam " + AI_Player.CurrentGold_Value + " Golda");
                    choosenSkillLevel = 2;
                }
                if (avaiableGold >= 10)
                {
                    //  AndroidLogger.Log("mozna wybrac skill lvl 3 aktualnie mam " + AI_Player.CurrentGold_Value + " Golda");
                    choosenSkillLevel = 3;
                }

                isSkillSelected = choosenSkillLevel > 0 ? true : false;
                if (choosenSkillLevel > 0) AndroidLogger.Log("Wybrano skill Idun`y na posiomie " + choosenSkillLevel, AndroidLogger.GetPlayerLogColor(AI_Player.Name));
                AI_Player.SelectLevel1Skill(godName, choosenSkillLevel);
            }

        }
    }

    private string CalculateWhichGodChoose()
    {
        var whoGonnaFirstAttackWithSkillInCombat = AI_Player.GameManager.PlayerWhoMakeFirstRollinCurrentGameSession;
        string godToChoose = "";

        // Thor => ATAK
        //      MASZ PIERWSZEŃŚTWO ?
        //          PRZECIWNIK ZEJDZIE NA HITA :D?
            if(whoGonnaFirstAttackWithSkillInCombat == AI_Player.Name) godToChoose = "Thor";
            if(AI_Player.CurrentHealth_Value >= 5) godToChoose = "Thor";

        // Idun => OBRONA
        //      MASZ PIERWSZEŃŚTWO ?
        //          ZDĄZYSZ OBRONIĆ SIĘ PRZED ŚMIERCIĄ ?
            if(AI_Player.CurrentHealth_Value <5) godToChoose = "Idun";

        return godToChoose;
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
        }

        ENEMY_PickedDices = GetListDiceNames(GetCurrentEnemyDicesInBattlefield);
        AI_PickedDices = GetListDiceNames(GetCurrentDicesInBattlefield);

        actualLeftDicesOnHand.Clear();
        foreach (var dice in dicesDict)
        {
            if (numberOFPickedDices.Contains(dice.Key)) continue;
            actualLeftDicesOnHand.Add(dice.Key, dice.Value);
        }

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

            if (actualLeftDicesOnHand.ContainsKey(dice.Key))
            {
                StartCoroutine(CalculatePickingValuesForOwnedDices(actualLeftDicesOnHand.Where(d => d.Key == dice.Key).First()));

                yield return new WaitUntil(() => calculatingNewPickValuesIsCompleted);

                if (CheckIfDiceShouldBePicked(diceNumber: dice.Key))
                {
                    PickDice(diceNumber: dice.Key);
                    numberOFPickedDices.Add(dice.Key);
                    AI_PickedDices = GetListDiceNames(GetCurrentDicesInBattlefield);
                }
                yield return new WaitForSeconds(0.2f);
            }
        }

        EndTurn(roundNumber);
    }
    IEnumerator CalculatePickingValuesForOwnedDices(KeyValuePair<int, string> currentCheckingDice)
    {
        calculatingNewPickValuesIsCompleted = false;
        if (!isMockedDataInitiated) yield return null;
        if (calculatingNewPickValuesIsCompleted) yield return null;

        int calculatedPickValue = 0;
        RecalculatePickWeightValueBasedOnLogic(currentCheckingDice, out calculatedPickValue);
        yield return new WaitUntil(() => calculatedPickValue != 0);
        if (!pickProbablityDict.ContainsKey(currentCheckingDice.Key))
        {
            pickProbablityDict.Add(key: currentCheckingDice.Key, value: calculatedPickValue);
        }
        else
        {
            pickProbablityDict[currentCheckingDice.Key] = calculatedPickValue;
        }
        UI.SetPickChanceValues(diceNumber: currentCheckingDice.Key, dicePickChance: pickProbablityDict[currentCheckingDice.Key]);

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

        pickValue = RandomNumberGenerator.NumberBetween(1, 75);

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
            //...print($"ta kostka to {dicename} i może blokować {(dicename == "Helmet" ? "Axe" : "Bow")} przeciwnika");
            if (calculateHowMuchYouNeedToDeffenceYourself() >= 75)
            {
                //...print($"'Prawdopodobnie' powinieneś się bronić przed nadchodzącym atakiem");
                if (CheckIfOpponentHaveMoreAttackDicesToCover(deffenceDiceTypeName: dicename))
                {
                    //...print($"jezeli przeciwnik ma jeszcze jakieś {(dicename == "Helmet" ? "Axe" : "Bow")} do zablokowania");
                    if (CheckIfYouHaveMoreDeffenceDicesThanEnemyIncomeAttack(deffenceDiceTypeName: dicename))
                    {
                        //...print($"jeżeli masz dostępne {dicename} w ilości > większej niż pozostałe {(dicename == "Helmet" ? "Axe" : "Bow")} do zablokowania ");
                        if (IfAvailableSameDiceButBlessed)
                        {
                            //...print($"Posiadasz jeszcze inną kostke {dicename} która jest typu blessed.");
                            if (isBlessed)
                            {
                                //...print($"to jest kostka {dicename} typu blessed więc szansa na jej wybranie to 100%");
                                pickValue = 100;
                            }
                            else
                            {
                                //...print($"jeżeli masz do dyspozycji inny {dicename} typu blessed, aktualny zmniejsza szanse piknięcia o 25%");
                                pickValue -= 25;
                            }
                        }
                        else
                        {
                            //...print($"nie masz innych kości {dicename} typu blessed więc klasyczna ma 100% szansy na wybranie");
                            pickValue = 100;
                        }
                    }
                    else
                    {
                        //...print($"NIE MASZ dostępnych {dicename} w ilości > większej niż pozostałe {(dicename == "Helmet" ? "Axe" : "Bow")} do zablokowania ");
                        //...print($"więc bierzesz aktualny {dicename} na 100% szansy");
                        pickValue = 100;
                    }
                }
                else
                {
                    //...print($"przeciwnik nie ma już żadnych {(dicename == "Helmet" ? "Axe" : "Bow")} do zablokowania, ta kostka {dicename} ma 1% szansy na piknięcie");
                    pickValue = 1;
                }
            }
            else
            {
                //...print("'Prawdopodobnie' jesteś w stanie przyjąć nadchodzący atak na klate ;d");
            }
        }
        if (dice.Value.Contains("Hand"))
        {
            //...print("jest to kostka rąsi i służy do kradzieży");

            if (calculateHowMuchYouNeedToDeffenceYourself() >= 50)
            {
                //...print("jest duże parcie na obrone, łapki są ci teraz nie potrzebne, liczymy ze w nastepnej turze dropnie wiecej defa");
                pickValue = 1;
            }
            else
            {
                if (ENEMY_Player.CurrentGold_Value > 0)
                {
                    //...print("przeciwnik ma golda któego można ukraść");
                    if (isBlessed)
                    {
                        //...print("kostka rąsi na dodatek typu blessed, szansa na piknięcie +15%");
                        pickValue += 15;
                    }
                    else
                    {
                        //...print("kostka rąsi klasyczna, losowa wartośc picku");
                        pickValue = pickValue;
                    }
                }
                else
                {
                    //...print("przeciwnik nie ma nic co można ukraść, szansa wyboru łapki 1%");
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

        return true;

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

            // UWZGLĘDNIENIE RÓŻNICY W PUNKTACH HP
            ChangeDeffPRobablityDeppendOfHealthDifferences(ref deffProbality, healthDifference);

            // UWZGLĘDNIENIE AKTUALNEJ WARTOŚĆI HP
            ChangeDeffPRobablityDeppendOfCurrentHealth(ref deffProbality, ENEMY_Player.CurrentHealth_Value, AI_Player.CurrentHealth_Value);

            // UWZGLĘDNIENIE NADCHODZĄCEGO ATAKU - POZIOMU ZAGROŻENIA
            ChangeDeffProbablityDeppendOfEnemyAttackIncomming(ref deffProbality, AI_Player.CurrentHealth_Value, ENEMY_Picked_axesCounter, ENEMY_Picked_bowsCounter);

            return deffProbality;

            static void ChangeDeffPRobablityDeppendOfHealthDifferences(ref int deffProbality, int healthDifference)
            {
                int chance;
                if (healthDifference >= 3)
                {
                    chance = RandomNumberGenerator.NumberBetween(25, 60);
                    deffProbality += chance;
                    //...print($"przeciwnik ma 3hp i więcej niż ty, szansa na obrone diametralnie wzrasta o {chance}%.[25-60%]");
                }
                else if (healthDifference < 3 && healthDifference > -3)
                {
                    chance = RandomNumberGenerator.NumberBetween(1, 25);
                    deffProbality += chance;
                    //...print($"masz w miare tyle samo hp co przeciwnik +2/-2, szansa na obrone ulega nieznacznej zmianie {chance}%.[1-25%]");
                }
                else if (healthDifference <= 3)
                {
                    chance = RandomNumberGenerator.NumberBetween(1, 25);
                    deffProbality -= chance;
                    //...print($"masz znacząco więcej hp niż przeciwnik +3 i wiecej, szansa na obrone zmniejsza sie {chance}%.[1-25%]");
                }
            }

            static void ChangeDeffPRobablityDeppendOfCurrentHealth(ref int deffProbality, int Enemy_HP, int AI_HP)
            {
                int chance = 0;
                if (Enemy_HP < 3)
                {
                    chance = RandomNumberGenerator.NumberBetween(10, 40);
                    deffProbality += chance;
                    //...print($"Przeciwnik ma mniej niż 4 hp, czas go dobić, szansa na obrone spada o {chance}%. [10-40%]");
                }

                if (AI_HP < 4)
                {
                    chance = RandomNumberGenerator.NumberBetween(25, 50);
                    deffProbality += chance;
                    //...print($"Masz mniej niż 4 hp, czas przyjąc postawe deffensywną, szansa na obrone wzrasta o {chance}%. [25-50%]");
                }
            }

            static void ChangeDeffProbablityDeppendOfEnemyAttackIncomming(ref int deffProbality, int AI_HP, int ENEMY_axesCounter, int ENEMY_bowsCounter)
            {
                int chance = 0;
                if (ENEMY_axesCounter + ENEMY_bowsCounter <= 2)
                {
                    chance = RandomNumberGenerator.NumberBetween(1, 25);
                    deffProbality -= chance;
                    //...print($"PRzeciwnik posiada na ręce mniej niż / równo 2 kości ataku, obrona nie jest tak potrzebna, zmniejszenie szansy o {chance}%. [1-25%]");
                }

                if (ENEMY_axesCounter + ENEMY_bowsCounter > 2)
                {
                    chance = RandomNumberGenerator.NumberBetween(10, 30);
                    deffProbality += chance;
                    //...print($"Przeciwnik posiada na ręce więcej niż 2 kości ataku, obrona jest potrzebna, zwiększenie szansy o {chance}%. [10-30%]");
                }
                if (ENEMY_axesCounter + ENEMY_bowsCounter > 4)
                {
                    chance = RandomNumberGenerator.NumberBetween(30, 50);
                    deffProbality += chance;
                    //...print($"Przeciwnik ma zdecydowanie za dużo kości ataku na polu, obrona jest wymagana !, zwiększenie szansy o kolejne  {chance}%. [30-50%]");
                }

                if (AI_HP - (ENEMY_axesCounter + ENEMY_bowsCounter) <= 0)
                {
                    deffProbality = 100;
                    //...print($"Nadchodzący atak jest w stanie cie pokonać, obrona jest wymagana w 100%. [100%]");
                }

                if ((ENEMY_axesCounter + ENEMY_bowsCounter) == 0)
                {
                    deffProbality = 0;
                    //...print($"PRzeciwnik cie nie atakuje, obrona nie jest konieczna. [0%]");
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
        if (IsRollAllowed)
        {
            needToRollDices = false;

            AI_Player.DiceManager.OnClick_ROLLDICES();
            AI_Player.GameManager.SwapRollButonWithEndTurn_OnClick(AI_Player.Name);
            IsRollAllowed = false;
        }
    }
    void PickDice(int diceNumber)
    {
        int diceIndex = diceNumber - 1;
        Button diceButton = AI_Player.DiceManager.transform
        .GetChild(diceIndex)
        .GetComponent<Button>();

        if (diceButton.IsInteractable()) diceButton.onClick.Invoke();
    }
    [SerializeField] public bool bot_can_use_skills = false;
    [SerializeField] public bool smartSkillSelect = false;

    void EndTurn(int turnNumber)
    {
        AI_DicesLeftsInHand = GetListDiceNames(actualLeftDicesOnHand);

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

        var values = Enum.GetValues(typeof(gods));

        if (bot_can_use_skills)
        {
                string godName = ((gods)values.GetValue(RandomNumberGenerator.NumberBetween(0, values.Length - 1))).ToString();
                if(smartSkillSelect) godName = CalculateWhichGodChoose();
                TrySelectAnyAvaiableSkillFromGod(godName);
        }

        IsRollAllowed = true;
        itsNeedToReCalculateRandomPickingValues = true;
        isMockedDataInitiated = true;
        calculatingNewPickValuesIsCompleted = false;
        calculatingCoroutine = null;
        needToRollDices = true;

        this.transform.Find("EndTurnButton").GetComponent<Button>().onClick.Invoke();
    }
    enum gods
    {
        Thor,
        Idun
    }
    public void TurnONOFF() => IsTurnON = !IsTurnON;
    public void OnClick_TurnOnOFFAI()
    {
        TurnONOFF();
        print("Wylaczenie/Włączenie bota");
        if (IsTurnON)
        {
            print("zmiana koloru na zielony");
            transform.Find("AIIcon_Button").GetComponent<Image>().color = new Color32(0, 255, 0, 255);
        }
        else
        {
            print("zmiana koloru na czerwony");
            transform.Find("AIIcon_Button").GetComponent<Image>().color = new Color32(255, 0, 0, 128);
        }
    }
}
