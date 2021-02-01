using DG.Tweening;
using RoboRyanTron.Unite2017.Events;
using RoboRyanTron.Unite2017.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class Board : ScriptableObject
{
    public Vector2Int Size;
    public static TileController[,] Tiles;
    public Tile[] TileSet;

    public Tile Bomb;
    public Tile ColorMagnet;

    public Vector2Variable Displacement;

    public GameEvent ValidMovePerformed;

    public static Sequence CollectSequence;
    public static Sequence MoveTileSequence;

    public bool TryGetTile(Vector2Int index, out TileController tileController)
    {
        if (index.x >= 0 && index.y >= 0 && index.x < Size.x && index.y < Size.y)
        {
            tileController = Tiles[index.x, index.y];

            return true;
        }
        else
        {
            tileController = null;
            return false;
        }
    }

    public bool TryGetTile(int x, int y, out TileController tileController)
    {
        if (x >= 0 && y >= 0 && x < Size.x && y < Size.y)
        {
            tileController = Tiles[x, y];

            return true;
        }
        else
        {
            tileController = null;
            return false;
        }
    }

    private void OnEnable()
    {
        Matches.horizontalMatchA = new List<TileController>();
        Matches.horizontalMatchB = new List<TileController>();
        Matches.verticalMatchA = new List<TileController>();
        Matches.verticalMatchB = new List<TileController>();
    }

    public static class Matches
    {
        public static List<TileController> horizontalMatchA;
        public static List<TileController> horizontalMatchB;
        public static List<TileController> verticalMatchA;
        public static List<TileController> verticalMatchB;
    }

    public bool IsValidMove(TileController a, TileController b)
    {
        Matches.horizontalMatchA.Clear();

        if (Displacement.Value.x >= 0) a.CheckMatch(Vector2Int.left, b.Tile, ref Matches.horizontalMatchA);
        if (Displacement.Value.x <= 0) a.CheckMatch(Vector2Int.right, b.Tile, ref Board.Matches.horizontalMatchA);

        Board.Matches.horizontalMatchB.Clear();

        if (Displacement.Value.x <= 0) b.CheckMatch(Vector2Int.left, a.Tile, ref Matches.horizontalMatchB);
        if (Displacement.Value.x >= 0) b.CheckMatch(Vector2Int.right, a.Tile, ref Matches.horizontalMatchB);

        Matches.verticalMatchA.Clear();

        if (Displacement.Value.y >= 0) a.CheckMatch(Vector2Int.down, b.Tile, ref Matches.verticalMatchA);
        if (Displacement.Value.y <= 0) a.CheckMatch(Vector2Int.up, b.Tile, ref Matches.verticalMatchA);

        Matches.verticalMatchB.Clear();

        if (Displacement.Value.y <= 0) b.CheckMatch(Vector2Int.down, a.Tile, ref Matches.verticalMatchB);
        if (Displacement.Value.y >= 0) b.CheckMatch(Vector2Int.up, a.Tile, ref Matches.verticalMatchB);

        return Matches.horizontalMatchA.Count >= 2 ||
                Matches.horizontalMatchB.Count >= 2 ||
                Matches.verticalMatchA.Count >= 2 ||
                Matches.verticalMatchB.Count >= 2;
    }
}
