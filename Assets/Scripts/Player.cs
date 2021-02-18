using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    [SerializeField] private List<DiceRollScript> _listOfDicesOnBattleground;
    [SerializeField] private int _currentHealth_Value;
    public int CurrentHealth_Value
    {
        get => _currentHealth_Value;
        set
        {
            HPPoints_Text.text = value.ToString();
            _currentHealth_Value = Convert.ToInt32(HPPoints_Text.text);
        }
    }

    [SerializeField] private int _currentGold_Value;
    public int CurrentGold_Value
    {
        get =>  _currentGold_Value;
        set
        {
            GoldVault_Text.text = value.ToString();
            _currentGold_Value = Convert.ToInt32(GoldVault_Text.text);
        }
    }

//-------------------------------------------------

    public GameManager GameManager;
    public string Name;
    public int RollingCounter;
    public GodsManager GodsManager_Script;
    public GameObject UseSkillTestButton;
    public GameObject GodSkillWindow;
    public GameObject TurnBlocker;

    #region GOLD Blessed + Steal
      public Text GoldVault_Text;
      public int _cumulativeGoldStealingCounter;
      public int CumulativeGoldStealingCounter { get => _cumulativeGoldStealingCounter; set => _cumulativeGoldStealingCounter = value; }
      private int _liczbaPrzelewowGolda;
      public int LiczbaPrzelewowGolda 
      { 
          get => _liczbaPrzelewowGolda; 
          set 
          {
              _liczbaPrzelewowGolda = value; 
          }
      }  
      public TextMeshProUGUI coinText_TMP;
      int _temporaryGoldVault;
      public int TemporaryGoldVault
      {
          get
          {
              return _temporaryGoldVault;
          }
          set
          {
              if (value > 0)
              {
                  // DODAWANIE GOLDA
                  if (value != 0)
                  {
                      CumulativeGoldStealingCounter++;
                      coinText_TMP.SetText("+" + CumulativeGoldStealingCounter.ToString());
                      LiczbaPrzelewowGolda++;
                  }
              }
              if (value < 0)
              {
                  // ODEJMOWANIE GOLDA
                  if (value != 0)
                  {
                      CumulativeGoldStealingCounter--;
                      coinText_TMP.SetText(CumulativeGoldStealingCounter.ToString());
                      LiczbaPrzelewowGolda--;
                  }
              }
              _temporaryGoldVault = value;
          }
      }
    #endregion

#region HEALTH Combat    
    [SerializeField] public Text HPPoints_Text;
    [SerializeField] public TextMeshProUGUI HealthText_TMP;
    [SerializeField] int liczbaPrzelewaniaObrazen;
    [SerializeField] int _temporaryIntakeDamage;
    public int TemporaryIntakeDamage
    {
        get
        {
            return _temporaryIntakeDamage;
        }
        set
        {
            _temporaryIntakeDamage = value;
            
            if(GameManager.IsGameEnded == false)
            {
                if(CurrentHealth_Value <= 0)
                {
                    print($"{Name} Actual HP = "+CurrentHealth_Value);
                    string winnerName = Name=="Player1"?"Player2":"Player1";
                    GameManager.LastGameWinner = Name =="Player1"?"Player2":"Player1"; 
                    GameManager.ShowEndGameResultWindow(winner:winnerName);
                    _temporaryIntakeDamage = 0;
                }
            }
            if (value > 0)
            {
                HealthText_TMP.SetText("-" + _temporaryIntakeDamage.ToString());
                HealthText_TMP.color = Color.red;
                liczbaPrzelewaniaObrazen++;

            }

            if (value < 0)
            {
                print("value: " + value);

                print("różnica : " + (TemporaryIntakeDamage - value).ToString());

                HealthText_TMP.SetText("+" + _temporaryIntakeDamage.ToString());
                HealthText_TMP.color = Color.green;
                liczbaPrzelewaniaObrazen--;
            }
            
            if (value == 0)
            {
                HealthText_TMP.SetText("");
            }
        }
    }

    public List<DiceRollScript> ListOfDicesOnBattleground 
    { 
        get => _listOfDicesOnBattleground; 
        set 
        {
            _listOfDicesOnBattleground = value; 
        }
    }

    public DiceManager DiceManager;
    #endregion



    void Start()
    {
        ListOfDicesOnBattleground = new List<DiceRollScript>();
        CurrentGold_Value = Convert.ToInt32(GoldVault_Text.text);
        CurrentHealth_Value = Convert.ToInt32(HPPoints_Text.text);
        HealthText_TMP.text = "";

        RollingCounter = 0;
    }
    internal void TransferGold()
    {
        if (LiczbaPrzelewowGolda > 0)
            {
                // DODAWANIE GOLDA
                CurrentGold_Value++;
                //CurrentGold++;
                //GoldVault_Text.text = CurrentGold.ToString();
                LiczbaPrzelewowGolda--;
                GodsManager_Script.CollorSkillButtonsIfCanBeUsed();
            }
            else if (LiczbaPrzelewowGolda < 0)
            {
                // ODEJMOWANIE GOLDA
                CurrentGold_Value--;;
              //  GoldVault_Text.text = CurrentGold.ToString();
                LiczbaPrzelewowGolda++;
                GodsManager_Script.CollorSkillButtonsIfCanBeUsed();
            }
            else if (LiczbaPrzelewowGolda == 0)
            {
                // ZEROWANIE WARTOSCI TYMCZASOWYCH
                TemporaryGoldVault = 0;
                LiczbaPrzelewowGolda = 0;

                if (CumulativeGoldStealingCounter == 0)
                {
                    coinText_TMP.SetText("");
                }
            }
    }

    internal void TransferDamage()
    {
            if (liczbaPrzelewaniaObrazen > 0)
            {
                // DAMAGING
                CurrentHealth_Value--;

                liczbaPrzelewaniaObrazen--;
            }
            else if (liczbaPrzelewaniaObrazen < 0)
            {
                // HEALING
                CurrentHealth_Value++;

                liczbaPrzelewaniaObrazen++;
            }
            else
            {
                if (liczbaPrzelewaniaObrazen == 0)
                {
                    TemporaryIntakeDamage = 0;
                }
            }
    }








    public bool skillsLoades = false;
    public IEnumerator LoadSkillsData()
    {
        print("start");
        GodSkillWindow.SetActive(true);
        yield return new WaitUntil(()=>GodsManager_Script._godCardsInContainer.First()._skill != null);
        GodSkillWindow.SetActive(false);
        print("end");
        skillsLoades = true;
    }
    public void SelectLevel1Skill(string godName, int level)
    {   
        if(level > 0)
        {
            GodSkillWindow.SetActive(true);
            CardScript CS = GodsManager_Script._godCardsInContainer.Where(c=>c._card.name == godName).First()._card;
            CS._godTotem._skill.TrySelectSkill(level,Name, CS._godTotem._godData);
            GodSkillWindow.SetActive(false);
        }
    }
}
