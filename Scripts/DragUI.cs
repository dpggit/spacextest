using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SpaceX
{
    public class DragUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
    {
        public RectTransform rect;
        Canvas canvas;

        void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
        }

        void Start()
        {
            rect.anchoredPosition = Vector2.one * 1000f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {

        }


        public void OnEndDrag(PointerEventData eventData)
        {
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            rect.SetAsLastSibling();
        }
    }
}
