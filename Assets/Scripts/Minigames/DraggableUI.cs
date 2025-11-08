using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int id;

    RectTransform rectTransform;
    Canvas canvas;
    CanvasGroup canvasGroup;

    Vector2 originalAnchoredPos;
    Transform originalParent;
    int originalSiblingIndex;
    bool placed = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        originalParent = transform.parent;
        originalAnchoredPos = rectTransform.anchoredPosition;
        originalSiblingIndex = transform.GetSiblingIndex();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (placed) return;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.9f;

        if (canvas != null)
            transform.SetParent(canvas.transform, false); // false supaya posisi relatif tidak kacau
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (placed) return;
        if (canvas == null) return;

        Vector2 localPoint;
        RectTransform canvasRect = canvas.transform as RectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (placed) return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (transform.parent == canvas.transform)
        {
            ReturnToOriginal();
        }
    }

    // Dipanggil oleh DropZone setelah sprite berhasil diganti
    public void OnPlacedIntoDropZone()
    {
        placed = true;
        // Hilangkan draggable dari layar (kamu bisa ganti jadi disable component jika mau)
        gameObject.SetActive(false);
    }

    public void ReturnToOriginal()
    {
        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalSiblingIndex);
        rectTransform.anchoredPosition = originalAnchoredPos;
    }
}
