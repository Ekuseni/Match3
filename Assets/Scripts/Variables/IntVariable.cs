using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class IntVariable : ScriptableObject
{
    public int value;

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
