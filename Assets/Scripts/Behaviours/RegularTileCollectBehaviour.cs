using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RegularTileCollectBehaviour : CollectBehaviour
{
    public override void Collect(TileController tileController, Sequence collectsequence, int score)
    {
        collectsequence.Append(tileController.tileImage.rectTransform.DOScale(Vector2.zero, 0.1f).OnComplete(() => Score.Value += score));
        
    }
}
