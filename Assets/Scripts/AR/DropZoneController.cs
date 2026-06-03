using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class DropZoneController : MonoBehaviour, IDropHandler
{
    [Header("UI References")]
    public Image dropZoneImage;
    public TextMeshProUGUI dropZoneLabel;
    public Button submitButton;
    public Image submitButtonImage;
    public TextMeshProUGUI submitButtonText;

    [Header("Colors")]
    public Color defaultColor = new Color(0.278f, 0.333f, 0.404f, 0.7f);
    public Color hoverColor = new Color(0.145f, 0.239f, 0.369f, 0.9f);
    public Color droppedColor = new Color(0.086f, 0.635f, 0.290f, 0.9f);

    [Header("Submit Button Colors")]
    public Color disabledColor = new Color(0.145f, 0.239f, 0.369f, 0.6f);
    public Color enabledColor = new Color(0.145f, 0.337f, 0.922f, 1f);

    private TriageDragDrop _currentTag;
    private bool _hasTag = false;
    private RectTransform _rectTransform;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        SetSubmitEnabled(false);
    }

    public bool IsPointerOverDropZone(PointerEventData eventData)
    {
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();

        // Fail-safe 1: Standard rectangle boundary check
        bool insideRect = RectTransformUtility.RectangleContainsScreenPoint(
            _rectTransform,
            eventData.position,
            eventData.pressEventCamera);

        if (insideRect) return true;

        // Fail-safe 2: Distance-based fallback check (Great for varied mobile resolutions)
        float distance = Vector2.Distance(transform.position, eventData.position);
        return distance < 120f; // Registers a match if dropped within a 120-pixel radius of center
    }

    public void OnTagDropped(TriageDragDrop tag)
    {
        if (_currentTag != null && _currentTag != tag)
            _currentTag.ReturnToOrigin();

        _currentTag = tag;
        _hasTag = true;

        // Ensure target visual components are valid before running DOTween animations
        if (dropZoneImage != null)
        {
            dropZoneImage.DOKill();
            dropZoneImage.DOColor(droppedColor, 0.3f);
        }

        if (dropZoneLabel != null)
        {
            dropZoneLabel.text = GetTagLabel(tag.triageCategory);
            dropZoneLabel.color = Color.white;
        }

        _rectTransform.DOKill();
        _rectTransform.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.4f, 5, 0.5f);

        // Force enable submit button
        SetSubmitEnabled(true);
        Debug.Log($"Drop zone successfully locked tag: {tag.triageCategory} ✅");
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Engine backup interface handler
        TriageDragDrop dragTag = eventData.pointerDrag?.GetComponent<TriageDragDrop>();
        if (dragTag != null)
        {
            OnTagDropped(dragTag);
        }
    }

    public void SetSubmitEnabled(bool enabled)
    {
        if (submitButton != null) submitButton.interactable = enabled;

        if (submitButtonImage != null)
        {
            submitButtonImage.DOKill();
            submitButtonImage.DOColor(enabled ? enabledColor : disabledColor, 0.3f);
        }

        if (submitButtonText != null)
        {
            Color textColor = Color.white;
            textColor.a = enabled ? 1f : 0.5f;
            submitButtonText.color = textColor;
        }
    }

    public TriageCategory GetSelectedCategory()
    {
        return _currentTag != null ? _currentTag.triageCategory : TriageCategory.Minor;
    }

    public bool HasTag() => _hasTag;

    public void ResetDropZone()
    {
        if (_currentTag != null)
            _currentTag.ReturnToOrigin();

        _currentTag = null;
        _hasTag = false;

        if (dropZoneImage != null)
        {
            dropZoneImage.DOKill();
            dropZoneImage.DOColor(defaultColor, 0.3f);
        }

        if (dropZoneLabel != null)
        {
            dropZoneLabel.text = "Drop Tag Here";
            dropZoneLabel.color = new Color(0.392f, 0.451f, 0.529f);
        }

        SetSubmitEnabled(false);
    }

    string GetTagLabel(TriageCategory cat) => cat switch
    {
        TriageCategory.Immediate => "RED — Immediate",
        TriageCategory.Delayed => "YELLOW — Delayed",
        TriageCategory.Minor => "GREEN — Minor",
        TriageCategory.Expectant => "BLACK — Deceased",
        _ => "Tag Placed"
    };
}