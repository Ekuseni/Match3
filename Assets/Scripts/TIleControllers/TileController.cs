using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TileController : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    //[SerializeField]
    //private Board board;
    //[SerializeField]
    //private IntVariable points;

    [SerializeField]
    private TileVariable tileVariable;

    //public bool IsCombo = false;
    public TileVariable TileVariable
    {
        set
        {
            tileVariable = value;

            if (tileVariable != null)
            {
                tileImage.sprite = value.sprite;
                if(!value.dontChangeColor) tileImage.color = value.color;
            }

        }

        get
        {
            return tileVariable;
        }
    }

    public bool IsCollcted = false;
    

    public bool MarkedForRemoval = false;

    private RectTransform parent;
    private RectTransform rectTransform;

    public Vector2Int arrayIndex;
    

    private TileController tileToDrive;
    public Image tileImage;

    public void Initialize(int x, int y)
    {
        parent = transform.parent.gameObject.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        tileImage = GetComponentInChildren<Image>();
        tileImage.useSpriteMesh = true;
        tileImage.preserveAspect = true;

        rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width * x + rectTransform.rect.width / 2f, rectTransform.rect.height * y + rectTransform.rect.height / 2f);
        arrayIndex = new Vector2Int(x, y);
    }

    private Vector2 pointInTile;
    private Vector2 startPos
    {
        get
        {
            return new Vector2(rectTransform.rect.width * arrayIndex.x + rectTransform.rect.width / 2f, rectTransform.rect.height * arrayIndex.y + rectTransform.rect.height / 2f);
        }
    }
    private Vector2 displacement;

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, Camera.main, out pointInTile);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!TileVariable.dragable) return;

            Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, eventData.position, Camera.main, out position);
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

    private void SetClampedPosition(Vector2 position)
    {
        rectTransform.anchoredPosition = new Vector2(Mathf.Clamp(position.x, rectTransform.rect.width / 2f, parent.rect.width - rectTransform.rect.width / 2f), Mathf.Clamp(position.y, rectTransform.rect.height / 2f, parent.rect.height - rectTransform.rect.height / 2f));
    }

    private void ReturnToStartPos()
    {
        MoveTo(startPos);
    }

    private void MoveTo(Vector2 postion)
    {
        BoardVariable.MoveTileSequence.Join(rectTransform.DOAnchorPos(postion, 0.1f));
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
            if (BoardVariable.IsValidSwap(this, tileToDrive))
            {
                BoardVariable.MoveTileSequence = DOTween.Sequence();
                SwapWithAnim(this, tileToDrive);
                BoardVariable.MoveTileSequence.AppendCallback(() => BoardVariable.MarkForRemoval());
            }
            else
            {
                BoardVariable.MoveTileSequence = DOTween.Sequence();
                ReturnToStartPos();
                tileToDrive.ReturnToStartPos();
            }
        }
    }


    private TileController GetTileToDrive(Vector2 displacement)
    {
        Vector2Int direction = new Vector2Int((int)displacement.normalized.x, (int)displacement.normalized.y);

        Vector2Int index = arrayIndex + direction;

        return BoardVariable.TryGetTile(index);
    }


    private void DriveTile(Vector2 displacement)
    {
        rectTransform.anchoredPosition = startPos - displacement;
    }


    public static void SwapWithAnim(TileController a, TileController b)
    {
        BoardVariable.Swap(a, b);
        if (!a.IsCollcted) a.ReturnToStartPos();
        if (!b.IsCollcted) b.ReturnToStartPos();
    }

    public void Collect(Sequence collectSequence)
    {
        IsCollcted = true;
        MarkedForRemoval = false;
        tileVariable.collectBehaviour.Collect(this, collectSequence, tileVariable.points);
    }

    public void AddNew(TileVariable tile)
    {
        rectTransform.anchoredPosition = startPos;
        tileImage.rectTransform.anchoredPosition = new Vector2(BoardVariable.replaceFrom.x * rectTransform.rect.height, BoardVariable.replaceFrom.y * rectTransform.rect.height);
        tileImage.rectTransform.localScale = Vector2.one;
        TileVariable = tile;
        BoardVariable.MoveTileSequence.Join(tileImage.rectTransform.DOAnchorPos(Vector2.zero, 0.1f));
        IsCollcted = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!tileVariable.clickable) return;
        MarkedForRemoval = true;
        BoardVariable.RemoveTiles();
    }
}
