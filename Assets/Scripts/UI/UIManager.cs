using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls all UI elements during an AR triage scenario.
/// Updated to match the specific PatientInfoCard hierarchy configuration.
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

    // ── REMOVED OLD SCENARIO CARD / ADDED EXACT MATCH REFERENCE ──
    [Header("Patient Info Card System")]
    public GameObject patientInfoCard;
    public TextMeshProUGUI difficultyBadge;
    public TextMeshProUGUI patientStatusText;

    [Header("Submit Button")]
    public Button submitButton;

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

        // ── FIX: Ensure your actual PatientInfoCard STAYS VISIBLE on start ──
        if (patientInfoCard != null)
            patientInfoCard.SetActive(true);
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

        foreach (var drag in
            triageTagsRow.GetComponentsInChildren<TriageDragDrop>())
        {
            if (drag != null) drag.enabled = interactable;
        }

        CanvasGroup cg = triageTagsRow.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = interactable ? 1f : 0.4f;
            cg.interactable = interactable;
            cg.blocksRaycasts = interactable;
        }
    }

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

    // ── Keep method compatible with older scene references ──
    public void ShowScenarioCard(string difficulty, string status)
    {
        if (patientInfoCard != null)
            patientInfoCard.SetActive(true);
        if (difficultyBadge != null)
            difficultyBadge.text = difficulty.ToUpper();
        if (patientStatusText != null)
            patientStatusText.text = status;
    }
}