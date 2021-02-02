using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoardVariable : ScriptableObject
{
    public Vector2Int BoardSize;

    public static Vector2Int Size;



    private void OnEnable()
    {
        Size = BoardSize;
        TileSet = new TileVariable[tileSet.Length];
        tileSet.CopyTo(TileSet, 0);
        Specials = new TileVariable[specials.Length];
        specials.CopyTo(Specials, 0);
    }

    public TileVariable[] tileSet;
    public TileVariable[] specials;
    private static TileVariable[] Specials;
    private static TileVariable[] TileSet;
    public GameObject TilePrefab;

    public static TileController[,] TileControllers;


    private static Sequence moveTileSequence;
    public static Sequence MoveTileSequence;
    //{
    //    get
    //    {
    //        if (!moveTileSequence.IsActive())
    //        {
    //            moveTileSequence = DOTween.Sequence();
    //        }



    //        return moveTileSequence.OnKill(() => moveTileSequence = null);
    //    }
    //}

    private static Sequence remomoveTileSequence;
    public static Sequence CollectSequence;
    //{
    //    get
    //    {
    //        if (!remomoveTileSequence.IsActive())
    //        {
    //            remomoveTileSequence = DOTween.Sequence();
    //        }

    //        return remomoveTileSequence.OnKill(() => remomoveTileSequence = null);
    //    }
    //}

    public void InitialzeBoard(MonoBehaviour caller)
    {
        TileControllers = new TileController[Size.x, Size.y];

        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                TileControllers[x, y] = Instantiate(TilePrefab, caller.transform).GetComponent<TileController>();
                TileControllers[x, y].Initialize(x, y);
            }
        }

        SetTiles();
    }


    private void SetTiles()
    {
        do
        {
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    List<TileVariable> availableTiles = new List<TileVariable>(TileSet);

                    if (x - 1 >= 0 && x - 2 >= 0 && TileControllers[x - 1, y].TileVariable == TileControllers[x - 2, y].TileVariable)
                    {
                        availableTiles.Remove(TileControllers[x - 1, y].TileVariable);
                    }

                    if (y - 1 >= 0 && y - 2 >= 0 && TileControllers[x, y - 1].TileVariable == TileControllers[x, y - 2].TileVariable)
                    {
                        availableTiles.Remove(TileControllers[x, y - 1].TileVariable);
                    }

                    TileControllers[x, y].TileVariable = availableTiles[Random.Range(0, availableTiles.Count)];
                }
            }
        } while (!CheckForValidMove());
    }

    private bool CheckForValidMove()
    {
        TileController[] match = new TileController[3];

        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {


                if (TryGetTile(x - 1, y, out match[0]) && TryGetTile(x - 2, y, out match[1]))
                {
                    if (match[0].TileVariable == match[1].TileVariable && TryGetTile(x + 1, y, out match[2]) && match[2].TileVariable == match[1].TileVariable)
                    {
                        return true;
                    }

                    if (match[0].TileVariable == match[1].TileVariable && TryGetTile(x, y + 1, out match[2]) && match[2].TileVariable == match[1].TileVariable)
                    {
                        return true;
                    }

                    if (match[0].TileVariable == match[1].TileVariable && TryGetTile(x, y - 1, out match[2]) && match[2].TileVariable == match[1].TileVariable)
                    {
                        return true;
                    }
                }

                if (TryGetTile(x, y - 1, out match[0]) && TryGetTile(x, y - 2, out match[1]))
                {
                    if (match[0].TileVariable == match[1].TileVariable && TryGetTile(x, y + 1, out match[2]) && match[2].TileVariable == match[1].TileVariable)
                    {
                        return true;
                    }

                    if (match[0].TileVariable == match[1].TileVariable && TryGetTile(x + 1, y, out match[2]) && match[2].TileVariable == match[1].TileVariable)
                    {
                        return true;
                    }

                    if (match[0].TileVariable == match[1].TileVariable && TryGetTile(x - 1, y, out match[2]) && match[2].TileVariable == match[1].TileVariable)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static TileController TryGetTile(Vector2Int index)
    {
        return TryGetTile(index.x, index.y);
    }

    public static TileController TryGetTile(int x, int y)
    {
        TileController tileController;

        TryGetTile(x, y, out tileController);

        return tileController;
    }

    public static bool TryGetTile(Vector2Int index, out TileController tileController)
    {
        return TryGetTile(index.x, index.y, out tileController);
    }

    public static bool TryGetTile(int x, int y, out TileController tileController)
    {
        if (x >= 0 && y >= 0 && x < Size.x && y < Size.y)
        {
            tileController = TileControllers[x, y];

            return true;
        }
        else
        {
            tileController = null;
            return false;
        }
    }

    public static bool IsValidSwap(TileController a, TileController b)
    {
        List<TileController> matchStreak = new List<TileController>();


        Vector2Int direction = b.arrayIndex - a.arrayIndex;

        if (direction.x >= 0) CheckMatch(a, Vector2Int.left, b.TileVariable, ref matchStreak);
        if (direction.x <= 0) CheckMatch(a, Vector2Int.right, b.TileVariable, ref matchStreak);

        if (matchStreak.Count >= 2) return true;

        matchStreak.Clear();

        if (direction.x <= 0) CheckMatch(b, Vector2Int.left, a.TileVariable, ref matchStreak);
        if (direction.x >= 0) CheckMatch(b, Vector2Int.right, a.TileVariable, ref matchStreak);

        if (matchStreak.Count >= 2) return true;

        matchStreak.Clear();

        if (direction.y >= 0) CheckMatch(a, Vector2Int.down, b.TileVariable, ref matchStreak);
        if (direction.y <= 0) CheckMatch(a, Vector2Int.up, b.TileVariable, ref matchStreak);

        if (matchStreak.Count >= 2) return true;

        matchStreak.Clear();

        if (direction.y <= 0) CheckMatch(b, Vector2Int.down, a.TileVariable, ref matchStreak);
        if (direction.y >= 0) CheckMatch(b, Vector2Int.up, a.TileVariable, ref matchStreak);

        if (matchStreak.Count >= 2) return true;

        return false;
    }

    public static void CheckMatch(TileController tileToCheckFor, Vector2Int direction, TileVariable match, ref List<TileController> matchList)
    {
        TileController tileController;

        if (TryGetTile(tileToCheckFor.arrayIndex + direction, out tileController) && tileController.TileVariable == match)
        {
            matchList.Add(tileController);
            CheckMatch(tileController, direction, match, ref matchList);
        }
    }

    public static void Swap(TileController a, TileController b)
    {
        TileController swapTileController = a;
        TileControllers[a.arrayIndex.x, a.arrayIndex.y] = b;
        TileControllers[b.arrayIndex.x, b.arrayIndex.y] = swapTileController;

        Vector2Int swapArrayIndex = a.arrayIndex;
        a.arrayIndex = b.arrayIndex;
        b.arrayIndex = swapArrayIndex;
    }

    public static void MarkForRemoval()
    {
        bool foundTileToRemove = false;
        List<TileController> tileControllers = new List<TileController>();

        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                //if (TileControllers[x, y].MarkedForRemoval) continue;

                int totalRemoved = 0;

                tileControllers.Clear();


                CheckMatch(TileControllers[x, y], Vector2Int.right, TileControllers[x, y].TileVariable, ref tileControllers);


                if (tileControllers.Count >= 2)
                {
                    foreach (TileController tileController in tileControllers)
                    {
                        tileController.MarkedForRemoval = true;
                    }

                    TileControllers[x, y].MarkedForRemoval = true;

                    foundTileToRemove = true;

                    totalRemoved += tileControllers.Count;
                }

                tileControllers.Clear();


                CheckMatch(TileControllers[x, y], Vector2Int.up, TileControllers[x, y].TileVariable, ref tileControllers);


                if (tileControllers.Count >= 2)
                {
                    foreach (TileController tileController in tileControllers)
                    {
                        tileController.MarkedForRemoval = true;
                    }

                    TileControllers[x, y].MarkedForRemoval = true;

                    foundTileToRemove = true;

                    totalRemoved += tileControllers.Count;
                }

                if(totalRemoved - 3 >= 0)
                {
                    TileControllers[x, y].MarkedForRemoval = false;
                    TileControllers[x, y].TileVariable = Specials[Mathf.Clamp(totalRemoved - 3, 0, Specials.Length - 1)];
                }
            }
        }

        if (foundTileToRemove)
        {
            RemoveTiles();
        }

    }

    public static void RemoveTiles()
    {
        CollectSequence = DOTween.Sequence();

        foreach (TileController tileController in TileControllers)
        {
            if (tileController.MarkedForRemoval && !tileController.IsCollcted) tileController.Collect(CollectSequence);
        }


        CollectSequence.AppendCallback(() => SpawnNew());
    }

    public static Vector2Int replaceFrom = Vector2Int.down;

    public static void SpawnNew()
    {
        bool foundEmpty = false;

        MoveTileSequence = DOTween.Sequence();



        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                TileController tileController;

                int indexX = replaceFrom.x > 0 ? x : Size.x - 1 - x;
                int indexY = replaceFrom.y > 0 ? y : Size.y - 1 - y;

                if (TileControllers[indexX, indexY].IsCollcted && TryGetTile(TileControllers[indexX, indexY].arrayIndex + replaceFrom, out tileController))
                {
                    TileController.SwapWithAnim(TileControllers[indexX, indexY], tileController);
                    foundEmpty = true;
                }
                else if (TileControllers[indexX, indexY].IsCollcted)
                {
                    TileControllers[indexX, indexY].AddNew(TileSet[Random.Range(0, TileSet.Length)]);
                    foundEmpty = true;
                }
            }
        }

        if (foundEmpty)
        {

            MoveTileSequence.OnComplete(() => SpawnNew());
        }
        else
        {
            MarkForRemoval();
        }
    }
}

