using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

/// <summary>
/// Controls the EHR intervention panel.
/// Thesis Rule 4: Student must select correct intervention
/// after triage tagging before session ends.
/// Appropriate Action = 1 point in accuracy scoring.
/// </summary>
public class EHRPanelUI : MonoBehaviour
{
    [Header("UI References - EHR Panel")]
    public Button[] ehrOptionButtons;
    public TextMeshProUGUI[] ehrOptionTexts;
    public GameObject ehrFeedbackPanel;
    public TextMeshProUGUI ehrFeedbackText;

    [Header("UI References - Priority Panel (Separate Panel)")]
    public GameObject priorityPanel;
    public TextMeshProUGUI priorityQuestionText;
    public Button[] priorityButtons;
    public TextMeshProUGUI[] priorityButtonTexts;
    public GameObject priorityFeedbackPanel;
    public TextMeshProUGUI priorityFeedbackText;

    [Header("Dependencies")]
    public TriageDecisionManager decisionManager;
    public ScenarioLoader scenarioLoader;
    public ScoringSystem scoringSystem;

    [Header("Colors")]
    public Color defaultColor = new Color(0.953f, 0.953f, 0.961f);
    public Color correctColor = new Color(0.086f, 0.635f, 0.290f);
    public Color wrongColor = new Color(0.863f, 0.149f, 0.149f);

    // State tracking
    private bool _ehrAnswered = false;
    private bool _ehrIsCorrect = false;
    private int _selectedEHRIndex = -1;
    
    // Priority state
    private bool _prioritySelected = false;
    private bool _priorityCorrect = false;
    private string[] _cachedPriorityOptions;
    private int _cachedCorrectPriorityIndex = 0;
    private ScenarioData _cachedScenario;
    private bool _isMultiPatient = false;
    private bool _isProcessing = false;
    private Coroutine _currentPriorityCoroutine;

    void Start()
    {
        if (scoringSystem == null)
            scoringSystem = FindFirstObjectByType<ScoringSystem>();
        if (scenarioLoader == null)
            scenarioLoader = FindFirstObjectByType<ScenarioLoader>();
        if (decisionManager == null)
            decisionManager = FindFirstObjectByType<TriageDecisionManager>();
        
        WireAllButtons();
    }

    private void WireAllButtons()
    {
        for (int i = 0; i < priorityButtons.Length; i++)
        {
            if (priorityButtons[i] != null)
            {
                int index = i;
                priorityButtons[i].onClick.RemoveAllListeners();
                priorityButtons[i].onClick.AddListener(() => OnPrioritySelected(index));
                Debug.Log($"Wired Priority Button {i}");
            }
        }
        
        for (int i = 0; i < ehrOptionButtons.Length; i++)
        {
            if (ehrOptionButtons[i] != null)
            {
                int index = i;
                ehrOptionButtons[i].onClick.RemoveAllListeners();
                ehrOptionButtons[i].onClick.AddListener(() => OnEHROptionSelected(index));
                Debug.Log($"Wired EHR Button {i}");
            }
        }
    }

    public void PopulateEHRActions(ScenarioData scenario)
    {
        if (scenario == null || scenario.ehrActions == null)
        {
            Debug.LogError("EHRPanelUI: Scenario or ehrActions is null!");
            return;
        }

        _cachedScenario = scenario;
        _ehrAnswered = false;
        _ehrIsCorrect = false;
        _selectedEHRIndex = -1;
        _prioritySelected = false;
        _priorityCorrect = false;
        _isProcessing = false;

        if (_currentPriorityCoroutine != null)
            StopCoroutine(_currentPriorityCoroutine);

        if (ehrFeedbackPanel != null)
            ehrFeedbackPanel.SetActive(false);
        if (priorityFeedbackPanel != null)
            priorityFeedbackPanel.SetActive(false);

        _isMultiPatient = (scenario.isMultiPatient || 
                           (scenario.additionalPatients != null && scenario.additionalPatients.Length > 0) ||
                           scenario.difficulty == DifficultyLevel.Medium ||
                           scenario.difficulty == DifficultyLevel.Hard);

        // Populate EHR Actions
        for (int i = 0; i < ehrOptionButtons.Length; i++)
        {
            if (i < scenario.ehrActions.Length)
            {
                ehrOptionButtons[i].gameObject.SetActive(true);
                ehrOptionButtons[i].interactable = true;
                var img = ehrOptionButtons[i].GetComponent<Image>();
                if (img != null) img.color = defaultColor;
                ehrOptionTexts[i].text = scenario.ehrActions[i].actionName;
                ehrOptionTexts[i].color = new Color(0.12f, 0.16f, 0.23f);
            }
            else
            {
                ehrOptionButtons[i].gameObject.SetActive(false);
            }
        }
        
        // Cache priority data for later use
        _cachedPriorityOptions = scenario.priorityOptions;
        _cachedCorrectPriorityIndex = scenario.correctPriorityIndex;
        
        Debug.Log($"=== PopulateEHRActions Debug ===");
        Debug.Log($"Scenario: {scenario.scenarioTitle}");
        Debug.Log($"Difficulty: {scenario.difficulty}");
        Debug.Log($"isMultiPatient: {scenario.isMultiPatient}");
        Debug.Log($"Priority Options array length: {(scenario.priorityOptions != null ? scenario.priorityOptions.Length : 0)}");
        Debug.Log($"Correct Priority Index: {_cachedCorrectPriorityIndex}");
        
        if (scenario.priorityOptions != null)
        {
            for (int i = 0; i < scenario.priorityOptions.Length; i++)
            {
                Debug.Log($"  Priority Option {i}: '{scenario.priorityOptions[i]}'");
            }
        }
        Debug.Log($"=== End Debug ===");
    }

    public void PopulatePriorityPanel(ScenarioData scenario)
    {
        if (scenario == null)
        {
            Debug.LogError("EHRPanelUI: Cannot populate priority panel - scenario is null!");
            return;
        }
        
        Debug.Log($"=== PopulatePriorityPanel START ===");
        
        // Cancel any ongoing coroutine
        if (_currentPriorityCoroutine != null)
            StopCoroutine(_currentPriorityCoroutine);
        
        _cachedScenario = scenario;
        _prioritySelected = false;
        _priorityCorrect = false;
        _isProcessing = false;
        _cachedPriorityOptions = scenario.priorityOptions;
        _cachedCorrectPriorityIndex = scenario.correctPriorityIndex;
        
        Debug.Log($"Cached priority index: {_cachedCorrectPriorityIndex}");
        Debug.Log($"Priority Options array length: {(scenario.priorityOptions != null ? scenario.priorityOptions.Length : 0)}");
        
        if (scenario.priorityOptions != null)
        {
            for (int i = 0; i < scenario.priorityOptions.Length; i++)
            {
                Debug.Log($"  Priority Option {i}: '{scenario.priorityOptions[i]}'");
            }
        }
        
        // Set question text
        if (priorityQuestionText != null)
            priorityQuestionText.text = scenario.priorityQuestion;
        
        // Get number of options from ScriptableObject
        int numOptions = 0;
        if (scenario.priorityOptions != null && scenario.priorityOptions.Length > 0)
        {
            numOptions = scenario.priorityOptions.Length;
            Debug.Log($"Using {numOptions} options from ScriptableObject");
        }
        else
        {
            int patientCount = 1;
            if (scenario.additionalPatients != null && scenario.additionalPatients.Length > 0)
                patientCount = scenario.additionalPatients.Length + 1;
            else if (scenario.isMultiPatient)
                patientCount = 2;
            numOptions = patientCount;
            Debug.Log($"Using fallback {numOptions} options based on patient count");
        }
        
        // IMPORTANT: Force hide ALL buttons first
        for (int i = 0; i < priorityButtons.Length; i++)
        {
            if (priorityButtons[i] != null)
            {
                priorityButtons[i].gameObject.SetActive(false);
                Debug.Log($"Hiding button {i}");
            }
        }
        
        // Now show and configure only the buttons we need
        for (int i = 0; i < numOptions; i++)
        {
            if (priorityButtons[i] != null)
            {
                priorityButtons[i].gameObject.SetActive(true);
                Debug.Log($"Showing button {i}");
                
                // Try to get TextMeshProUGUI from button if not assigned
                TextMeshProUGUI buttonText = priorityButtonTexts[i];
                if (buttonText == null && priorityButtons[i] != null)
                {
                    buttonText = priorityButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                    Debug.Log($"Auto-found TextMeshPro on button {i}");
                }
                
                if (buttonText != null)
                {
                    if (scenario.priorityOptions != null && i < scenario.priorityOptions.Length && !string.IsNullOrEmpty(scenario.priorityOptions[i]))
                    {
                        buttonText.text = scenario.priorityOptions[i];
                        Debug.Log($"Button {i} text set to: '{scenario.priorityOptions[i]}'");
                    }
                    else
                    {
                        string defaultText = GetDefaultPriorityOption(i);
                        buttonText.text = defaultText;
                        Debug.Log($"Button {i} using default text: '{defaultText}'");
                    }
                }
                else
                {
                    Debug.LogError($"Button {i} has no TextMeshProUGUI component!");
                }
                
                var img = priorityButtons[i].GetComponent<Image>();
                if (img != null) img.color = defaultColor;
                priorityButtons[i].interactable = true;
            }
        }
        
        if (priorityFeedbackPanel != null)
            priorityFeedbackPanel.SetActive(false);
        
        // Activate the priority panel
        if (priorityPanel != null)
        {
            priorityPanel.SetActive(true);
            Debug.Log($"Priority panel ACTIVATED - Active: {priorityPanel.activeSelf}");
        }
        
        Debug.Log($"=== PopulatePriorityPanel END ===");
    }

    private string GetDefaultPriorityOption(int index)
    {
        switch (index)
        {
            case 0: return "Patient A should be treated FIRST";
            case 1: return "Patient B should be treated FIRST";
            case 2: return "Patient C should be treated FIRST";
            default: return $"Patient {(char)('A' + index)} should be treated FIRST";
        }
    }

    public void OnPrioritySelected(int priorityIndex)
    {
        Debug.Log($"=== OnPrioritySelected called with index: {priorityIndex} ===");
        
        if (_prioritySelected || _isProcessing)
        {
            Debug.Log("Priority already selected or processing, ignoring");
            return;
        }
        
        if (priorityPanel == null || !priorityPanel.activeSelf)
        {
            Debug.Log($"Priority panel not active - cannot select");
            return;
        }
        
        _isProcessing = true;
        _prioritySelected = true;
        
        // Check if correct
        bool isCorrect = (priorityIndex == _cachedCorrectPriorityIndex);
        _priorityCorrect = isCorrect;
        
        Debug.Log($"Selected: {priorityIndex}, Correct index: {_cachedCorrectPriorityIndex}, Result: {(isCorrect ? "CORRECT" : "INCORRECT")}");
        
        Color resultColor = isCorrect ? correctColor : wrongColor;
        
        // Animate and color selected button
        if (priorityButtons.Length > priorityIndex && priorityButtons[priorityIndex] != null)
        {
            var btnImage = priorityButtons[priorityIndex].GetComponent<Image>();
            if (btnImage != null)
            {
                btnImage.DOKill();
                btnImage.DOColor(resultColor, 0.3f);
            }
            
            var btnRt = priorityButtons[priorityIndex].GetComponent<RectTransform>();
            if (btnRt != null)
            {
                btnRt.DOKill();
                btnRt.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.3f);
            }
        }
        
        // Disable all priority buttons
        foreach (var btn in priorityButtons)
        {
            if (btn != null) btn.interactable = false;
        }
        
        // Show feedback
        if (priorityFeedbackPanel != null && priorityFeedbackText != null)
        {
            priorityFeedbackPanel.SetActive(true);
            string feedback = isCorrect 
                ? "✓ Correct! You selected the correct priority order." 
                : $"✗ Incorrect. The correct answer is: {GetCorrectAnswerText()}";
            priorityFeedbackText.text = feedback;
            priorityFeedbackText.color = resultColor;
            Debug.Log($"Feedback shown: {feedback}");
        }
        
        // Record score
        if (scoringSystem != null)
        {
            scoringSystem.RecordPriorityOrder(isCorrect);
        }
        
        // Start coroutine to hide panel
        if (_currentPriorityCoroutine != null)
            StopCoroutine(_currentPriorityCoroutine);
        _currentPriorityCoroutine = StartCoroutine(HidePriorityAndComplete());
    }
    
    private string GetCorrectAnswerText()
    {
        if (_cachedPriorityOptions != null && _cachedCorrectPriorityIndex < _cachedPriorityOptions.Length)
        {
            return _cachedPriorityOptions[_cachedCorrectPriorityIndex];
        }
        return $"Option {_cachedCorrectPriorityIndex + 1}";
    }
    
    private IEnumerator HidePriorityAndComplete()
    {
        yield return new WaitForSeconds(1.5f);
        
        Debug.Log("Hiding priority panel...");
        
        // Force hide priority panel and all its buttons
        if (priorityPanel != null)
        {
            priorityPanel.SetActive(false);
            Debug.Log("Priority panel set to inactive");
        }
        
        // Also force hide all buttons individually
        for (int i = 0; i < priorityButtons.Length; i++)
        {
            if (priorityButtons[i] != null)
            {
                priorityButtons[i].gameObject.SetActive(false);
            }
        }
        
        // Notify decision manager
        if (decisionManager != null)
        {
            Debug.Log($"Notifying decision manager with result: {_priorityCorrect}");
            decisionManager.OnPrioritySelected(_priorityCorrect);
        }
        
        _isProcessing = false;
        _currentPriorityCoroutine = null;
    }

    public void OnEHROptionSelected(int index)
    {
        Debug.Log($"=== OnEHROptionSelected called with index: {index} ===");
        
        if (_ehrAnswered) return;
        
        ScenarioData scenario = _cachedScenario ?? scenarioLoader?.GetActiveScenario();
        
        if (scenario == null || scenario.ehrActions == null || index >= scenario.ehrActions.Length)
        {
            Debug.LogError("EHRPanelUI: Cannot get scenario or EHR actions!");
            return;
        }
        
        _ehrAnswered = true;
        _selectedEHRIndex = index;

        bool isCorrect = scenario.ehrActions[index].isCorrectAction;
        _ehrIsCorrect = isCorrect;

        Color resultColor = isCorrect ? correctColor : wrongColor;

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

        if (ehrFeedbackPanel != null && ehrFeedbackText != null)
        {
            ehrFeedbackPanel.SetActive(true);
            ehrFeedbackText.text = isCorrect
                ? $"✓ Correct! {scenario.ehrActions[index].actionDescription}"
                : $"✗ Incorrect. {scenario.ehrActions[index].actionDescription}";
            ehrFeedbackText.color = resultColor;
        }

        foreach (var btn in ehrOptionButtons)
            if (btn != null) btn.interactable = false;

        if (scoringSystem != null)
        {
            scoringSystem.RecordEHRAction(isCorrect);
        }

        StartCoroutine(HideEHRAndNotify());
    }

    private IEnumerator HideEHRAndNotify()
    {
        yield return new WaitForSeconds(1.5f);
        
        if (gameObject != null)
            gameObject.SetActive(false);
        
        if (decisionManager != null)
            decisionManager.OnEHRActionSelected(_ehrIsCorrect);
    }

    public void ResetPanel()
    {
        Debug.Log("EHRPanelUI.ResetPanel called");
        
        if (_currentPriorityCoroutine != null)
            StopCoroutine(_currentPriorityCoroutine);
        
        _ehrAnswered = false;
        _ehrIsCorrect = false;
        _selectedEHRIndex = -1;
        _prioritySelected = false;
        _priorityCorrect = false;
        _isProcessing = false;
        _cachedScenario = null;
        _cachedPriorityOptions = null;
        _cachedCorrectPriorityIndex = 0;
        _isMultiPatient = false;

        if (ehrFeedbackPanel != null)
            ehrFeedbackPanel.SetActive(false);
        
        if (priorityFeedbackPanel != null)
            priorityFeedbackPanel.SetActive(false);
        
        if (priorityPanel != null)
            priorityPanel.SetActive(false);

        // Hide all priority buttons
        for (int i = 0; i < priorityButtons.Length; i++)
        {
            if (priorityButtons[i] != null)
            {
                priorityButtons[i].gameObject.SetActive(false);
                priorityButtons[i].interactable = true;
                var img = priorityButtons[i].GetComponent<Image>();
                if (img != null) img.color = defaultColor;
            }
        }

        for (int i = 0; i < ehrOptionButtons.Length; i++)
        {
            if (ehrOptionButtons[i] != null)
            {
                ehrOptionButtons[i].interactable = true;
                var img = ehrOptionButtons[i].GetComponent<Image>();
                if (img != null) img.color = defaultColor;
                
                if (i < ehrOptionTexts.Length && ehrOptionTexts[i] != null)
                {
                    ehrOptionTexts[i].color = new Color(0.12f, 0.16f, 0.23f);
                }
            }
        }
    }
}