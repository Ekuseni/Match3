using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class BombTileCollectBehaviour : CollectBehaviour
{

    static Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.up + Vector2Int.right,
            Vector2Int.right,
            Vector2Int.right + Vector2Int.down,
            Vector2Int.down,
            Vector2Int.down + Vector2Int.left,
            Vector2Int.left,
            Vector2Int.left + Vector2Int.up
        };

    public override void Collect(TileController tileController, Sequence collectSequence, int score)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(tileController.tileImage.rectTransform.DOScale(Vector2.one * 3, 0.1f));
        sequence.Append(tileController.tileImage.rectTransform.DOScale(Vector2.zero, 0.1f));

        foreach (Vector2Int direction in directions)
        {
            TileController neighbour;

            if (BoardVariable.TryGetTile(tileController.arrayIndex + direction, out neighbour))
            {
                if (!neighbour.IsCollcted) neighbour.Collect(sequence);
            }


            //neighbour?.TileVariable.collectBehaviour.Collect(neighbour, sequence, neighbour.TileVariable.points);
        }

        collectSequence.Append(sequence);
    }
}
