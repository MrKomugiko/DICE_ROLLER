using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameManager GM;
    public string Name;
    public int RollingCounter;
    public GodsManager GodsManager_Script;
    public GameObject UseSkillTestButton;
    public GameObject GodSkillWindow;
    public GameObject TurnBlocker;

    #region GOLD Blessed + Steal
      public Text GoldVault;
      public int _cumulativeGoldStealingCounter;
      public int CumulativeGoldStealingCounter { get => _cumulativeGoldStealingCounter; set => _cumulativeGoldStealingCounter = value; }
      private int _currentGold;
      public int CurrentGold
      {
          get => _currentGold;
          set
          {
          _currentGold = value;
          }
      }
      private int _liczbaPrzelewowGolda;
      public int LiczbaPrzelewowGolda 
      { 
          get => _liczbaPrzelewowGolda; 
          set 
          {
              _liczbaPrzelewowGolda = value; 
          }
      }  
      public TextMeshProUGUI coinText;
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
                      coinText.SetText("+" + CumulativeGoldStealingCounter.ToString());
                      LiczbaPrzelewowGolda++;
                  }
              }
              if (value < 0)
              {
                  // ODEJMOWANIE GOLDA
                  if (value != 0)
                  {
                      CumulativeGoldStealingCounter--;
                      coinText.SetText(CumulativeGoldStealingCounter.ToString());
                      LiczbaPrzelewowGolda--;
                  }
              }
              _temporaryGoldVault = value;
          }
      }
    #endregion

#region HEALTH Combat    
    [SerializeField] Text HPPointsText;
    [SerializeField] int ActualHPValue;
    [SerializeField] int liczbaPrzelewaniaObrazen;
    [SerializeField] private bool IsGameEnded => GM.IsGameEnded; 
    public TextMeshProUGUI HealthText;
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
            
            if(IsGameEnded == false)
            {
                if(ActualHPValue <= 0)
                {
                    print($"{Name} Actual HP = "+ActualHPValue);
                    GM.IsGameEnded = true;
                    string winnerName = Name=="Player1"?"Player2":"Player1";
                    GM.ShowEndGameResultWindow(winner:winnerName);
                    _temporaryIntakeDamage = 0;
                }
            }
            if (value > 0)
            {
                HealthText.SetText("-" + _temporaryIntakeDamage.ToString());
                HealthText.color = Color.red;
                liczbaPrzelewaniaObrazen++;

            }

            if (value < 0)
            {
                print("value: " + value);

                print("różnica : " + (TemporaryIntakeDamage - value).ToString());

                HealthText.SetText("+" + _temporaryIntakeDamage.ToString());
                HealthText.color = Color.green;
                liczbaPrzelewaniaObrazen--;
            }
            
            if (value == 0)
            {
                HealthText.SetText("");
            }
        }
    }
  #endregion



  void Start()
  {
    CurrentGold = Convert.ToInt32(GoldVault.text);
    HealthText.text = "";

    RollingCounter = 0;
  }
    internal void TransferGold()
    {
        if (LiczbaPrzelewowGolda > 0)
            {
                // DODAWANIE GOLDA
                CurrentGold++;
                GoldVault.text = CurrentGold.ToString();
                LiczbaPrzelewowGolda--;
                GodsManager_Script.CollorSkillButtonsIfCanBeUsed();
            }
            else if (LiczbaPrzelewowGolda < 0)
            {
                // ODEJMOWANIE GOLDA
                CurrentGold--;
                GoldVault.text = CurrentGold.ToString();
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
                    var p1coin = GameObject.Find("CoinTextPlayer1").GetComponent<TextMeshProUGUI>();
                    p1coin.SetText("");
                }
            }
    }

    internal void TransferDamage()
    {
        
            ActualHPValue = Convert.ToInt32(HealthText.text);

            // reset czasu do 0 i naliczanie dalej os początku
           
            if (liczbaPrzelewaniaObrazen > 0)
            {
                // DAMAGING
                int Currenthp = ActualHPValue;
                int p1NewHpValue = Currenthp - 1;
                HealthText.text = (p1NewHpValue.ToString());

                liczbaPrzelewaniaObrazen--;
            }
            else if (liczbaPrzelewaniaObrazen < 0)
            {
                // HEALING
                int Currenthp = ActualHPValue;
                int p1NewHpValue = Currenthp + 1;
                HealthText.text = (p1NewHpValue.ToString());

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
}
