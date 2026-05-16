using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

/// <summary>
/// Handles dragging triage tags onto the patient drop zone.
/// Implements thesis requirement: drag-and-drop triage tagging.
/// Submit button only activates after a tag is dropped on the zone.
/// </summary>
public class TriageDragDrop : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("This Tag's Category")]
    public TriageCategory triageCategory;

    [Header("Visual")]
    public Image tagImage;
    public Color normalColor;
    public Color dragColor = new Color(1f, 1f, 1f, 0.85f);

    [Header("References")]
    public DropZoneController dropZone;
    public Canvas canvas;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector2 _originalPosition;
    private Transform _originalParent;
    private bool _isDropped = false;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        _originalPosition = _rectTransform.anchoredPosition;
        _originalParent = transform.parent;
        normalColor = tagImage != null ?
            tagImage.color : Color.white;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isDropped) return;

        _originalPosition = _rectTransform.anchoredPosition;

        // Make semi-transparent while dragging
        _canvasGroup.alpha = 0.8f;
        _canvasGroup.blocksRaycasts = false;

        // Bring to front
        transform.SetAsLastSibling();

        // Scale up slightly — Framer Motion whileDrag equivalent
        _rectTransform.DOScale(1.08f, 0.15f).SetEase(Ease.OutBack);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDropped) return;

        // Move with finger
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pos);

        _rectTransform.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isDropped) return;

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _rectTransform.DOScale(1f, 0.15f);

        // Check if dropped on drop zone
        if (dropZone != null && dropZone.IsPointerOverDropZone(eventData))
        {
            DropOnZone();
        }
        else
        {
            // Snap back to original position
            ReturnToOrigin();
        }
    }

    void DropOnZone()
    {
        _isDropped = true;

        // Animate to drop zone center
        Vector2 dropCenter = dropZone.GetComponent<RectTransform>()
            .anchoredPosition;

        _rectTransform.DOAnchorPos(dropCenter, 0.25f)
            .SetEase(Ease.OutCubic);
        _rectTransform.DOScale(0.85f, 0.25f);

        // Notify drop zone
        dropZone.OnTagDropped(this);

        Debug.Log($"Tag dropped: {triageCategory}");
    }

    public void ReturnToOrigin()
    {
        _isDropped = false;
        _rectTransform.DOAnchorPos(_originalPosition, 0.3f)
            .SetEase(Ease.OutBack);
        _rectTransform.DOScale(1f, 0.2f);
    }

    public void ResetTag()
    {
        _isDropped = false;
        ReturnToOrigin();
    }
}