using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Controls the sequential RPM assessment.
/// Enforces R → P → M order per thesis rules.
/// Triage buttons stay locked until all 3 steps complete.
/// </summary>
public class RPMSequenceController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject rpmPanel;
    public TextMeshProUGUI stepTitleText;
    public TextMeshProUGUI questionText;
    public Button[] optionButtons;
    public TextMeshProUGUI[] optionButtonTexts;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;
    public GameObject feedbackPanel;

    [Header("Dependencies")]
    public UIManager uiManager;

    // Internal state
    private ScenarioData _currentScenario;
    private int _currentStep = 0; // 0=R, 1=P, 2=M
    private bool[] _stepCompleted = new bool[3];
    private int[] _selectedAnswers = new int[3];

    // Events
    public Action OnRPMComplete;

    private string[] _stepNames = { "Respiration", "Perfusion", "Mental Status" };
    private string[] _stepIcons = { "STEP 1 OF 3 — RESPIRATION",
                                     "STEP 2 OF 3 — PERFUSION",
                                     "STEP 3 OF 3 — MENTAL STATUS" };

    public void StartRPMAssessment(ScenarioData scenario)
    {
        _currentScenario = scenario;
        _currentStep = 0;
        _stepCompleted = new bool[3];
        _selectedAnswers = new int[3];

        // Show RPM panel, hide triage buttons
        rpmPanel.SetActive(true);
        uiManager.SetTriageButtonsInteractable(false);

        LoadCurrentStep();
    }

    void LoadCurrentStep()
    {
        RPMAssessment rpm = _currentScenario.rpmAssessment;
        string question = "";
        string[] options = null;

        switch (_currentStep)
        {
            case 0: // Respiration
                question = rpm.respirationQuestion;
                options = rpm.respirationOptions;
                break;
            case 1: // Perfusion
                question = rpm.perfusionQuestion;
                options = rpm.perfusionOptions;
                break;
            case 2: // Mental Status
                question = rpm.mentalStatusQuestion;
                options = rpm.mentalStatusOptions;
                break;
        }

        // Update UI
        stepTitleText.text = _stepIcons[_currentStep];
        questionText.text = question;

        // Set button texts and reset colors
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < options.Length)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtonTexts[i].text = options[i];
                optionButtons[i].interactable = true;

                // Reset button color to white
                optionButtons[i].GetComponent<Image>().color = Color.white;
                optionButtonTexts[i].color = new Color(0.12f, 0.16f, 0.24f);
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }

        // Hide feedback
        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
    }

    // Called by each option button — assign button index 0, 1, 2
    public void OnOptionSelected(int selectedIndex)
    {
        _selectedAnswers[_currentStep] = selectedIndex;
        RPMAssessment rpm = _currentScenario.rpmAssessment;

        int correctIndex = 0;
        string feedback = "";

        switch (_currentStep)
        {
            case 0:
                correctIndex = rpm.correctRespirationIndex;
                feedback = rpm.respirationFeedback;
                break;
            case 1:
                correctIndex = rpm.correctPerfusionIndex;
                feedback = rpm.perfusionFeedback;
                break;
            case 2:
                correctIndex = rpm.correctMentalStatusIndex;
                feedback = rpm.mentalStatusFeedback;
                break;
        }

        bool isCorrect = selectedIndex == correctIndex;

        // Color the selected button
        Color selectedColor = isCorrect ?
            new Color(0.086f, 0.635f, 0.29f) :  // green
            new Color(0.863f, 0.149f, 0.149f);   // red

        optionButtons[selectedIndex].GetComponent<Image>().color = selectedColor;
        optionButtonTexts[selectedIndex].color = Color.white;

        // Show feedback
        if (feedbackPanel != null && feedbackText != null)
        {
            feedbackPanel.SetActive(true);
            feedbackText.text = feedback;
            feedbackText.color = isCorrect ?
                new Color(0.086f, 0.635f, 0.29f) :
                new Color(0.863f, 0.149f, 0.149f);
        }

        _stepCompleted[_currentStep] = true;

        // Disable buttons after selection
        foreach (var btn in optionButtons)
            btn.interactable = false;

        // Move to next step after delay
        Invoke(nameof(NextStep), 1.5f);
    }

    void NextStep()
    {
        _currentStep++;

        if (_currentStep < 3)
        {
            LoadCurrentStep();
        }
        else
        {
            // All 3 steps done
            CompleteRPM();
        }
    }

    void CompleteRPM()
    {
        // Hide RPM panel
        rpmPanel.SetActive(false);

        // Unlock triage buttons
        uiManager.SetTriageButtonsInteractable(true);

        // Fire completion event
        OnRPMComplete?.Invoke();

        Debug.Log("RPM Assessment complete. Triage buttons unlocked.");
    }

    public int[] GetSelectedAnswers()
    {
        return _selectedAnswers;
    }
}