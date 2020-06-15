using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickBlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public InputManager IM; //InputManager reference

    //Calls When player hovers over UI
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(GetComponent<CanvasGroup>().alpha == 1)
        {
            IM.canClickGround = false;
        }
    }

    //Calls when player stops hovering over UI
    public void OnPointerExit(PointerEventData eventData)
    {
        IM.canClickGround = true;
    }
}
