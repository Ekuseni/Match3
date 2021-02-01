using DG.Tweening;
using RoboRyanTron.Unite2017.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TileController : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Board board;
    [SerializeField]
    private IntVariable points;

    private Tile tile;

    public bool IsCombo = false;
    public Tile Tile
    {
        set
        {
            tile = value;

            if(tile != null)
            {
                tileImage.sprite = value.sprite;
                tileImage.color = value.color;
            }
            
        }

        get
        {
            return tile;
        }
    }

    public bool IsCollcted
    {
        get
        {
            return tileImage.rectTransform.localScale == Vector3.zero;
        }
    }

    private RectTransform parent;
    private RectTransform rectTransform;
    private Vector2 pointInTile;
    private Vector2 startPos
    {
        get
        {
            return new Vector2(rectTransform.rect.width * arrayIndex.x + rectTransform.rect.width / 2f, rectTransform.rect.height * arrayIndex.y + rectTransform.rect.height / 2f);
        }
    }

    private Vector2 displacement
    {
        get
        {
            return board.Displacement.Value;
        }

        set
        {
            board.Displacement.Value = value;
        }
    }


    private Vector2Int arrayIndex;
    private TileController tileToDrive;
    private Image tileImage;

    public void Initialize(int x, int y)
    {
        parent = transform.parent.gameObject.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        tileImage = GetComponentInChildren<Image>();
        tileImage.useSpriteMesh = true;

        rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width * x + rectTransform.rect.width / 2f, rectTransform.rect.height * y + rectTransform.rect.height / 2f);

        arrayIndex = new Vector2Int(x, y);
    }

    

    public void OnDrag(PointerEventData eventData)
    {
        if(tile == board.Bomb || tile == board.ColorMagnet)
        {
            return;
        }

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, eventData.position, null, out position);
        displacement = Vector2.ClampMagnitude((position - pointInTile) - startPos, rectTransform.rect.width);

        displacement = ((displacement.magnitude < rectTransform.rect.width / 2) ? displacement : displacement * (Mathf.Abs(displacement.x) < Mathf.Abs(displacement.y) ? Vector2.up : Vector2.right));

        SetClampedPosition(startPos + displacement);

        if (displacement.magnitude > rectTransform.rect.width / 2f)
        {
            if (tileToDrive != GetTileToDrive(displacement))
            {
                tileToDrive?.ReturnToStartPos();
                tileToDrive = GetTileToDrive(displacement);
            }

            tileToDrive?.DriveTile(displacement);
        }
        else
        {
            tileToDrive?.ReturnToStartPos();
            tileToDrive = null;
        }
    }
    private TileController GetTileToDrive(Vector2 displacement)
    {
        Vector2Int direction = new Vector2Int((int)displacement.normalized.x, (int)displacement.normalized.y);

        Vector2Int index = arrayIndex + direction;

        if (index.x >= 0 && index.y >= 0 && index.x < board.Size.x && index.y < board.Size.y)
        {
            return Board.Tiles[index.x, index.y];
        }
        else
        {
            return null;
        }
    }
    private void DriveTile(Vector2 displacement)
    {
        rectTransform.anchoredPosition = startPos - displacement;
    }
    private void MoveTo(Vector2 postion, TweenCallback tweenCallback = null)
    {
        if (Board.MoveTileSequence == null)
        {
            Board.MoveTileSequence = DOTween.Sequence();
        }

        Board.MoveTileSequence.Join(rectTransform.DOAnchorPos(startPos, 0.1f).OnComplete(() => tweenCallback?.Invoke()));
    }
    private void ReturnToStartPos(TweenCallback tweenCallback = null)
    {
        MoveTo(startPos, tweenCallback);
    }
    private void SetClampedPosition(Vector2 position)
    {
        rectTransform.anchoredPosition = new Vector2(Mathf.Clamp(position.x, rectTransform.rect.width / 2f, parent.rect.width - rectTransform.rect.width / 2f), Mathf.Clamp(position.y, rectTransform.rect.height / 2f, parent.rect.height - rectTransform.rect.height / 2f));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1.25f;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, null, out pointInTile);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;

        if (displacement.magnitude < rectTransform.rect.width * 0.5)
        {
            ReturnToStartPos();
        }
        else
        {
            if (board.IsValidMove(this, tileToDrive))
            {
                SwapWithAnim(this, tileToDrive);

                Board.MoveTileSequence.AppendCallback(() => ApplyMoveAfterAnim());
            }
            else
            {
                ReturnToStartPos();
                tileToDrive.ReturnToStartPos();
            }
        }
    }

    public Tween HideTile()
    {
        return tileImage.rectTransform.DOScale(Vector2.zero, 0.1f).OnComplete(() => Tile = null);
    }

    public Tween ShowTile()
    {
        return tileImage.rectTransform.DOScale(Vector2.one, 0.1f);
    }

    void ApplyMoveAfterAnim()
    {
        if (Board.Matches.horizontalMatchA.Count >= 2 || Board.Matches.verticalMatchA.Count >= 2)
        {
            if(Board.Matches.horizontalMatchA.Count == 3 || Board.Matches.verticalMatchA.Count == 3)
            {
                tileToDrive.Tile = board.Bomb;
            }
            else if(Board.Matches.horizontalMatchA.Count > 3 || Board.Matches.verticalMatchA.Count > 3)
            {
                board.ColorMagnet.color = tileToDrive.tileImage.color;

                tileToDrive.Tile = board.ColorMagnet;
            }
            else
            {
                tileToDrive.Collect();
            }
            

            if (Board.Matches.horizontalMatchA.Count >= 2)
            {
                foreach (TileController tile in Board.Matches.horizontalMatchA)
                {
                    tile.Collect();
                }
            }

            if (Board.Matches.verticalMatchA.Count >= 2)
            {
                foreach (TileController tile in Board.Matches.verticalMatchA)
                {
                    tile.Collect();
                }
            }
        }

        if (Board.Matches.horizontalMatchB.Count >= 2 || Board.Matches.verticalMatchB.Count >= 2)
        {
            if (Board.Matches.horizontalMatchB.Count == 3 || Board.Matches.verticalMatchB.Count == 3)
            {
                Tile = board.Bomb;
            }
            else if (Board.Matches.horizontalMatchB.Count > 3 || Board.Matches.verticalMatchB.Count > 3)
            {
                board.ColorMagnet.color = tileImage.color;

                Tile = board.ColorMagnet;
            }
            else
            {
                Collect();
            }

            if (Board.Matches.horizontalMatchB.Count >= 2)
            {
                foreach (TileController tile in Board.Matches.horizontalMatchB)
                {
                    tile.Collect();
                }
            }

            if (Board.Matches.verticalMatchB.Count >= 2)
            {
                foreach (TileController tile in Board.Matches.verticalMatchB)
                {
                    tile.Collect();
                }
            }
        }
    }
    public void Collect()
    {
        IsCombo = false;
        Debug.Log("Collecting: " + tile.points);
        points.Value += tile.points;

        if (Board.CollectSequence == null)
        {
            Board.CollectSequence = DOTween.Sequence();
            Board.CollectSequence.AppendCallback(() => board.ValidMovePerformed.Raise());
        }

        Board.CollectSequence.Append(tileImage.rectTransform.DOScale(Vector2.zero, 0.1f));
        
    }
    public static void SwapWithAnim(TileController a, TileController b)
    {
        Swap(a, b);
        if (!a.IsCollcted) a.ReturnToStartPos();
        if (!b.IsCollcted) b.ReturnToStartPos();
    }
    public void AddNew(Tile tile)
    {
        rectTransform.anchoredPosition = startPos;
        tileImage.rectTransform.anchoredPosition = new Vector2(0f, rectTransform.rect.height);
        tileImage.rectTransform.localScale = Vector2.one;
        Tile = tile;
        tileImage.rectTransform.DOAnchorPos(Vector2.zero, 0.1f);
    }
    private static void Swap(TileController a, TileController b)
    {
        TileController swapTileController = a;
        Board.Tiles[a.arrayIndex.x, a.arrayIndex.y] = b;
        Board.Tiles[b.arrayIndex.x, b.arrayIndex.y] = swapTileController;

        Vector2Int swapArrayIndex = a.arrayIndex;
        a.arrayIndex = b.arrayIndex;
        b.arrayIndex = swapArrayIndex;
    }
    public void CheckMatch(Vector2Int direction, Tile match, ref List<TileController> matchList)
    {
        TileController tileController;

        if (board.TryGetTile(arrayIndex + direction, out tileController) && tileController.Tile == match)
        {
            matchList.Add(tileController);
            tileController.CheckMatch(direction, match, ref matchList);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(tile == board.Bomb)
        {
            TileController tileController;

            board.TryGetTile(arrayIndex + Vector2Int.up, out tileController);
            tileController?.Collect();
            board.TryGetTile(arrayIndex + Vector2Int.right + Vector2Int.up, out tileController);
            tileController?.Collect();
            board.TryGetTile(arrayIndex + Vector2Int.right, out tileController);
            tileController?.Collect();
            board.TryGetTile(arrayIndex + Vector2Int.right + Vector2Int.down, out tileController);
            tileController?.Collect();
            board.TryGetTile(arrayIndex + Vector2Int.down, out tileController);
            tileController?.Collect();
            board.TryGetTile(arrayIndex + Vector2Int.left + Vector2Int.down, out tileController);
            tileController?.Collect();
            board.TryGetTile(arrayIndex + Vector2Int.left, out tileController);
            tileController?.Collect();
            board.TryGetTile(arrayIndex + Vector2Int.left + Vector2Int.up, out tileController);
            tileController?.Collect();

            Collect();
        }

        if(tile == board.ColorMagnet)
        {
            foreach(TileController tile in Board.Tiles)
            {
                if(tile.tileImage.color == tileImage.color)
                {
                    tile.Collect();
                }
            }

            Collect();
        }
    }
}
