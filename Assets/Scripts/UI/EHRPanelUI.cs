using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// Controls the EHR intervention panel.
/// Thesis Rule 4: Student must select correct intervention
/// after triage tagging before session ends.
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

    [Header("Colors")]
    public Color defaultColor = new Color(0.953f, 0.953f, 0.961f);
    public Color correctColor = new Color(0.086f, 0.635f, 0.290f);
    public Color wrongColor = new Color(0.863f, 0.149f, 0.149f);

    private bool _answered = false;

    public void PopulateEHRActions(ScenarioData scenario)
    {
        if (scenario.ehrActions == null) return;

        for (int i = 0; i < ehrOptionButtons.Length; i++)
        {
            if (i < scenario.ehrActions.Length)
            {
                ehrOptionButtons[i].gameObject.SetActive(true);
                ehrOptionTexts[i].text =
                    scenario.ehrActions[i].actionName;
            }
            else
            {
                ehrOptionButtons[i].gameObject.SetActive(false);
            }
        }

        _answered = false;
        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
    }

    public void OnEHROptionSelected(int index)
    {
        if (_answered) return;
        _answered = true;

        ScenarioData scenario = scenarioLoader.GetActiveScenario();
        bool isCorrect = scenario.ehrActions[index].isCorrectAction;

        // Color feedback
        Color resultColor = isCorrect ? correctColor : wrongColor;
        ehrOptionButtons[index].GetComponent<Image>()
            .DOColor(resultColor, 0.3f);
        ehrOptionTexts[index].color = Color.white;

        // Punch animation
        ehrOptionButtons[index].GetComponent<RectTransform>()
            .DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.3f);

        // Show feedback
        if (feedbackPanel != null && feedbackText != null)
        {
            feedbackPanel.SetActive(true);
            feedbackText.text = isCorrect
                ? $"Correct! {scenario.ehrActions[index].actionDescription}"
                : $"Incorrect. {scenario.ehrActions[index].actionDescription}";
            feedbackText.color = resultColor;
        }

        // Disable all buttons
        foreach (var btn in ehrOptionButtons)
            btn.interactable = false;

        // Notify decision manager after delay
        Invoke(nameof(NotifyDecisionManager), 2f);
        _isCorrect = isCorrect;
    }

    private bool _isCorrect;

    void NotifyDecisionManager()
    {
        decisionManager.OnEHRActionSelected(_isCorrect);
    }
}