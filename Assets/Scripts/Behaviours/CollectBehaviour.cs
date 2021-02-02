using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectBehaviour : ScriptableObject
{
    public IntVariable Score;

    public abstract void Collect(TileController tileController, Sequence collectsequence, int score);
}
