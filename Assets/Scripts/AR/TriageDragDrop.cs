using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using System.Collections;

/// <summary>
/// Handles dragging triage tags onto the patient drop zone.
/// Implements thesis requirement: drag-and-drop triage tagging.
/// Fixes the drop alignment by snapping precisely to the drop zone's world center.
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
    private bool _hasCapturedHome = false;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

        _originalParent = transform.parent;
        normalColor = tagImage != null ? tagImage.color : Color.white;
    }

    void Start()
    {
        // Safe Check: Wait for the layout pass loop to finish arranging columns before saving coordinates
        StartCoroutine(CaptureHomePositionPass());
    }

    IEnumerator CaptureHomePositionPass()
    {
        yield return new WaitForEndOfFrame();
        _originalPosition = _rectTransform.anchoredPosition;
        _hasCapturedHome = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isDropped) return;

        // Fallback capture if things load out of frame sequence order
        if (!_hasCapturedHome)
        {
            _originalPosition = _rectTransform.anchoredPosition;
            _hasCapturedHome = true;
        }

        _canvasGroup.alpha = 0.8f;
        _canvasGroup.blocksRaycasts = false;

        _rectTransform.DOScale(1.08f, 0.15f).SetEase(Ease.OutBack);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDropped) return;

        // Keeps your exact working mouse tracking structure untouched!
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isDropped) return;

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _rectTransform.DOScale(1f, 0.15f);

        if (dropZone != null && dropZone.IsPointerOverDropZone(eventData))
        {
            DropOnZone();
        }
        else
        {
            ReturnToOrigin();
        }
    }

    void DropOnZone()
    {
        _isDropped = true;

        if (dropZone != null)
        {
            _rectTransform.DOKill();

            // ── FIX: Snap perfectly to the drop zone's absolute world position center ──
            // This bypasses parent layout group scaling and keeps it locked exactly in the circle!
            transform.DOMove(dropZone.transform.position, 0.25f).SetEase(Ease.OutCubic);
            _rectTransform.DOScale(0.85f, 0.25f);

            dropZone.OnTagDropped(this);
            Debug.Log($"Tag dropped cleanly: {triageCategory}");
        }
    }

    public void ReturnToOrigin()
    {
        _isDropped = false;

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }

        // Animate using DOAnchorPos back to the cached slot captured right at Start()!
        _rectTransform.DOKill();
        _rectTransform.DOAnchorPos(_originalPosition, 0.3f).SetEase(Ease.OutBack);
        _rectTransform.DOScale(1f, 0.2f);
    }

    public void ResetTag()
    {
        _isDropped = false;
        ReturnToOrigin();
    }
}