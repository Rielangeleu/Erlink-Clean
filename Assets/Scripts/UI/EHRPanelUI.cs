using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// Controls the EHR intervention panel.
/// Thesis Rule 4: Student must select correct intervention
/// after triage tagging before session ends.
/// Appropriate Action = 1 point in accuracy scoring.
/// </summary>
public class EHRPanelUI : MonoBehaviour
{
    [Header("UI References")]
    public Button[] ehrOptionButtons;
    public TextMeshProUGUI[] ehrOptionTexts;
    public GameObject feedbackPanel;
    public TextMeshProUGUI feedbackText;

    [Header("Dependencies")]
    public TriageDecisionManager decisionManager;
    public ScenarioLoader scenarioLoader;
    public ScoringSystem scoringSystem;

    [Header("Colors")]
    public Color defaultColor = new Color(0.953f, 0.953f, 0.961f);
    public Color correctColor = new Color(0.086f, 0.635f, 0.290f);
    public Color wrongColor = new Color(0.863f, 0.149f, 0.149f);

    private bool _answered = false;
    private bool _isCorrect = false;
    private int _selectedIndex = -1;

    void Start()
    {
        // Find scoring system if not assigned
        if (scoringSystem == null)
            scoringSystem = FindFirstObjectByType<ScoringSystem>();
    }

    public void PopulateEHRActions(ScenarioData scenario)
    {
        if (scenario == null || scenario.ehrActions == null) return;

        // Reset interaction lock tracking state flags
        _answered = false;
        _isCorrect = false;
        _selectedIndex = -1;

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);

        for (int i = 0; i < ehrOptionButtons.Length; i++)
        {
            if (i < scenario.ehrActions.Length)
            {
                ehrOptionButtons[i].gameObject.SetActive(true);
                ehrOptionButtons[i].interactable = true;

                var img = ehrOptionButtons[i].GetComponent<UnityEngine.UI.Image>();
                if (img != null) img.color = defaultColor;

                ehrOptionTexts[i].text = scenario.ehrActions[i].actionName;
                ehrOptionTexts[i].color = new Color(0.12f, 0.16f, 0.23f);
            }
            else
            {
                ehrOptionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnEHROptionSelected(int index)
    {
        if (_answered) return;
        _answered = true;
        _selectedIndex = index;

        ScenarioData scenario = scenarioLoader.GetActiveScenario();
        bool isCorrect = scenario.ehrActions[index].isCorrectAction;
        _isCorrect = isCorrect;

        Color resultColor = isCorrect ? correctColor : wrongColor;

        // Animate selected button
        Image btnImage = ehrOptionButtons[index].GetComponent<Image>();
        if (btnImage != null)
        {
            btnImage.DOKill();
            btnImage.DOColor(resultColor, 0.3f);
        }

        RectTransform btnRt = ehrOptionButtons[index].GetComponent<RectTransform>();
        if (btnRt != null)
        {
            btnRt.DOKill();
            btnRt.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.3f);
        }

        // Show feedback
        if (feedbackPanel != null && feedbackText != null)
        {
            feedbackPanel.SetActive(true);
            feedbackText.text = isCorrect
                ? $"✓ Correct! {scenario.ehrActions[index].actionDescription}"
                : $"✗ Incorrect. {scenario.ehrActions[index].actionDescription}";
            feedbackText.color = resultColor;
        }

        // Disable all buttons after selection
        foreach (var btn in ehrOptionButtons)
            if (btn != null) btn.interactable = false;

        // Send to scoring system (Appropriate Action = 1 point)
        if (scoringSystem != null)
        {
            scoringSystem.RecordEHRAction(isCorrect);
            Debug.Log($"EHR Action Selected: {(isCorrect ? "Correct" : "Incorrect")} - Appropriate Action +{(isCorrect ? "1" : "0")} point");
        }

        // Notify decision manager after delay
        Invoke(nameof(NotifyDecisionManager), 2.0f);
    }

    void NotifyDecisionManager()
    {
        if (decisionManager != null)
            decisionManager.OnEHRActionSelected(_isCorrect);
    }

    public void ResetPanel()
    {
        _answered = false;
        _isCorrect = false;
        _selectedIndex = -1;

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);

        // Reset button appearances
        for (int i = 0; i < ehrOptionButtons.Length; i++)
        {
            if (ehrOptionButtons[i] != null && ehrOptionButtons[i].gameObject.activeSelf)
            {
                ehrOptionButtons[i].interactable = true;
                var img = ehrOptionButtons[i].GetComponent<Image>();
                if (img != null) img.color = defaultColor;
                ehrOptionTexts[i].color = new Color(0.12f, 0.16f, 0.23f);
            }
        }
    }
}