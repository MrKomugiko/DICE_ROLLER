﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New God", menuName = "Scriptable Object/God")]
public class God : ScriptableObject
{
    public int Index;
    public Sprite Image;
    public string Name;
    public string TotemFullName;
    public string Description;
    public string SkillDescriptionTemplate;

    public int LevelCost;
    public int Leve2Cost;
    public int Leve3Cost;

    public int Level1SkillValue;
    public int Level2SkillValue;
    public int Level3SkillValue;

    private Dictionary<int, int> DictOfSkillsWithValues()
    {
        Dictionary<int, int> skillsWithValues = new Dictionary<int, int>();

        skillsWithValues.Add(this.LevelCost, this.Level1SkillValue);
        skillsWithValues.Add(this.Leve2Cost, this.Level2SkillValue);
        skillsWithValues.Add(this.Leve3Cost, this.Level3SkillValue);

        return skillsWithValues;
    }

    public List<string> GenerateListOFSkillsDescription()
    {
        List<string> listOfSkillsDescription = new List<string>();

        foreach(KeyValuePair<int,int> skill in DictOfSkillsWithValues()){
            string skilDescription = SkillDescriptionTemplate;
            skilDescription = skilDescription.Replace("X", skill.Value.ToString());
            skilDescription = skilDescription.Replace("Y", skill.Key.ToString());

            listOfSkillsDescription.Add(skilDescription);
        }

        return listOfSkillsDescription;
    }
}
