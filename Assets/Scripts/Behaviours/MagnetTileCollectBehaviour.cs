using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class MagnetTileCollectBehaviour : CollectBehaviour
{
    public override void Collect(TileController tileController, Sequence collectSequence, int score)
    {
        collectSequence.Append(tileController.tileImage.rectTransform.DOScale(Vector2.zero, 0.1f));

        foreach (TileController controller in BoardVariable.TileControllers)
        {
            if (controller.tileImage.color == tileController.tileImage.color && controller.TileVariable != tileController.TileVariable) controller.Collect(collectSequence);
        }

       
    }

   
}
