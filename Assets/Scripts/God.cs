using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName="New God",menuName="Scriptable Object/God")]
public class God : ScriptableObject
{
    public int Index;
    public Sprite Image;
    public string Name;
    public string TotemFullName;
    public string Description;
    public string SkillType;

    public int LevelCost;
    public int Leve2Cost;
    public int Leve3Cost;

    public string Level1SkillValue;
    public string Level2SkillValue;
    public string Level3SkillValue;

}
