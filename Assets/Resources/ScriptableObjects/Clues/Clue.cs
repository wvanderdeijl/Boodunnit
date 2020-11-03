using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Clue", menuName = "ScriptableObjects/Clue")]
public class Clue : ScriptableObject
{
    public string Name;
    public string Details;
    public string Location;
    public string Description;
    public Sprite Image;
}
