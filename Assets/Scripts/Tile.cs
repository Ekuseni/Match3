using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu]
public class Tile : ScriptableObject
{
    public Sprite sprite;
    public int points;
    public Color color;
}
