using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragableRect : MonoBehaviour//, IDragHandler, IEndDragHandler, IBeginDragHandler
{ 
//{

//    RectTransform parent;
//    RectTransform rectTransform;

//    private void OnEnable()
//    {
        
//    }

//    public void OnBeginDrag(PointerEventData eventData)
//    {
//        throw new System.NotImplementedException();
//    }

//    public void OnDrag(PointerEventData eventData)
//    {
//        Vector2 position;
//        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, eventData.position, null, out position);
//        displacement = Vector2.ClampMagnitude((position - pointInTile) - startPos, rectTransform.rect.width);

//        displacement = ((displacement.magnitude < rectTransform.rect.width / 2) ? displacement : displacement * (Mathf.Abs(displacement.x) < Mathf.Abs(displacement.y) ? Vector2.up : Vector2.right));
//    }

//    public void OnEndDrag(PointerEventData eventData)
//    {
//        throw new System.NotImplementedException();
//    }

    
}
