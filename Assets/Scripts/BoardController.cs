using DG.Tweening;
using RoboRyanTron.Unite2017.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    private Board board;
    public GameObject tile;

    //public TileController[,] tiles;


    // Start is called before the first frame update
    void Start()
    {
        Board.Tiles = new TileController[board.Size.x, board.Size.y];


        do
        {
            for (int x = 0; x < board.Size.x; x++)
            {
                for (int y = 0; y < board.Size.y; y++)
                {
                    Board.Tiles[x, y] = Instantiate(tile, transform).GetComponent<TileController>();
                    Board.Tiles[x, y].Initialize(x, y);


                    List<Tile> availableTiles = new List<Tile>(board.TileSet);

                    if (x - 1 >= 0 && x - 2 >= 0 && Board.Tiles[x - 1, y].Tile == Board.Tiles[x - 2, y].Tile)
                    {
                        availableTiles.Remove(Board.Tiles[x - 1, y].Tile);
                    }

                    if (y - 1 >= 0 && y - 2 >= 0 && Board.Tiles[x, y - 1].Tile == Board.Tiles[x, y - 2].Tile)
                    {
                        availableTiles.Remove(Board.Tiles[x, y - 1].Tile);
                    }

                    Board.Tiles[x, y].Tile = availableTiles[Random.Range(0, availableTiles.Count)];
                }
            }
        } while (!CheckForValidMove());

    }

    

    bool CheckForValidMove()
    {
        for (int x = 0; x < board.Size.x; x++)
        {
            for (int y = 0; y < board.Size.y; y++)
            {
                TileController match1, match2, match3;

                if (board.TryGetTile(x - 1, y, out match1) && board.TryGetTile(x - 2, y, out match2))
                {
                    if (match1.Tile == match2.Tile && board.TryGetTile(x + 1, y, out match3) && match3.Tile == match2.Tile)
                    {
                        return true;
                    }

                    if (match1.Tile == match2.Tile && board.TryGetTile(x, y + 1, out match3) && match3.Tile == match2.Tile)
                    {
                        return true;
                    }

                    if (match1.Tile == match2.Tile && board.TryGetTile(x, y - 1, out match3) && match3.Tile == match2.Tile)
                    {
                        return true;
                    }
                }

                if (board.TryGetTile(x, y - 1, out match1) && board.TryGetTile(x, y - 2, out match2))
                {
                    if (match1.Tile == match2.Tile && board.TryGetTile(x, y + 1, out match3) && match3.Tile == match2.Tile)
                    {
                        return true;
                    }

                    if (match1.Tile == match2.Tile && board.TryGetTile(x + 1, y, out match3) && match3.Tile == match2.Tile)
                    {
                        return true;
                    }

                    if (match1.Tile == match2.Tile && board.TryGetTile(x - 1, y, out match3) && match3.Tile == match2.Tile)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void ResetBoard()
    {
        StartCoroutine(ReplaceTiles());
    }

    private IEnumerator ReplaceTiles()
    {
        Sequence sequence = DOTween.Sequence();

        foreach (TileController tileController in Board.Tiles)
        {
            sequence.Join(tileController.HideTile());
        }

        yield return sequence.WaitForCompletion();

        do
        {
            for (int x = 0; x < board.Size.x; x++)
            {
                for (int y = 0; y < board.Size.y; y++)
                {
                    List<Tile> availableTiles = new List<Tile>(board.TileSet);

                    if (x - 1 >= 0 && x - 2 >= 0 && Board.Tiles[x - 1, y].Tile == Board.Tiles[x - 2, y].Tile)
                    {
                        availableTiles.Remove(Board.Tiles[x - 1, y].Tile);
                    }

                    if (y - 1 >= 0 && y - 2 >= 0 && Board.Tiles[x, y - 1].Tile == Board.Tiles[x, y - 2].Tile)
                    {
                        availableTiles.Remove(Board.Tiles[x, y - 1].Tile);
                    }

                    Board.Tiles[x, y].Tile = availableTiles[Random.Range(0, availableTiles.Count)];
                }
            }
        } while (!CheckForValidMove());


        foreach (TileController tileController in Board.Tiles)
        {
            sequence.Join(tileController.ShowTile());
        }
    }

    public void SpawnNew()
    {
        StartCoroutine(SpawnNew(true));
    }

    private IEnumerator SpawnNew(bool foundEmpty)
    {
       

        if (!foundEmpty)
        {
            if(!CheckCombo())
            {
                

                if (!CheckForValidMove())
                {
                    ResetBoard();
                }
            }
            else
            {
                yield return Board.CollectSequence.WaitForCompletion();
            }
        }
        

        bool foundNext = false;

        Board.MoveTileSequence = DOTween.Sequence();

        for (int x = 0; x < board.Size.x; x++)
        {
            for (int y = 0; y < board.Size.y; y++)
            {
                TileController tileController;

                if (Board.Tiles[x, y].IsCollcted && board.TryGetTile(x, y + 1, out tileController))
                {
                    TileController.SwapWithAnim(Board.Tiles[x, y],tileController);
                    foundNext = true;
                }
                else if(Board.Tiles[x, y].IsCollcted)
                {   
                    Board.Tiles[x, y].AddNew(board.TileSet[Random.Range(0, board.TileSet.Length)]);
                }
            }
        }

        yield return Board.MoveTileSequence.WaitForCompletion();

        StartCoroutine(SpawnNew(foundNext));
    }

    bool CheckCombo()
    {
        bool foundCombo = false;

        for (int x = 0; x < board.Size.x; x++)
        {
            for (int y = 0; y < board.Size.y; y++)
            {
                List<TileController> tileControllers = new List<TileController>();

                Board.Tiles[x, y].CheckMatch(Vector2Int.right, Board.Tiles[x, y].Tile, ref tileControllers);

                if(tileControllers.Count >= 2)
                {
                    foreach(TileController tileController in tileControllers)
                    {
                        tileController.IsCombo = true;
                    }

                    Board.Tiles[x, y].IsCombo = true;

                    foundCombo = true;
                }

                tileControllers.Clear();

                Board.Tiles[x, y].CheckMatch(Vector2Int.up, Board.Tiles[x, y].Tile, ref tileControllers);

                if (tileControllers.Count >= 2)
                {
                    foreach (TileController tileController in tileControllers)
                    {
                        tileController.IsCombo = true;
                    }

                    Board.Tiles[x, y].IsCombo = true;

                    foundCombo = true;
                }
            }
        }

        foreach(TileController tileController in Board.Tiles)
        {
            if(tileController.IsCombo && !tileController.IsCollcted)
            {
                tileController.Collect();
            }
        }

        return foundCombo;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
