using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler
{
    public int id;

    [Tooltip("Drag Image component of this drop zone here (inspector).")]
    public Image targetImage;

    public bool disableDraggableAfterDrop = true;

    void Reset()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dropped = eventData.pointerDrag;
        Debug.Log($"DropZone.OnDrop called. pointerDrag = {dropped}");

        if (dropped == null) return;

        var draggable = dropped.GetComponent<DraggableUI>();
        var dragImage = dropped.GetComponent<Image>();

        if (draggable == null)
        {
            Debug.Log("Dropped object has no DraggableUI script.");
            return;
        }

        Debug.Log($"draggable.id = {draggable.id}, dropzone.id = {id}");

        if (draggable.id == id)
        {
            // fallback: jika targetImage belum di-assign di inspector, coba ambil Image pada DropZone
            if (targetImage == null) targetImage = GetComponent<Image>();

            if (targetImage != null && dragImage != null)
            {
                // swap sprite
                targetImage.sprite = dragImage.sprite;
                targetImage.preserveAspect = true;

                // pastikan ukuran visual targetImage mengikuti drop zone
                RectTransform dropRect = GetComponent<RectTransform>();
                RectTransform tiRect = targetImage.rectTransform;
                Vector2 targetSize = dropRect.rect.size;
                if (targetSize == Vector2.zero) targetSize = dropRect.sizeDelta;
                tiRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetSize.x);
                tiRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetSize.y);

                Debug.Log("Sprite swapped into drop zone: " + (dragImage.sprite != null ? dragImage.sprite.name : "null"));
            }
            else
            {
                Debug.LogWarning("targetImage or dragImage is null. Make sure DropZone.targetImage assigned and Draggable has Image.");
            }

            if (disableDraggableAfterDrop)
            {
                draggable.OnPlacedIntoDropZone();
            }

            MiniManager.Instance?.RegisterSuccess();
        }
        else
        {
            draggable.ReturnToOriginal();
            Debug.Log("Wrong place (salah posisi).");
        }
    }
}
