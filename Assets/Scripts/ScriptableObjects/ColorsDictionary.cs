using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/Colors Dictionary")]
public class ColorsDictionary : ScriptableObject
{
    public List<Color> colors;
}
