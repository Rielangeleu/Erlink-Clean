using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Controls the sequential RPM assessment.
/// Displays all bubbles at once to test student critical thinking.
/// Enforces R -> P -> M order strictly with generic error filtering.
/// </summary>
public class RPMSequenceController : MonoBehaviour
{
    [Header("UI Question Quiz Panels")]
    public GameObject rpmPanel;
    public TextMeshProUGUI stepTitleText;
    public TextMeshProUGUI questionText;
    public Button[] optionButtons;
    public TextMeshProUGUI[] optionButtonTexts;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;
    public GameObject feedbackPanel;

    [Header("3D Interactive Viewport Bubbles")]
    public GameObject respirationBubble;
    public GameObject perfusionBubble;
    public GameObject mentalBubble;

    [Header("Dependencies")]
    public UIManager uiManager;
    public PatientInfoCardUI infoCardUI;

    // Internal state management tracking
    private ScenarioData _currentScenario;
    private int _assessmentOrderIndex = 0; // 0 = Waiting for R, 1 = Waiting for P, 2 = Waiting for M, 3 = All Pills Shown
    private int _quizStepIndex = 0;        // Controls the subsequent quiz panel question progression loop
    private int[] _selectedAnswers = new int[3];

    // Events
    public Action OnRPMComplete;

    private string[] _stepIcons = { "STEP 1 OF 3 — RESPIRATION ASSESSMENT",
                                     "STEP 2 OF 3 — PERFUSION ASSESSMENT",
                                     "STEP 3 OF 3 — MENTAL STATUS ASSESSMENT" };

    public void StartRPMAssessment(ScenarioData scenario)
    {
        _currentScenario = scenario;
        _assessmentOrderIndex = 0;
        _quizStepIndex = 0;
        _selectedAnswers = new int[3];

        // Enforce total interface isolation at startup
        if (rpmPanel != null) rpmPanel.SetActive(false);
        if (uiManager != null) uiManager.SetTriageButtonsInteractable(false);

        // SHOW ALL BUBBLES AT ONCE to force the student to make an intentional choice
        if (respirationBubble != null) respirationBubble.SetActive(true);
        if (perfusionBubble != null) perfusionBubble.SetActive(true);
        if (mentalBubble != null) mentalBubble.SetActive(true);

        Debug.Log("Sequential RPM Initialized: All bubbles visible. Enforcing R -> P -> M order.");
    }

    // ── 3D WORLD INTERACTION CLICK HOOKS WITH ORDER & PLACEMENT VALIDATION ──

    public void OnRespirationBubbleTapped()
    {
        // CRITICAL CHECK: Block clicks if no patient has been spawned into AR yet!
        if (_currentScenario == null)
        {
            TriggerSequenceErrorAlert("You must tap the scene floor to place the patient first!");
            return;
        }

        if (rpmPanel.activeSelf) return;

        if (_assessmentOrderIndex == 0)
        {
            _assessmentOrderIndex = 1;
            if (respirationBubble != null) respirationBubble.SetActive(false);

            if (infoCardUI != null)
                infoCardUI.RevealRespirationValue(_currentScenario.vitalSigns.Length > 0 ? _currentScenario.vitalSigns[0] : "Assessed");

            CheckSequenceProgression();
        }
        else
        {
            TriggerSequenceErrorAlert("Incorrect Sequence!");
        }
    }

    public void OnPerfusionBubbleTapped()
    {
        // CRITICAL CHECK: Block clicks if no patient has been spawned into AR yet!
        if (_currentScenario == null)
        {
            TriggerSequenceErrorAlert("You must tap the scene floor to place the patient first!");
            return;
        }

        if (rpmPanel.activeSelf) return;

        if (_assessmentOrderIndex == 1)
        {
            _assessmentOrderIndex = 2;
            if (perfusionBubble != null) perfusionBubble.SetActive(false);

            if (infoCardUI != null)
                infoCardUI.RevealPerfusionValue(_currentScenario.vitalSigns.Length > 1 ? _currentScenario.vitalSigns[1] : "Assessed");

            CheckSequenceProgression();
        }
        else
        {
            TriggerSequenceErrorAlert("Incorrect Sequence!");
        }
    }

    public void OnMentalBubbleTapped()
    {
        // CRITICAL CHECK: Block clicks if no patient has been spawned into AR yet!
        if (_currentScenario == null)
        {
            TriggerSequenceErrorAlert("You must tap the scene floor to place the patient first!");
            return;
        }

        if (rpmPanel.activeSelf) return;

        if (_assessmentOrderIndex == 2)
        {
            _assessmentOrderIndex = 3;
            if (mentalBubble != null) mentalBubble.SetActive(false);

            if (infoCardUI != null)
                infoCardUI.RevealMentalValue(_currentScenario.vitalSigns.Length > 2 ? _currentScenario.vitalSigns[2] : "Assessed");

            CheckSequenceProgression();
        }
        else
        {
            TriggerSequenceErrorAlert("Incorrect Sequence!");
        }
    }

    // ── INTERACTION SEQUENCE ROUTING LOGIC ──

    void CheckSequenceProgression()
    {
        if (_assessmentOrderIndex == 3)
        {
            _quizStepIndex = 0;
            Invoke(nameof(ShowCurrentQuestionQuizWindow), 0.5f);
        }
    }

    void TriggerSequenceErrorAlert(string trackingMessage)
    {
        if (uiManager != null)
        {
            uiManager.ShowCodeAlert(trackingMessage);

            CancelInvoke(nameof(ClearAlertBanner));
            Invoke(nameof(ClearAlertBanner), 2.5f);
        }
    }

    void ClearAlertBanner()
    {
        if (uiManager != null)
            uiManager.HideCodeAlert();
    }

    void ShowCurrentQuestionQuizWindow()
    {
        if (rpmPanel != null) rpmPanel.SetActive(true);
        LoadCurrentStepData();
    }

    void LoadCurrentStepData()
    {
        RPMAssessment rpm = _currentScenario.rpmAssessment;
        string question = "";
        string[] options = null;

        switch (_quizStepIndex)
        {
            case 0:
                question = rpm.respirationQuestion;
                options = rpm.respirationOptions;
                break;
            case 1:
                question = rpm.perfusionQuestion;
                options = rpm.perfusionOptions;
                break;
            case 2:
                question = rpm.mentalStatusQuestion;
                options = rpm.mentalStatusOptions;
                break;
        }

        if (stepTitleText != null) stepTitleText.text = _stepIcons[_quizStepIndex];
        if (questionText != null) questionText.text = question;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < options.Length)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtonTexts[i].text = options[i];
                optionButtons[i].interactable = true;

                optionButtons[i].GetComponent<Image>().color = Color.white;
                optionButtonTexts[i].color = new Color(0.12f, 0.16f, 0.24f);
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
    }

    public void OnOptionSelected(int selectedIndex)
    {
        _selectedAnswers[_quizStepIndex] = selectedIndex;
        RPMAssessment rpm = _currentScenario.rpmAssessment;

        int correctIndex = 0;
        string feedback = "";

        switch (_quizStepIndex)
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

        Color selectedColor = isCorrect ?
            new Color(0.086f, 0.635f, 0.29f) :
            new Color(0.863f, 0.149f, 0.149f);

        optionButtons[selectedIndex].GetComponent<Image>().color = selectedColor;
        optionButtonTexts[selectedIndex].color = Color.white;

        if (feedbackPanel != null && feedbackText != null)
        {
            feedbackPanel.SetActive(true);
            feedbackText.text = feedback;
            feedbackText.color = selectedColor;
        }

        foreach (var btn in optionButtons)
            btn.interactable = false;

        Invoke(nameof(NextQuizStep), 2.0f);
    }

    void NextQuizStep()
    {
        if (rpmPanel != null) rpmPanel.SetActive(false);

        _quizStepIndex++;

        if (_quizStepIndex < 3)
        {
            ShowCurrentQuestionQuizWindow();
        }
        else
        {
            CompleteRPM();
        }
    }

    void CompleteRPM()
    {
        if (rpmPanel != null) rpmPanel.SetActive(false);
        if (uiManager != null) uiManager.SetTriageButtonsInteractable(true);

        OnRPMComplete?.Invoke();
        Debug.Log("All 3 sequential verification steps completed. Tags unlocked.");
    }

    public int[] GetSelectedAnswers() => _selectedAnswers;
}