using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// Controls the circular drop zone over the patient.
/// Detects when a triage tag is dropped on it.
/// Activates Submit button when a valid tag is placed.
/// </summary>
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
        // Check if pointer is within drop zone bounds
        return RectTransformUtility.RectangleContainsScreenPoint(
            _rectTransform,
            eventData.position,
            eventData.pressEventCamera);
    }

    public void OnTagDropped(TriageDragDrop tag)
    {
        // Remove previous tag if any
        if (_currentTag != null && _currentTag != tag)
            _currentTag.ResetTag();

        _currentTag = tag;
        _hasTag = true;

        // Visual feedback
        dropZoneImage.DOColor(droppedColor, 0.3f);
        dropZoneLabel.text = GetTagLabel(tag.triageCategory);
        dropZoneLabel.color = Color.white;

        // Pulse animation
        _rectTransform.DOPunchScale(
            new Vector3(0.05f, 0.05f, 0), 0.4f, 5, 0.5f);

        // Enable submit button
        SetSubmitEnabled(true);

        Debug.Log($"Drop zone received: {tag.triageCategory}");
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Unity's built-in drop handler as backup
    }

    void SetSubmitEnabled(bool enabled)
    {
        submitButton.interactable = enabled;

        submitButtonImage.DOColor(
            enabled ? enabledColor : disabledColor, 0.3f);

        Color textColor = Color.white;
        textColor.a = enabled ? 1f : 0.5f;
        submitButtonText.color = textColor;
    }

    public TriageCategory GetSelectedCategory()
    {
        return _currentTag != null ?
            _currentTag.triageCategory : TriageCategory.Minor;
    }

    public bool HasTag() => _hasTag;

    public void ResetDropZone()
    {
        if (_currentTag != null)
            _currentTag.ResetTag();

        _currentTag = null;
        _hasTag = false;

        dropZoneImage.DOColor(defaultColor, 0.3f);
        dropZoneLabel.text = "Drop Tag Here";
        dropZoneLabel.color = new Color(0.392f, 0.451f, 0.529f);

        SetSubmitEnabled(false);
    }

    string GetTagLabel(TriageCategory cat)
    {
        return cat switch
        {
            TriageCategory.Immediate => "RED — Immediate",
            TriageCategory.Delayed => "YELLOW — Delayed",
            TriageCategory.Minor => "GREEN — Minor",
            TriageCategory.Expectant => "BLACK — Deceased",
            _ => "Tag Placed"
        };
    }
}