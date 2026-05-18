using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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

    // ── FIX: Direct reference bypasses Unity finding inactive object limit ──
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
    private float _decisionStartTime;
    private TriageCategory _selectedCategory;

    void Start()
    {
        // Wire RPM completion event
        if (rpmController != null)
            rpmController.OnRPMComplete += OnRPMSequenceComplete;

        // Wire submit button
        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitDecision);

        _decisionStartTime = Time.time;
    }

    void OnRPMSequenceComplete()
    {
        _rpmComplete = true;
        Debug.Log("RPM complete — triage tags now active");

        // Animate vital bubbles to show they're assessed
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

        // THESIS RULE 3: Validate against START protocol
        ValidateDecision(_selectedCategory, timeTaken);
    }

    void ValidateDecision(TriageCategory selected, float timeTaken)
    {
        if (scenarioLoader == null || scoringSystem == null) return;

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

        // Flash red screen overlay canvas bounding frames
        if (alertOverlay != null)
        {
            alertOverlay.DOKill();
            alertOverlay.color = new Color(0.863f, 0.149f, 0.149f, 0f);
            alertOverlay.DOFade(0.3f, 0.2f).SetLoops(2, LoopType.Yoyo);
        }
    }

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

    void HandleScenarioTransition()
    {
        ScenarioManager sm = FindAnyObjectByType<ScenarioManager>();

        // Check if there are more patients left to evaluate in multi-victim configurations
        if (sm != null && !sm.IsLastPatient())
        {
            // Reset local verification state flags before shifting to Patient B
            ResetManagerStates();

            // Load next patient via active spawner structures
            sm.LoadNextPatient();
        }
        else
        {
            Debug.Log("All scenario targets completed successfully! Routing to FeedbackScene.");

            // Reset local state metrics so tracking bounds start fresh next session
            ResetManagerStates();

            // All targets completed — jump scenes cleanly
            UnityEngine.SceneManagement.SceneManager.LoadScene("FeedbackScene");
        }
    }

    public void ResetManagerStates()
    {
        _rpmComplete = false;
        _tagPlaced = false;
        _ehrComplete = false;
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
    }

    void ShowAlert(string message)
    {
        Debug.LogWarning(message);

        // On-screen toast fallback notification popups for physical phone validation
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
}