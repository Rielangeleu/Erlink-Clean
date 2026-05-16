using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls all UI elements during an AR triage scenario.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Top Bar")]
    public TextMeshProUGUI scenarioTitleText;
    public TextMeshProUGUI timerText;

    [Header("Triage Tags (Drag-Drop version)")]
    public GameObject triageTagsRow;

    [Header("Panels")]
    public GameObject rpmPanel;
    public GameObject ehrPanel;
    public GameObject codeAlertBanner;
    public TextMeshProUGUI codeAlertText;
    public GameObject placementPrompt;

    [Header("Submit Button")]
    public Button submitButton;

    [Header("Scenario Card")]
    public GameObject scenarioCard;
    public TextMeshProUGUI difficultyBadge;
    public TextMeshProUGUI patientStatusText;

    [Header("Triage Colors")]
    public Color immediateColor = new Color(0.863f, 0.149f, 0.149f);
    public Color delayedColor = new Color(0.851f, 0.467f, 0.024f);
    public Color minorColor = new Color(0.086f, 0.639f, 0.290f);
    public Color disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

    void Start()
    {
        // Tags locked until RPM complete
        SetTriageTagsInteractable(false);

        // Submit locked until tag dropped
        if (submitButton != null)
            submitButton.interactable = false;

        if (scenarioCard != null)
            scenarioCard.SetActive(false);
    }

    // ── Scenario Title ───────────────────────────────────
    public void SetScenarioTitle(string title)
    {
        if (scenarioTitleText != null)
            scenarioTitleText.text = title;
    }

    // ── Timer ────────────────────────────────────────────
    public void UpdateTimerDisplay(float secondsRemaining)
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(secondsRemaining / 60f);
        int seconds = Mathf.FloorToInt(secondsRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.color = secondsRemaining <= 30f ?
            new Color(1f, 0.3f, 0.3f) : Color.white;
    }

    // ── Triage Tags Interactability ──────────────────────
    public void SetTriageTagsInteractable(bool interactable)
    {
        if (triageTagsRow == null) return;

        // Enable/disable all TriageDragDrop components
        foreach (var drag in
            triageTagsRow.GetComponentsInChildren<TriageDragDrop>())
        {
            drag.enabled = interactable;
        }

        // Also toggle CanvasGroup alpha for visual feedback
        CanvasGroup cg = triageTagsRow.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = interactable ? 1f : 0.4f;
            cg.interactable = interactable;
            cg.blocksRaycasts = interactable;
        }
    }

    // Keep this for backward compatibility with older scripts
    public void SetTriageButtonsInteractable(bool interactable)
    {
        SetTriageTagsInteractable(interactable);
    }

    // ── Panels ───────────────────────────────────────────
    public void ShowRPMPanel(bool show)
    {
        if (rpmPanel != null)
            rpmPanel.SetActive(show);
    }

    public void ShowEHRPanel(bool show)
    {
        if (ehrPanel != null)
            ehrPanel.SetActive(show);
    }

    public void ShowPlacementPrompt(bool show)
    {
        if (placementPrompt != null)
            placementPrompt.SetActive(show);
    }

    // ── Code Alert ───────────────────────────────────────
    public void ShowCodeAlert(string message)
    {
        if (codeAlertBanner == null) return;
        codeAlertBanner.SetActive(true);
        if (codeAlertText != null)
            codeAlertText.text = message;
    }

    public void HideCodeAlert()
    {
        if (codeAlertBanner != null)
            codeAlertBanner.SetActive(false);
    }

    // ── Scenario Card ────────────────────────────────────
    public void ShowScenarioCard(string difficulty, string status)
    {
        if (scenarioCard == null) return;
        scenarioCard.SetActive(true);
        if (difficultyBadge != null)
            difficultyBadge.text = difficulty.ToUpper();
        if (patientStatusText != null)
            patientStatusText.text = status;
    }
}