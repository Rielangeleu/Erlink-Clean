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

    [Header("UI")]
    public Button submitButton;
    public GameObject ehrPanel;
    public GameObject validationAlertPanel;
    public TextMeshProUGUI validationAlertText;

    [Header("Under-Triage Alert")]
    public Image alertOverlay;
    public TextMeshProUGUI alertMessage;

    // Tracks state
    private bool _rpmComplete = false;
    private bool _tagPlaced = false;
    private bool _ehrComplete = false;
    private float _decisionStartTime;
    private TriageCategory _selectedCategory;

    void Start()
    {
        // Wire RPM completion event
        rpmController.OnRPMComplete += OnRPMSequenceComplete;

        // Wire submit button
        submitButton.onClick.AddListener(OnSubmitDecision);

        _decisionStartTime = Time.time;
    }

    void OnRPMSequenceComplete()
    {
        _rpmComplete = true;
        Debug.Log("RPM complete — triage tags now active");

        // Animate vital bubbles to show they're assessed
        uiManager.ShowRPMPanel(false);
    }

    public void OnSubmitDecision()
    {
        if (!_rpmComplete)
        {
            ShowAlert("Complete RPM assessment first!");
            return;
        }

        if (!dropZone.HasTag())
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
        ScenarioData scenario = scenarioLoader.GetActiveScenario();
        bool isCorrect = selected == scenario.correctTriageCategory;

        if (!isCorrect)
        {
            // Show under-triage / over-triage alert
            ShowValidationAlert(selected, scenario.correctTriageCategory);
        }

        // THESIS RULE 4: Show EHR panel regardless
        ShowEHRPanel();

        // Record decision for scoring
        scoringSystem.RecordTriageDecision(
            selected,
            scenario.correctTriageCategory,
            timeTaken);
    }

    void ShowValidationAlert(TriageCategory selected,
        TriageCategory correct)
    {
        if (validationAlertPanel == null) return;

        validationAlertPanel.SetActive(true);

        bool isUnderTriage =
            (int)selected > (int)correct;

        string msg = isUnderTriage
            ? $"Warning: You under-triaged this patient!\n" +
              $"You selected {selected} but the correct " +
              $"category is {correct}.\n" +
              $"This could result in delayed critical care."
            : $"You over-triaged this patient.\n" +
              $"You selected {selected} but the correct " +
              $"category is {correct}.\n" +
              $"Review the START protocol criteria.";

        validationAlertText.text = msg;

        // Flash red overlay
        if (alertOverlay != null)
        {
            alertOverlay.color = new Color(0.863f, 0.149f, 0.149f, 0f);
            alertOverlay.DOFade(0.3f, 0.2f)
                .SetLoops(2, LoopType.Yoyo);
        }
    }

    void ShowEHRPanel()
    {
        // THESIS RULE 4: Must select intervention
        if (ehrPanel != null)
        {
            ehrPanel.SetActive(true);
            ehrPanel.GetComponent<CanvasGroup>().alpha = 0;
            ehrPanel.GetComponent<CanvasGroup>()
                .DOFade(1f, 0.3f);
        }
    }

    public void OnEHRActionSelected(bool isCorrectAction)
    {
        _ehrComplete = true;

        // Record EHR score
        scoringSystem.RecordEHRAction(isCorrectAction);

        // Calculate final score
        scoringSystem.CalculateFinalScore();

        // Load feedback scene
        Invoke(nameof(LoadFeedbackScene), 1.5f);
    }

    void LoadFeedbackScene()
    {
        // Check if medium scenario has more patients
        ScenarioManager sm = FindAnyObjectByType<ScenarioManager>();

        if (sm != null && !sm.IsLastPatient())
        {
            // Load next patient in medium scenario
            sm.LoadNextPatient();
        }
        else
        {
            // All done — go to feedback
            UnityEngine.SceneManagement.SceneManager
                .LoadScene("FeedbackScene");
        }
    }

    void ShowAlert(string message)
    {
        Debug.LogWarning(message);
        // Hook to UI alert toast in next step
    }

    void OnDestroy()
    {
        if (rpmController != null)
            rpmController.OnRPMComplete -= OnRPMSequenceComplete;
    }
}