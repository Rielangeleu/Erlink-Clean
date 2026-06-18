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
    public ScoringSystem scoringSystem;

    // Internal state management tracking
    private ScenarioData _currentScenario;
    private int _assessmentOrderIndex = 0;
    private int _quizStepIndex = 0;
    private int[] _selectedAnswers = new int[3];
    private int[] _correctAnswers = new int[3];
    private int _correctCount = 0;

<<<<<<< HEAD
    // Runtime scenario support
    private RuntimeScenarioData _currentRuntimeScenario;
    
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    // Events
    public Action OnRPMComplete;

    private string[] _stepIcons = {
        "STEP 1 OF 3 — RESPIRATION ASSESSMENT",
        "STEP 2 OF 3 — PERFUSION ASSESSMENT",
        "STEP 3 OF 3 — MENTAL STATUS ASSESSMENT"
    };

    void Start()
    {
<<<<<<< HEAD
=======
        // Find scoring system if not assigned
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        if (scoringSystem == null)
            scoringSystem = FindFirstObjectByType<ScoringSystem>();
    }

    public void StartRPMAssessment(ScenarioData scenario)
    {
        _currentScenario = scenario;
<<<<<<< HEAD
        _currentRuntimeScenario = null;
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        _assessmentOrderIndex = 0;
        _quizStepIndex = 0;
        _selectedAnswers = new int[3];
        _correctCount = 0;

<<<<<<< HEAD
        if (scenario == null || scenario.rpmAssessment == null)
        {
            Debug.LogError("RPMSequenceController: Scenario or rpmAssessment is null!");
            return;
        }

=======
        // Store correct answers for later comparison
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        RPMAssessment rpm = scenario.rpmAssessment;
        _correctAnswers[0] = rpm.correctRespirationIndex;
        _correctAnswers[1] = rpm.correctPerfusionIndex;
        _correctAnswers[2] = rpm.correctMentalStatusIndex;

<<<<<<< HEAD
        if (rpmPanel != null) rpmPanel.SetActive(false);
        if (uiManager != null) uiManager.SetTriageButtonsInteractable(false);

=======
        // Enforce total interface isolation at startup
        if (rpmPanel != null) rpmPanel.SetActive(false);
        if (uiManager != null) uiManager.SetTriageButtonsInteractable(false);

        // SHOW ALL BUBBLES AT ONCE to force the student to make an intentional choice
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        if (respirationBubble != null) respirationBubble.SetActive(true);
        if (perfusionBubble != null) perfusionBubble.SetActive(true);
        if (mentalBubble != null) mentalBubble.SetActive(true);

<<<<<<< HEAD
        Debug.Log("Sequential RPM Initialized for ScriptableObject scenario.");
    }

    public void StartRPMAssessment(RuntimeScenarioData scenario)
    {
        if (scenario == null)
        {
            Debug.LogError("RPMSequenceController: StartRPMAssessment received NULL runtime scenario!");
            return;
        }
        
        _currentRuntimeScenario = scenario;
        _currentScenario = null;
        _assessmentOrderIndex = 0;
        _quizStepIndex = 0;
        _selectedAnswers = new int[3];
        _correctCount = 0;

        Debug.Log($"RPM: Starting assessment for runtime scenario '{scenario.scenarioTitle}'");

        RPMAssessment rpm = scenario.rpmAssessment;
        
        if (rpm == null)
        {
            Debug.LogError("RPMSequenceController: rpmAssessment is NULL in runtime scenario!");
            return;
        }
        
        _correctAnswers[0] = rpm.correctRespirationIndex;
        _correctAnswers[1] = rpm.correctPerfusionIndex;
        _correctAnswers[2] = rpm.correctMentalStatusIndex;

        if (rpmPanel != null) rpmPanel.SetActive(false);
        if (uiManager != null) uiManager.SetTriageButtonsInteractable(false);

        if (respirationBubble != null) respirationBubble.SetActive(true);
        if (perfusionBubble != null) perfusionBubble.SetActive(true);
        if (mentalBubble != null) mentalBubble.SetActive(true);

        Debug.Log($"✅ RPM initialized for runtime scenario: {scenario.scenarioTitle}");
    }

    // Helper method to check if any scenario is active
    private bool HasActiveScenario()
    {
        return (_currentScenario != null || _currentRuntimeScenario != null);
    }

    // Helper method to get vital signs
    private string GetVitalSign(int index)
    {
        if (_currentScenario != null && _currentScenario.vitalSigns != null && index < _currentScenario.vitalSigns.Length)
            return _currentScenario.vitalSigns[index];
        if (_currentRuntimeScenario != null && _currentRuntimeScenario.vitalSigns != null && index < _currentRuntimeScenario.vitalSigns.Length)
            return _currentRuntimeScenario.vitalSigns[index];
        return "Assessed";
    }

    // Helper method to get RPM assessment
    private RPMAssessment GetRPMAssessment()
    {
        if (_currentScenario != null)
            return _currentScenario.rpmAssessment;
        if (_currentRuntimeScenario != null)
            return _currentRuntimeScenario.rpmAssessment;
        return null;
=======
        Debug.Log("Sequential RPM Initialized: All bubbles visible. Enforcing R -> P -> M order.");
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    // ── 3D WORLD INTERACTION CLICK HOOKS WITH ORDER & PLACEMENT VALIDATION ──

    public void OnRespirationBubbleTapped()
    {
<<<<<<< HEAD
        if (!HasActiveScenario())
=======
        // CRITICAL CHECK: Block clicks if no patient has been spawned into AR yet!
        if (_currentScenario == null)
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
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
<<<<<<< HEAD
                infoCardUI.RevealRespirationValue(GetVitalSign(0));
=======
                infoCardUI.RevealRespirationValue(_currentScenario.vitalSigns.Length > 0 ? _currentScenario.vitalSigns[0] : "Assessed");
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

            CheckSequenceProgression();
        }
        else
        {
            TriggerSequenceErrorAlert("Incorrect Sequence! Please complete Respiration assessment first.");
        }
    }

    public void OnPerfusionBubbleTapped()
    {
<<<<<<< HEAD
        if (!HasActiveScenario())
=======
        // CRITICAL CHECK: Block clicks if no patient has been spawned into AR yet!
        if (_currentScenario == null)
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
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
<<<<<<< HEAD
                infoCardUI.RevealPerfusionValue(GetVitalSign(1));
=======
                infoCardUI.RevealPerfusionValue(_currentScenario.vitalSigns.Length > 1 ? _currentScenario.vitalSigns[1] : "Assessed");
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

            CheckSequenceProgression();
        }
        else
        {
            TriggerSequenceErrorAlert("Incorrect Sequence! Please complete Respiration before Perfusion.");
        }
    }

    public void OnMentalBubbleTapped()
    {
<<<<<<< HEAD
        if (!HasActiveScenario())
=======
        // CRITICAL CHECK: Block clicks if no patient has been spawned into AR yet!
        if (_currentScenario == null)
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
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
<<<<<<< HEAD
                infoCardUI.RevealMentalValue(GetVitalSign(2));
=======
                infoCardUI.RevealMentalValue(_currentScenario.vitalSigns.Length > 2 ? _currentScenario.vitalSigns[2] : "Assessed");
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

            CheckSequenceProgression();
        }
        else
        {
            TriggerSequenceErrorAlert("Incorrect Sequence! Please complete Respiration and Perfusion first.");
        }
    }

<<<<<<< HEAD
=======
    // ── INTERACTION SEQUENCE ROUTING LOGIC ──

>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
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
<<<<<<< HEAD
=======

>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
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
<<<<<<< HEAD
        RPMAssessment rpm = GetRPMAssessment();
        if (rpm == null)
        {
            Debug.LogError("No RPM assessment available!");
            return;
        }
        
=======
        RPMAssessment rpm = _currentScenario.rpmAssessment;
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
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
<<<<<<< HEAD
            if (options != null && i < options.Length)
=======
            if (i < options.Length)
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
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

<<<<<<< HEAD
=======
        // Check if answer is correct
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        bool isCorrect = selectedIndex == _correctAnswers[_quizStepIndex];
        if (isCorrect)
        {
            _correctCount++;
        }

<<<<<<< HEAD
        RPMAssessment rpm = GetRPMAssessment();
        string feedback = "";

        if (rpm != null)
        {
            switch (_quizStepIndex)
            {
                case 0:
                    feedback = rpm.respirationFeedback;
                    break;
                case 1:
                    feedback = rpm.perfusionFeedback;
                    break;
                case 2:
                    feedback = rpm.mentalStatusFeedback;
                    break;
            }
=======
        RPMAssessment rpm = _currentScenario.rpmAssessment;
        string feedback = "";

        switch (_quizStepIndex)
        {
            case 0:
                feedback = rpm.respirationFeedback;
                break;
            case 1:
                feedback = rpm.perfusionFeedback;
                break;
            case 2:
                feedback = rpm.mentalStatusFeedback;
                break;
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        }

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
<<<<<<< HEAD
            if (btn != null) btn.interactable = false;
=======
            btn.interactable = false;
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

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
<<<<<<< HEAD
=======
        // CRITICAL FIX: Send RPM results to ScoringSystem
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        if (scoringSystem != null)
        {
            scoringSystem.RecordRPMAssessment(_correctCount);
            Debug.Log($"=== RPM COMPLETE: {_correctCount}/3 correct answers added to accuracy ===");
        }
        else
        {
            Debug.LogError("ScoringSystem is NULL! RPM points not recorded!");
        }

        if (rpmPanel != null) rpmPanel.SetActive(false);
        if (uiManager != null) uiManager.SetTriageButtonsInteractable(true);

        OnRPMComplete?.Invoke();
        Debug.Log($"RPM Assessment Complete! {_correctCount}/3 quiz answers correct. Triage tags unlocked.");
    }

    public int[] GetSelectedAnswers() => _selectedAnswers;
    public int GetCorrectCount() => _correctCount;
}