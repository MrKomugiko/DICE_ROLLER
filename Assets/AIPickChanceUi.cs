using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIPickChanceUi : MonoBehaviour
{
    [SerializeField] List<Image> ListOfPickChance_Image;
    [SerializeField] List<Text> ListOfPickChance_Text;

    [SerializeField] private bool needUpdateColorsUI = true;

    void Start()
    {
        ListOfPickChance_Image.AddRange(this.GetComponentsInChildren<Image>().ToList().Where(i => i.name.Contains("Dice")));
        ListOfPickChance_Text.AddRange(this.GetComponentsInChildren<Text>().ToList());
    }

    void Update()
    {
        UpdateColorsDeppendsOfPickChanceValue();
    }

    void UpdateColorsDeppendsOfPickChanceValue()
    {
        if(!needUpdateColorsUI) return;
        
        needUpdateColorsUI = false;
        for (int i = 0; i < 6; i++)
        {
            ListOfPickChance_Image[i].color = Color.HSVToRGB(Convert.ToInt32(ListOfPickChance_Text[i].text)/100f*0.36f, 1f, 1f);
        }
    }
    public void SetPickChanceValues(int diceNumber, int dicePickChance)
    {
        int diceIndex = diceNumber - 1;
        if(ListOfPickChance_Text[diceIndex].text == dicePickChance.ToString()) return;

        ListOfPickChance_Text[diceIndex].text = dicePickChance.ToString();
        print($"kostka nr: {diceNumber} Change value from {ListOfPickChance_Text[diceIndex].text} to {dicePickChance}");

        needUpdateColorsUI=true;
    }
}

