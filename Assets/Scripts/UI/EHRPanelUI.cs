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
        if (scenario == null || scenario.ehrActions == null) return;

        // Reset interaction lock tracking state flags
        _answered = false;

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);

        for (int i = 0; i < ehrOptionButtons.Length; i++)
        {
            if (i < scenario.ehrActions.Length)
            {
                ehrOptionButtons[i].gameObject.SetActive(true);

                // Re-enable button interaction tracking states for the incoming patient assignment
                ehrOptionButtons[i].interactable = true;

                // Re-assign button background color back to its standard tint look definition metrics
                var img = ehrOptionButtons[i].GetComponent<UnityEngine.UI.Image>();
                if (img != null) img.color = defaultColor;

                ehrOptionTexts[i].text = scenario.ehrActions[i].actionName;
                ehrOptionTexts[i].color = new Color(0.12f, 0.16f, 0.23f); // Clean dark slate text color layer look
            }
            else
            {
                // Turn off extra slots cleanly if the incoming scenario has fewer than 3 options
                ehrOptionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnEHROptionSelected(int index)
    {
        if (_answered) return;
        _answered = true;

        ScenarioData scenario = scenarioLoader.GetActiveScenario();
        bool isCorrect = scenario.ehrActions[index].isCorrectAction;

        Color resultColor = isCorrect ? correctColor : wrongColor;

        // Fix: Verify image exists before running DOTween
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

        if (feedbackPanel != null && feedbackText != null)
        {
            feedbackPanel.SetActive(true);
            feedbackText.text = isCorrect
                ? $"Correct! {scenario.ehrActions[index].actionDescription}"
                : $"Incorrect. {scenario.ehrActions[index].actionDescription}";
            feedbackText.color = resultColor;
        }

        foreach (var btn in ehrOptionButtons)
            if (btn != null) btn.interactable = false;

        Invoke(nameof(NotifyDecisionManager), 2f);
        _isCorrect = isCorrect;
    }

    private bool _isCorrect;

    void NotifyDecisionManager()
    {
        decisionManager.OnEHRActionSelected(_isCorrect);
    }
}