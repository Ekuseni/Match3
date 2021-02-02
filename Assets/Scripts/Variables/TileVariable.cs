using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu]
public class TileVariable : ScriptableObject
{
    public Sprite sprite;
    public int points;
    public Color color;
    public bool dragable;
    public bool clickable;

    public bool dontChangeColor;

    public CollectBehaviour collectBehaviour;
}
