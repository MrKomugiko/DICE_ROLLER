using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New God", menuName = "Scriptable Object/God")]
public class God : ScriptableObject
{
    public bool IsGodPlayable = false;
    public int Index;
    public Sprite MainImage;
    public Sprite WorkInProgressImage;
    public Sprite CardReverseImage;
    public string Name;
    public string TotemFullName;
    public string Description;
    public string SkillDescriptionTemplate;

    public int LevelCost, Leve2Cost, Leve3Cost;
    public int Level1SkillValue, Level2SkillValue, Level3SkillValue;

    public Dictionary<int, int> DictOfSkillsWithValues()
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
