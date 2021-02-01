using RoboRyanTron.Unite2017.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class IntVariable : ScriptableObject
{
    private int value;

    public GameEvent ValueChanged;

    public int Value
    {
        set
        {
            ValueChanged?.Raise();
            this.value = value;
        }

        get
        {
            return value;
        }
    }

    
}
