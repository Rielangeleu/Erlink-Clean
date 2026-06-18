using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
<<<<<<< HEAD
using System.Collections.Generic;
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

/// <summary>
/// Manages the complete triage decision flow.
/// Enforces ALL 4 thesis rules:
/// 1. Sequential RPM
/// 2. Time pressure
/// 3. Validation against START protocol
/// 4. EHR documentation required
/// </summary>
public class TriageDecisionManager : MonoBehaviour
{
    [Header("Dependencies")]
    public DropZoneController dropZone;
    public RPMSequenceController rpmController;
    public UIManager uiManager;
    public ScenarioLoader scenarioLoader;
    public ScoringSystem scoringSystem;

    [Header("UI Panels")]
    public Button submitButton;
    public GameObject ehrPanel;
<<<<<<< HEAD
    public GameObject priorityPanel;

=======

    // ── FIX: Direct reference bypasses Unity finding inactive object limit ──
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    public EHRPanelUI ehrPanelUI;

    public GameObject validationAlertPanel;
    public TextMeshProUGUI validationAlertText;

    [Header("Under-Triage Alert")]
    public Image alertOverlay;
    public TextMeshProUGUI alertMessage;

    // Tracks internal state states
    private bool _rpmComplete = false;
    private bool _tagPlaced = false;
    private bool _ehrComplete = false;
<<<<<<< HEAD
    private bool _priorityComplete = false;
    private float _decisionStartTime;
    private TriageCategory _selectedCategory;
    private ScenarioData _currentScenario;
    
    // Track how many patients have completed EHR
    private int _completedPatientsCount = 0;
    private int _totalPatients = 1;
    private bool _isMultiPatient = false;
    private string _currentPatientName = "Patient";

    void Start()
    {
        if (rpmController != null)
            rpmController.OnRPMComplete += OnRPMSequenceComplete;

=======
    private float _decisionStartTime;
    private TriageCategory _selectedCategory;

    void Start()
    {
        // Wire RPM completion event
        if (rpmController != null)
            rpmController.OnRPMComplete += OnRPMSequenceComplete;

        // Wire submit button
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitDecision);

        _decisionStartTime = Time.time;
<<<<<<< HEAD
        
        PatientAssessmentTracker.ClearAssessments();
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    void OnRPMSequenceComplete()
    {
        _rpmComplete = true;
        Debug.Log("RPM complete — triage tags now active");

<<<<<<< HEAD
=======
        // Animate vital bubbles to show they're assessed
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        if (uiManager != null)
            uiManager.ShowRPMPanel(false);
    }

    public void OnSubmitDecision()
    {
        if (!_rpmComplete)
        {
            ShowAlert("Complete RPM assessment first!");
            return;
        }

        if (dropZone == null || !dropZone.HasTag())
        {
            ShowAlert("Drag a triage tag to the patient first!");
            return;
        }

        _selectedCategory = dropZone.GetSelectedCategory();
        float timeTaken = Time.time - _decisionStartTime;

<<<<<<< HEAD
=======
        // THESIS RULE 3: Validate against START protocol
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        ValidateDecision(_selectedCategory, timeTaken);
    }

    void ValidateDecision(TriageCategory selected, float timeTaken)
    {
        if (scenarioLoader == null || scoringSystem == null) return;

<<<<<<< HEAD
        _currentScenario = scenarioLoader.GetActiveScenario();
        
        if (_currentScenario == null)
        {
            var runtimeScenario = scenarioLoader.GetActiveRuntimeScenario();
            if (runtimeScenario != null)
            {
                _currentScenario = ConvertRuntimeToScriptable(runtimeScenario);
            }
        }
        
        if (_currentScenario == null) return;

        bool isCorrect = selected == _currentScenario.correctTriageCategory;

        if (!isCorrect)
        {
            ShowValidationAlert(selected, _currentScenario.correctTriageCategory);
        }

        scoringSystem.RecordTriageDecision(selected, _currentScenario.correctTriageCategory, timeTaken);
        
        _currentPatientName = GetCurrentPatientName();
        PatientAssessmentTracker.AddAssessment(new PatientAssessmentData
        {
            patientName = _currentPatientName,
            selectedCategory = selected,
            correctCategory = _currentScenario.correctTriageCategory,
            isCorrect = isCorrect
        });
        Debug.Log($"Recorded assessment for {_currentPatientName}: {(isCorrect ? "CORRECT" : "INCORRECT")}");
        
        _isMultiPatient = (_currentScenario.isMultiPatient || 
                           (_currentScenario.additionalPatients != null && _currentScenario.additionalPatients.Length > 0) ||
                           _currentScenario.difficulty == DifficultyLevel.Medium ||
                           _currentScenario.difficulty == DifficultyLevel.Hard);
        
        _totalPatients = 1;
        if (_currentScenario.additionalPatients != null)
            _totalPatients = _currentScenario.additionalPatients.Length + 1;
        else if (_currentScenario.isMultiPatient)
            _totalPatients = 2;
        
        ShowEHRPanel();
    }

    private string GetCurrentPatientName()
    {
        ScenarioManager sm = FindAnyObjectByType<ScenarioManager>();
        if (sm != null && sm.GetActiveDifficulty() == DifficultyLevel.Medium)
        {
            int patientIndex = sm.GetCurrentPatientIndex();
            if (patientIndex == 0) return "Patient A";
            if (patientIndex == 1) return "Patient B";
            if (patientIndex == 2) return "Patient C";
        }
        return "Patient";
    }

    void ShowEHRPanel()
    {
        if (ehrPanel != null)
        {
            ehrPanel.SetActive(true);
            
            if (ehrPanelUI != null && _currentScenario != null)
            {
                ehrPanelUI.PopulateEHRActions(_currentScenario);
                Debug.Log($"✅ EHR Panel populated for: {_currentScenario.scenarioTitle}");
            }
            
            CanvasGroup cg = ehrPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.DOKill();
                cg.alpha = 0;
                cg.DOFade(1f, 0.3f);
            }
        }
    }

    void ShowPriorityPanel()
    {
        if (priorityPanel != null)
        {
            priorityPanel.SetActive(true);
            
            if (ehrPanelUI != null && _currentScenario != null)
            {
                ehrPanelUI.PopulatePriorityPanel(_currentScenario);
                Debug.Log($"✅ Priority Panel populated for: {_currentScenario.scenarioTitle}");
            }
            
            CanvasGroup cg = priorityPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.DOKill();
                cg.alpha = 0;
                cg.DOFade(1f, 0.3f);
            }
        }
    }

    public void OnEHRActionSelected(bool isCorrectAction)
    {
        _ehrComplete = true;

        if (scoringSystem != null)
        {
            scoringSystem.RecordEHRAction(isCorrectAction);
            Debug.Log($"EHR Action Selected for {_currentPatientName}: {(isCorrectAction ? "Correct" : "Incorrect")}");
        }

        if (ehrPanel != null)
            ehrPanel.SetActive(false);
        
        if (validationAlertPanel != null)
            validationAlertPanel.SetActive(false);
        
        _completedPatientsCount++;
        
        Debug.Log($"Patient {_completedPatientsCount}/{_totalPatients} completed EHR - {_currentPatientName}");
        
        if (_isMultiPatient && _completedPatientsCount < _totalPatients)
        {
            Invoke(nameof(ResetForNextPatient), 0.5f);
        }
        else if (_isMultiPatient && _completedPatientsCount >= _totalPatients)
        {
            Debug.Log($"All {_totalPatients} patients assessed - Total assessments: {PatientAssessmentTracker.GetAssessments().Count}");
            Invoke(nameof(ShowPriorityPanel), 0.5f);
        }
        else
        {
            FinalizeAndComplete();
        }
    }

    void ResetForNextPatient()
    {
        Debug.Log($"=== RESETTING FOR NEXT PATIENT (Completed: {_completedPatientsCount}/{_totalPatients}) ===");
        
        _ehrComplete = false;
        _rpmComplete = false;
        _tagPlaced = false;
        _priorityComplete = false;
        _decisionStartTime = Time.time;
        
        if (ehrPanel != null)
            ehrPanel.SetActive(false);
        
        if (priorityPanel != null)
            priorityPanel.SetActive(false);
        
        if (dropZone != null)
            dropZone.ResetDropZone();
        
        if (ehrPanelUI != null)
            ehrPanelUI.ResetPanel();
        
        ObjectSpawnerBridge bridge = FindFirstObjectByType<ObjectSpawnerBridge>();
        if (bridge != null)
            bridge.ResetPlacement();
        
        if (uiManager != null)
        {
            uiManager.ShowRPMPanel(false);
            uiManager.SetTriageTagsInteractable(false);
            uiManager.HideCodeAlert();
            uiManager.ShowEHRPanel(false);
            uiManager.ShowPlacementPrompt(true);
        }
        
        ScenarioManager sm = FindAnyObjectByType<ScenarioManager>();
        if (sm != null)
        {
            Debug.Log("Loading next patient...");
            sm.LoadNextPatient();
        }
        else
        {
            Debug.LogError("ScenarioManager not found!");
        }
    }

    public void OnPrioritySelected(bool isCorrect)
    {
        Debug.Log($"=== OnPrioritySelected called, isCorrect: {isCorrect} ===");
        
        _priorityComplete = true;
        
        if (priorityPanel != null)
            priorityPanel.SetActive(false);
        if (validationAlertPanel != null)
            validationAlertPanel.SetActive(false);
        
        FinalizeAndComplete();
    }
    
    void FinalizeAndComplete()
    {
        if (scoringSystem != null)
        {
            scoringSystem.CalculateFinalScore();
        }
        
        HandleScenarioTransition();
=======
        ScenarioData scenario = scenarioLoader.GetActiveScenario();
        if (scenario == null) return;

        bool isCorrect = selected == scenario.correctTriageCategory;

        if (!isCorrect)
        {
            // Show under-triage / over-triage alert overlay canvas
            ShowValidationAlert(selected, scenario.correctTriageCategory);
        }

        // THESIS RULE 4: Show EHR panel regardless
        ShowEHRPanel();

        // Record decision for scoring matrix metrics
        scoringSystem.RecordTriageDecision(
            selected,
            scenario.correctTriageCategory,
            timeTaken);
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    void ShowValidationAlert(TriageCategory selected, TriageCategory correct)
    {
        if (validationAlertPanel == null || validationAlertText == null) return;

        validationAlertPanel.SetActive(true);

        bool isUnderTriage = (int)selected > (int)correct;

        string msg = isUnderTriage
            ? $"Warning: You under-triaged this patient!\nYou selected {selected} but the correct category is {correct}.\nThis could result in delayed critical care."
            : $"You over-triaged this patient.\nYou selected {selected} but the correct category is {correct}.\nReview the START protocol criteria.";

        validationAlertText.text = msg;

<<<<<<< HEAD
=======
        // Flash red screen overlay canvas bounding frames
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        if (alertOverlay != null)
        {
            alertOverlay.DOKill();
            alertOverlay.color = new Color(0.863f, 0.149f, 0.149f, 0f);
            alertOverlay.DOFade(0.3f, 0.2f).SetLoops(2, LoopType.Yoyo);
        }
    }

<<<<<<< HEAD
=======
    void ShowEHRPanel()
    {
        // THESIS RULE 4: Must select medical intervention
        if (ehrPanel != null)
        {
            ehrPanel.SetActive(true);
            CanvasGroup cg = ehrPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.DOKill();
                cg.alpha = 0;
                cg.DOFade(1f, 0.3f);
            }
        }
    }

    public void OnEHRActionSelected(bool isCorrectAction)
    {
        _ehrComplete = true;

        // Record EHR evaluation score choice arrays
        if (scoringSystem != null)
        {
            scoringSystem.RecordEHRAction(isCorrectAction);
            scoringSystem.CalculateFinalScore();
        }

        // Hide panels prior to executing state transitions
        if (ehrPanel != null) ehrPanel.SetActive(false);
        if (validationAlertPanel != null) validationAlertPanel.SetActive(false);

        HandleScenarioTransition();
    }

>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    void HandleScenarioTransition()
    {
        ScenarioManager sm = FindAnyObjectByType<ScenarioManager>();

<<<<<<< HEAD
        if (sm != null && !sm.IsLastPatient() && !_isMultiPatient)
        {
            ResetManagerStates();
=======
        // Check if there are more patients left to evaluate in multi-victim configurations
        if (sm != null && !sm.IsLastPatient())
        {
            // Reset local verification state flags before shifting to Patient B
            ResetManagerStates();

            // Load next patient via active spawner structures
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
            sm.LoadNextPatient();
        }
        else
        {
            Debug.Log("All scenario targets completed successfully! Routing to FeedbackScene.");
<<<<<<< HEAD
            ResetManagerStates();
=======

            // Reset local state metrics so tracking bounds start fresh next session
            ResetManagerStates();

            // All targets completed — jump scenes cleanly
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
            UnityEngine.SceneManagement.SceneManager.LoadScene("FeedbackScene");
        }
    }

    public void ResetManagerStates()
    {
        _rpmComplete = false;
        _tagPlaced = false;
        _ehrComplete = false;
<<<<<<< HEAD
        _priorityComplete = false;
        _completedPatientsCount = 0;
        _decisionStartTime = Time.time;
        _currentScenario = null;

        if (dropZone != null)
            dropZone.ResetDropZone();

        if (ehrPanel != null) ehrPanel.SetActive(false);
        if (priorityPanel != null) priorityPanel.SetActive(false);
        
        if (ehrPanelUI != null)
            ehrPanelUI.ResetPanel();
    }

    private ScenarioData ConvertRuntimeToScriptable(RuntimeScenarioData runtime)
    {
        if (runtime == null) return null;
        
        var temp = ScriptableObject.CreateInstance<ScenarioData>();
        temp.scenarioTitle = runtime.scenarioTitle;
        temp.scenarioID = runtime.scenarioID;
        temp.difficulty = runtime.difficulty;
        temp.isMultiPatient = runtime.isMultiPatient;
        temp.timeLimitSeconds = runtime.timeLimitSeconds;
        temp.patientAge = runtime.patientAge;
        temp.patientGender = runtime.patientGender;
        temp.patientPresentation = runtime.patientPresentation;
        temp.vitalSigns = runtime.vitalSigns;
        temp.rpmAssessment = runtime.rpmAssessment;
        temp.correctTriageCategory = runtime.correctTriageCategory;
        temp.ehrActions = runtime.ehrActions;
        temp.clinicalExplanation = runtime.clinicalExplanation;
        temp.priorityQuestion = runtime.priorityQuestion;
        temp.priorityOptions = runtime.priorityOptions;
        temp.correctPriorityIndex = runtime.correctPriorityIndex;
        temp.patientPrefab = runtime.patientPrefab;
        
        return temp;
=======
        _decisionStartTime = Time.time;

        // Clear active drop center properties
        if (dropZone != null)
        {
            dropZone.ResetDropZone();
        }

        // ── FIX: Force EHR panel reset via explicit direct reference link ──
        if (ehrPanelUI != null && scenarioLoader != null)
        {
            ScenarioData nextScenario = scenarioLoader.GetActiveScenario();
            if (nextScenario != null)
            {
                ehrPanelUI.PopulateEHRActions(nextScenario);
                Debug.Log($"EHR Panel cleanly refreshed for incoming scenario: {nextScenario.scenarioTitle} ✅");
            }
        }
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    void ShowAlert(string message)
    {
        Debug.LogWarning(message);

<<<<<<< HEAD
=======
        // On-screen toast fallback notification popups for physical phone validation
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        if (validationAlertPanel != null && validationAlertText != null)
        {
            validationAlertPanel.SetActive(true);
            validationAlertText.text = $"⚠️ Assessment Incomplete\n\n{message}";

            CancelInvoke(nameof(HideValidationAlert));
            Invoke(nameof(HideValidationAlert), 2.2f);
        }
    }

    void HideValidationAlert()
    {
        if (validationAlertPanel != null)
            validationAlertPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (rpmController != null)
            rpmController.OnRPMComplete -= OnRPMSequenceComplete;
    }
<<<<<<< HEAD
}

public static class PatientAssessmentTracker
{
    private static System.Collections.Generic.List<PatientAssessmentData> _assessments = new System.Collections.Generic.List<PatientAssessmentData>();
    
    public static void AddAssessment(PatientAssessmentData assessment)
    {
        _assessments.Add(assessment);
        Debug.Log($"✅ Added assessment for {assessment.patientName}. Total: {_assessments.Count}");
        
        foreach (var a in _assessments)
        {
            Debug.Log($"  📋 {a.patientName}: {(a.isCorrect ? "CORRECT" : "INCORRECT")}");
        }
    }
    
    public static System.Collections.Generic.List<PatientAssessmentData> GetAssessments()
    {
        return new System.Collections.Generic.List<PatientAssessmentData>(_assessments);
    }
    
    public static void ClearAssessments()
    {
        _assessments.Clear();
        Debug.Log("Cleared all patient assessments");
    }
}

[System.Serializable]
public class PatientAssessmentData
{
    public string patientName;
    public TriageCategory selectedCategory;
    public TriageCategory correctCategory;
    public bool isCorrect;
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
}