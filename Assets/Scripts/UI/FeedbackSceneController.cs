using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
<<<<<<< HEAD
using System.Collections.Generic;
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

public class FeedbackSceneController : MonoBehaviour
{
    [Header("Score Circle")]
    public TextMeshProUGUI scoreText;
    public Image scoreCircleFill;
    public Image scoreCircleBorder;
    public TextMeshProUGUI performanceTitle;
    public TextMeshProUGUI performanceSubtitle;

    [Header("Accuracy Card")]
    public TextMeshProUGUI accuracyValue;
    public TextMeshProUGUI accuracyLabel;
    public Image accuracyIconBg;

    [Header("Speed Card")]
    public TextMeshProUGUI speedValue;
    public TextMeshProUGUI speedLabel;
    public Image speedIconBg;

    [Header("Confidence Card")]
    public TextMeshProUGUI confidenceValue;
    public TextMeshProUGUI confidenceLabel;
    public Image confidenceIconBg;

<<<<<<< HEAD
    [Header("Triage Assessment Panel")]
    public GameObject triageAssessmentContainer;
    public Transform triageAssessmentParent;
    public GameObject triageAssessmentCardPrefab;

    [Header("Explanation")]
    public TextMeshProUGUI explanationText;
    public ScrollRect explanationScrollRect;
=======
    [Header("Triage Comparison Panel")]
    public GameObject triageAssessmentContainer;
    public Image yourSelectionPill;
    public TextMeshProUGUI yourSelectionPillText;
    public Image correctTriagePill;
    public TextMeshProUGUI correctTriagePillText;

    [Header("Explanation")]
    public TextMeshProUGUI explanationText;
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

    [Header("Buttons")]
    public Button nextScenarioButton;
    public Button dashboardButton;

<<<<<<< HEAD
    // Store multiple patient results
    private List<PatientAssessmentData> _patientAssessments = new List<PatientAssessmentData>();
    
    // Track available scenarios
    private List<ScenarioData> _availableScenarios = new List<ScenarioData>();
    private List<ScenarioData[]> _availableMediumPairs = new List<ScenarioData[]>();
    private int _currentScenarioIndex = 0;
    private DifficultyLevel _currentDifficulty;

    void Start()
    {
        LoadPatientAssessments();
        
=======
    void Start()
    {
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        ScoringResult result = ScoringSystem.LastResult;

        if (result == null || result.timeTaken == 0)
        {
            ShowPlaceholder();
            WireButtons();
            return;
        }

<<<<<<< HEAD
        _currentDifficulty = ScoringSystem.LastScenario?.difficulty ?? DifficultyLevel.Easy;
        LoadAvailableScenarios();
        
        PopulateResults(result);
        WireButtons();
        SaveResultToFirebase(result);
        
        CheckNextScenarioAvailability();
        
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    void LoadAvailableScenarios()
    {
        _availableScenarios.Clear();
        _availableMediumPairs.Clear();
        
        if (_currentDifficulty == DifficultyLevel.Easy)
        {
            // Load Easy scenarios from Firebase
            if (ScenarioSelectController.AllEasyScenarios != null && ScenarioSelectController.AllEasyScenarios.Count > 0)
            {
                _availableScenarios.AddRange(ScenarioSelectController.AllEasyScenarios);
                Debug.Log($"Loaded {_availableScenarios.Count} easy scenarios from Firebase");
            }
            
            // Fallback to current scenario if no Firebase scenarios
            if (_availableScenarios.Count == 0 && ScoringSystem.LastScenario != null)
            {
                _availableScenarios.Add(ScoringSystem.LastScenario);
                Debug.Log("Using current easy scenario as fallback");
            }
        }
        else if (_currentDifficulty == DifficultyLevel.Medium)
        {
            // Load Medium scenario pairs from Firebase
            // First, check if we have medium scenarios stored in ScenarioSelectController
            if (ScenarioSelectController.AllMediumScenarios != null && ScenarioSelectController.AllMediumScenarios.Count >= 2)
            {
                // Group medium scenarios into pairs (Patient A and Patient B)
                for (int i = 0; i < ScenarioSelectController.AllMediumScenarios.Count - 1; i += 2)
                {
                    ScenarioData[] pair = new ScenarioData[2];
                    pair[0] = ScenarioSelectController.AllMediumScenarios[i];
                    pair[1] = ScenarioSelectController.AllMediumScenarios[i + 1];
                    _availableMediumPairs.Add(pair);
                    _availableScenarios.Add(pair[0]); // For tracking index
                    Debug.Log($"Loaded medium pair: {pair[0].scenarioTitle} & {pair[1].scenarioTitle}");
                }
            }
            
            // Fallback: try to use MediumPatients from ScenarioSelector
            if (_availableMediumPairs.Count == 0 && ScenarioSelector.MediumPatients != null && ScenarioSelector.MediumPatients.Length >= 2)
            {
                ScenarioData[] pair = new ScenarioData[2];
                pair[0] = ScenarioSelector.MediumPatients[0];
                pair[1] = ScenarioSelector.MediumPatients[1];
                _availableMediumPairs.Add(pair);
                _availableScenarios.Add(pair[0]);
                Debug.Log($"Using fallback medium pair: {pair[0].scenarioTitle} & {pair[1].scenarioTitle}");
            }
            
            // Last resort: use current scenario as single (not ideal but works)
            if (_availableMediumPairs.Count == 0 && ScoringSystem.LastScenario != null)
            {
                ScenarioData[] pair = new ScenarioData[2];
                pair[0] = ScoringSystem.LastScenario;
                pair[1] = ScoringSystem.LastScenario;
                _availableMediumPairs.Add(pair);
                _availableScenarios.Add(pair[0]);
                Debug.LogWarning("Using same scenario for both patients as fallback");
            }
        }
        else if (_currentDifficulty == DifficultyLevel.Hard)
        {
            // Load Hard scenario triples from Firebase
            // Similar to medium but with 3 patients per scenario
            if (ScenarioSelectController.AllHardScenarios != null && ScenarioSelectController.AllHardScenarios.Count >= 3)
            {
                for (int i = 0; i < ScenarioSelectController.AllHardScenarios.Count - 2; i += 3)
                {
                    ScenarioData[] triple = new ScenarioData[3];
                    triple[0] = ScenarioSelectController.AllHardScenarios[i];
                    triple[1] = ScenarioSelectController.AllHardScenarios[i + 1];
                    triple[2] = ScenarioSelectController.AllHardScenarios[i + 2];
                    // For hard, we would need a different storage mechanism
                    _availableScenarios.Add(triple[0]);
                    Debug.Log($"Loaded hard triple starting with: {triple[0].scenarioTitle}");
                }
            }
            
            if (_availableScenarios.Count == 0 && ScoringSystem.LastScenario != null)
            {
                _availableScenarios.Add(ScoringSystem.LastScenario);
                Debug.Log("Using current hard scenario as fallback");
            }
        }
        
        // Find current index
        if (ScoringSystem.LastScenario != null)
        {
            for (int i = 0; i < _availableScenarios.Count; i++)
            {
                if (_availableScenarios[i].scenarioTitle == ScoringSystem.LastScenario.scenarioTitle)
                {
                    _currentScenarioIndex = i;
                    break;
                }
            }
        }
        
        Debug.Log($"Current scenario index: {_currentScenarioIndex}, Total available: {_availableScenarios.Count}");
        if (_currentDifficulty == DifficultyLevel.Medium)
        {
            Debug.Log($"Available medium pairs: {_availableMediumPairs.Count}");
        }
    }

    void CheckNextScenarioAvailability()
    {
        if (nextScenarioButton == null) return;
        
        int nextIndex = _currentScenarioIndex + 1;
        
        if (_currentDifficulty == DifficultyLevel.Medium)
        {
            // For Medium, check if we have another pair available
            if (nextIndex < _availableMediumPairs.Count)
            {
                nextScenarioButton.interactable = true;
                Debug.Log($"Next medium scenario pair available at index: {nextIndex}");
            }
            else
            {
                nextScenarioButton.interactable = false;
                Debug.Log("No more medium scenario pairs available - Next button disabled");
            }
        }
        else
        {
            // For Easy and Hard
            if (nextIndex < _availableScenarios.Count)
            {
                nextScenarioButton.interactable = true;
                Debug.Log($"Next scenario available: {_availableScenarios[nextIndex].scenarioTitle}");
            }
            else
            {
                nextScenarioButton.interactable = false;
                Debug.Log("No more scenarios available - Next button disabled");
            }
        }
    }

    void LoadPatientAssessments()
    {
        _patientAssessments = PatientAssessmentTracker.GetAssessments();
        
        Debug.Log($"Loaded {_patientAssessments.Count} patient assessments for feedback");
        
        for (int i = 0; i < _patientAssessments.Count; i++)
        {
            var assessment = _patientAssessments[i];
            Debug.Log($"  Assessment {i + 1}: {assessment.patientName} - Selected: {assessment.selectedCategory}, Correct: {assessment.correctCategory}, IsCorrect: {assessment.isCorrect}");
        }
        
        if (_patientAssessments.Count == 0 && ScoringSystem.LastResult != null)
        {
            _patientAssessments.Add(new PatientAssessmentData
            {
                patientName = "Patient",
                selectedCategory = ScoringSystem.LastResult.selectedCategory,
                correctCategory = ScoringSystem.LastResult.correctCategory,
                isCorrect = ScoringSystem.LastResult.isCorrect
            });
            Debug.Log("Added fallback single patient assessment");
        }
=======
        PopulateResults(result);
        WireButtons();
        SaveResultToFirebase(result);
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    async void SaveResultToFirebase(ScoringResult result)
    {
        if (FirebaseManager.Instance == null) return;

        ScenarioData scenario = ScoringSystem.LastScenario;
        if (scenario == null) return;

        await FirebaseManager.Instance.SaveSessionAsync(
            result,
            scenario.scenarioID,
            scenario.scenarioTitle,
            scenario.difficulty.ToString());
    }

<<<<<<< HEAD
    public void AddPatientAssessment(string patientName, TriageCategory selected, TriageCategory correct, bool isCorrect)
    {
        _patientAssessments.Add(new PatientAssessmentData
        {
            patientName = patientName,
            selectedCategory = selected,
            correctCategory = correct,
            isCorrect = isCorrect
        });
    }

    void PopulateResults(ScoringResult result)
    {
        bool triageCorrect = (result.selectedCategory == result.correctCategory);
=======
    void PopulateResults(ScoringResult result)
    {
        // Calculate triage correctness directly from selected vs correct
        bool triageCorrect = (result.selectedCategory == result.correctCategory);

>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        string title = GetPerformanceTitle(result.finalScore);
        string subtitle = GetPerformanceSubtitle(result.finalScore, triageCorrect);

        ApplyScoreCircleColors(result.finalScore);

<<<<<<< HEAD
        if (scoreText != null) scoreText.text = $"{result.finalScore}%";
        if (performanceTitle != null) performanceTitle.text = title;
        if (performanceSubtitle != null) performanceSubtitle.text = subtitle;

=======
        if (scoreText != null)
            scoreText.text = $"{result.finalScore}%";

        if (performanceTitle != null)
            performanceTitle.text = title;

        if (performanceSubtitle != null)
            performanceSubtitle.text = subtitle;

        // ── Accuracy Card ─────────────────────────────
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        if (accuracyValue != null)
        {
            accuracyValue.text = $"{result.accuracyScore}%";
            accuracyValue.color = GetScoreColor(result.accuracyScore);
        }

        if (accuracyLabel != null)
        {
            accuracyLabel.text = triageCorrect ? "Correct triage ✓" : "Incorrect triage ✗";
            accuracyLabel.color = triageCorrect ? new Color(0.086f, 0.635f, 0.290f) : new Color(0.863f, 0.149f, 0.149f);
        }

        if (accuracyIconBg != null)
<<<<<<< HEAD
            accuracyIconBg.color = triageCorrect ? new Color(0.086f, 0.635f, 0.290f, 0.12f) : new Color(0.863f, 0.149f, 0.149f, 0.12f);

=======
            accuracyIconBg.color = triageCorrect
                ? new Color(0.086f, 0.635f, 0.290f, 0.12f)
                : new Color(0.863f, 0.149f, 0.149f, 0.12f);

        // ── Speed Card ────────────────────────────────
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        if (speedValue != null)
        {
            speedValue.text = $"{result.speedScore}%";
            speedValue.color = GetScoreColor(result.speedScore);
        }

        bool isTimerExpired = result.timeTaken >= 180f || result.speedScore <= 0;

        if (speedLabel != null)
        {
            if (isTimerExpired)
            {
                speedLabel.text = "Timer expired — Delayed Care penalty";
                speedLabel.color = new Color(0.863f, 0.149f, 0.149f);
            }
            else
            {
                int mins = Mathf.FloorToInt(result.timeTaken / 60f);
                int secs = Mathf.FloorToInt(result.timeTaken % 60f);
                speedLabel.text = mins > 0 ? $"{mins}m {secs}s" : $"{secs} seconds";
                speedLabel.color = new Color(0.443f, 0.451f, 0.529f);
            }
        }

<<<<<<< HEAD
        if (speedIconBg != null) speedIconBg.color = new Color(0.145f, 0.337f, 0.922f, 0.12f);

=======
        if (speedIconBg != null)
            speedIconBg.color = new Color(0.145f, 0.337f, 0.922f, 0.12f);

        // ── Confidence Card ────────────────────────────────
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        if (confidenceValue != null)
        {
            confidenceValue.text = $"{result.confidenceScore}%";
            confidenceValue.color = GetScoreColor(result.confidenceScore);
        }

        if (confidenceLabel != null)
        {
            confidenceLabel.text = "Self-reported confidence";
            confidenceLabel.color = new Color(0.443f, 0.451f, 0.529f);
        }

<<<<<<< HEAD
        if (confidenceIconBg != null) confidenceIconBg.color = new Color(0.086f, 0.635f, 0.290f, 0.12f);

        PopulateTriageAssessments();
        
=======
        if (confidenceIconBg != null)
            confidenceIconBg.color = new Color(0.086f, 0.635f, 0.290f, 0.12f);

        // ── Triage Comparison Panel ────────────────────
        if (isTimerExpired && triageAssessmentContainer != null)
        {
            triageAssessmentContainer.SetActive(false);
        }
        else
        {
            if (triageAssessmentContainer != null) triageAssessmentContainer.SetActive(true);

            if (yourSelectionPillText != null)
                yourSelectionPillText.text = GetTriageLabel(result.selectedCategory);

            if (yourSelectionPill != null)
            {
                yourSelectionPill.color = GetTriagePillColor(result.selectedCategory);

                if (!triageCorrect)
                {
                    Outline ol = yourSelectionPill.GetComponent<Outline>() ?? yourSelectionPill.gameObject.AddComponent<Outline>();
                    ol.effectColor = new Color(0.863f, 0.149f, 0.149f, 0.9f);
                    ol.effectDistance = new Vector2(3, 3);
                }
            }

            if (yourSelectionPillText != null)
                yourSelectionPillText.color = GetTriagePillTextColor(result.selectedCategory);

            if (correctTriagePillText != null)
                correctTriagePillText.text = GetTriageLabel(result.correctCategory);

            if (correctTriagePill != null)
                correctTriagePill.color = GetTriagePillColor(result.correctCategory);

            if (correctTriagePillText != null)
                correctTriagePillText.color = GetTriagePillTextColor(result.correctCategory);
        }

        // ── Explanation ─────────
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        if (explanationText != null)
        {
            string explanation = ScoringSystem.LastScenario?.clinicalExplanation;

            if (string.IsNullOrEmpty(explanation))
                explanation = triageCorrect
                    ? "Well done! Your triage decision aligned with the START protocol criteria."
                    : "Review the START triage algorithm. Complete the RPM sequence in order (Respiration → Perfusion → Mental Status) before assigning a triage tag.";

            if (isTimerExpired)
            {
                explanation = "Critical Care Delay: The allocation timeline window reached its limit before a definitive prioritization tag was assigned. Under mass-casualty parameters, speed is vital to save lives.";
            }
            else if (!triageCorrect)
            {
                bool underTriage = result.selectedCategory > result.correctCategory;
                string prefix = underTriage ? "⚠ Under-triage detected. " : "⚠ Over-triage detected. ";
                explanation = prefix + explanation;
            }

            explanationText.text = explanation;
        }
<<<<<<< HEAD
        
        StartCoroutine(RebuildLayoutNextFrame());
    }

    System.Collections.IEnumerator RebuildLayoutNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        if (triageAssessmentParent != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(triageAssessmentParent as RectTransform);
        }
        if (explanationText != null && explanationText.transform.parent != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(explanationText.transform.parent as RectTransform);
        }
    }

    void PopulateTriageAssessments()
    {
        if (triageAssessmentContainer == null || triageAssessmentParent == null)
        {
            Debug.LogWarning("Triage assessment container or parent not assigned!");
            return;
        }

        foreach (Transform child in triageAssessmentParent)
        {
            if (child.gameObject != triageAssessmentContainer)
                Destroy(child.gameObject);
        }

        Debug.Log($"Populating {_patientAssessments.Count} triage assessment cards");

        if (_patientAssessments.Count == 0)
        {
            triageAssessmentContainer.SetActive(false);
            return;
        }

        triageAssessmentContainer.SetActive(true);

        foreach (var assessment in _patientAssessments)
        {
            CreateAssessmentCard(assessment);
        }
    }

    void CreateAssessmentCard(PatientAssessmentData data)
    {
        GameObject card;
        
        if (triageAssessmentCardPrefab != null)
        {
            card = Instantiate(triageAssessmentCardPrefab, triageAssessmentParent);
        }
        else
        {
            card = CreateDynamicAssessmentCard();
        }
        
        card.name = $"AssessmentCard_{data.patientName}";
        
        TextMeshProUGUI patientNameText = FindTextInChildren(card, "PatientName");
        TextMeshProUGUI yourSelectionText = FindTextInChildren(card, "YourSelection");
        TextMeshProUGUI correctAnswerText = FindTextInChildren(card, "CorrectAnswer");
        Image statusIcon = FindImageInChildren(card, "StatusIcon");
        
        if (patientNameText == null)
        {
            patientNameText = card.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log($"Using fallback for {data.patientName} - found {patientNameText?.name}");
        }
        
        if (patientNameText != null) patientNameText.text = data.patientName;
        if (yourSelectionText != null) yourSelectionText.text = GetTriageLabel(data.selectedCategory);
        if (correctAnswerText != null) correctAnswerText.text = GetTriageLabel(data.correctCategory);
        
        Color resultColor = data.isCorrect ? new Color(0.086f, 0.635f, 0.290f) : new Color(0.863f, 0.149f, 0.149f);
        
        if (yourSelectionText != null)
        {
            yourSelectionText.color = resultColor;
            var bg = yourSelectionText.GetComponentInParent<Image>();
            if (bg != null) bg.color = new Color(resultColor.r, resultColor.g, resultColor.b, 0.1f);
        }
        
        if (statusIcon != null)
        {
            statusIcon.color = resultColor;
        }
        
        Debug.Log($"Created assessment card for {data.patientName} - Correct: {data.isCorrect}");
    }

    TextMeshProUGUI FindTextInChildren(GameObject parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name)
            {
                return child.GetComponent<TextMeshProUGUI>();
            }
        }
        return null;
    }

    Image FindImageInChildren(GameObject parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name)
            {
                return child.GetComponent<Image>();
            }
        }
        return null;
    }

    GameObject CreateDynamicAssessmentCard()
    {
        GameObject card = new GameObject("AssessmentCard", typeof(RectTransform));
        card.transform.SetParent(triageAssessmentParent, false);
        
        RectTransform rect = card.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 80);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        
        LayoutElement layout = card.AddComponent<LayoutElement>();
        layout.minHeight = 80;
        layout.flexibleWidth = 1;
        
        Image bg = card.AddComponent<Image>();
        bg.color = new Color(0.95f, 0.97f, 0.98f);
        
        GameObject nameGo = new GameObject("PatientName", typeof(TextMeshProUGUI));
        nameGo.transform.SetParent(card.transform, false);
        TextMeshProUGUI nameText = nameGo.GetComponent<TextMeshProUGUI>();
        nameText.fontSize = 16;
        nameText.fontStyle = FontStyles.Bold;
        RectTransform nameRect = nameGo.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.5f);
        nameRect.anchorMax = new Vector2(0.3f, 0.5f);
        nameRect.offsetMin = new Vector2(12, -15);
        nameRect.offsetMax = new Vector2(0, 15);
        
        GameObject yourGo = new GameObject("YourSelection", typeof(TextMeshProUGUI));
        yourGo.transform.SetParent(card.transform, false);
        TextMeshProUGUI yourText = yourGo.GetComponent<TextMeshProUGUI>();
        yourText.fontSize = 14;
        RectTransform yourRect = yourGo.GetComponent<RectTransform>();
        yourRect.anchorMin = new Vector2(0.35f, 0.5f);
        yourRect.anchorMax = new Vector2(0.65f, 0.5f);
        yourRect.offsetMin = new Vector2(0, -15);
        yourRect.offsetMax = new Vector2(0, 15);
        
        GameObject correctGo = new GameObject("CorrectAnswer", typeof(TextMeshProUGUI));
        correctGo.transform.SetParent(card.transform, false);
        TextMeshProUGUI correctText = correctGo.GetComponent<TextMeshProUGUI>();
        correctText.fontSize = 14;
        RectTransform correctRect = correctGo.GetComponent<RectTransform>();
        correctRect.anchorMin = new Vector2(0.7f, 0.5f);
        correctRect.anchorMax = new Vector2(1f, 0.5f);
        correctRect.offsetMin = new Vector2(0, -15);
        correctRect.offsetMax = new Vector2(-12, 15);
        
        return card;
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    void ApplyScoreCircleColors(int score)
    {
        Color textAndBorderColor;
        Color solidPastelFillColor;

        if (score >= 90)
        {
            textAndBorderColor = new Color(0.086f, 0.635f, 0.290f);
            solidPastelFillColor = new Color(0.85f, 0.96f, 0.89f);
        }
        else if (score >= 80)
        {
            textAndBorderColor = new Color(0.145f, 0.337f, 0.922f);
            solidPastelFillColor = new Color(0.88f, 0.91f, 0.99f);
        }
        else if (score >= 70)
        {
            textAndBorderColor = new Color(0.75f, 0.40f, 0.01f);
            solidPastelFillColor = new Color(0.99f, 0.97f, 0.82f);
        }
        else
        {
            textAndBorderColor = new Color(0.863f, 0.149f, 0.149f);
            solidPastelFillColor = new Color(0.99f, 0.88f, 0.88f);
        }

        if (scoreText != null) scoreText.color = textAndBorderColor;
        if (performanceTitle != null) performanceTitle.color = textAndBorderColor;
        if (scoreCircleBorder != null) scoreCircleBorder.color = textAndBorderColor;
        if (scoreCircleFill != null) scoreCircleFill.color = solidPastelFillColor;
    }

<<<<<<< HEAD
    Color GetScoreColor(int score)
    {
        if (score >= 85) return new Color(0.086f, 0.635f, 0.290f);
        if (score >= 70) return new Color(0.851f, 0.467f, 0.024f);
        return new Color(0.863f, 0.149f, 0.149f);
=======
    // Color coding for scores (0-100)
    Color GetScoreColor(int score)
    {
        if (score >= 85) return new Color(0.086f, 0.635f, 0.290f);     // Green
        if (score >= 70) return new Color(0.851f, 0.467f, 0.024f);     // Yellow/Orange
        return new Color(0.863f, 0.149f, 0.149f);                       // Red
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    void WireButtons()
    {
        if (nextScenarioButton != null)
<<<<<<< HEAD
            nextScenarioButton.onClick.AddListener(OnNextScenarioClicked);
=======
            nextScenarioButton.onClick.AddListener(() =>
            {
                ScenarioSelector.Reset();
                ScoringSystem.LastResult = null;
                ScoringSystem.LastScenario = null;
                SceneManager.LoadScene("ScenarioSelectScene");
            });
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

        if (dashboardButton != null)
            dashboardButton.onClick.AddListener(() =>
            {
                ScenarioSelector.Reset();
                ScoringSystem.LastResult = null;
                ScoringSystem.LastScenario = null;
                SceneManager.LoadScene("DashboardScene");
            });
    }

<<<<<<< HEAD
    void OnNextScenarioClicked()
    {
        int nextIndex = _currentScenarioIndex + 1;
        
        if (_currentDifficulty == DifficultyLevel.Medium)
        {
            if (nextIndex < _availableMediumPairs.Count)
            {
                ScenarioData[] nextPair = _availableMediumPairs[nextIndex];
                Debug.Log($"Loading next medium scenario pair: {nextPair[0].scenarioTitle} & {nextPair[1].scenarioTitle}");
                ScenarioSelector.SelectMedium(nextPair[0], nextPair[1]);
                SceneManager.LoadScene("MainAR");
            }
            else
            {
                Debug.Log("No more medium scenarios available");
                GoToScenarioSelect();
            }
        }
        else if (_currentDifficulty == DifficultyLevel.Hard)
        {
            if (nextIndex < _availableScenarios.Count)
            {
                ScenarioData nextScenario = _availableScenarios[nextIndex];
                Debug.Log($"Loading next hard scenario: {nextScenario.scenarioTitle}");
                // For Hard, you would need a triple selection
                // This is a placeholder - implement based on your Hard scenario structure
                ScenarioSelector.SelectEasy(nextScenario); // Placeholder
                SceneManager.LoadScene("MainAR");
            }
            else
            {
                Debug.Log("No more hard scenarios available");
                GoToScenarioSelect();
            }
        }
        else // Easy
        {
            if (nextIndex < _availableScenarios.Count)
            {
                ScenarioData nextScenario = _availableScenarios[nextIndex];
                Debug.Log($"Loading next easy scenario: {nextScenario.scenarioTitle}");
                ScenarioSelector.SelectEasy(nextScenario);
                SceneManager.LoadScene("MainAR");
            }
            else
            {
                Debug.Log("No more easy scenarios available");
                GoToScenarioSelect();
            }
        }
    }

    void GoToScenarioSelect()
    {
        SceneManager.LoadScene("ScenarioSelectScene");
    }

=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    void ShowPlaceholder()
    {
        Color placeholderGray = new Color(0.443f, 0.451f, 0.529f);
        Color lightGrayBackground = new Color(0.94f, 0.94f, 0.95f);

<<<<<<< HEAD
        if (scoreText != null) scoreText.text = "--";
        if (scoreCircleBorder != null) scoreCircleBorder.color = placeholderGray;
        if (scoreCircleFill != null) scoreCircleFill.color = lightGrayBackground;

        if (performanceTitle != null) performanceTitle.text = "No Results Yet";
        if (performanceSubtitle != null) performanceSubtitle.text = "Complete a simulation to see your results";
=======
        if (scoreText != null)
        {
            scoreText.text = "--";
            scoreText.color = placeholderGray;
        }

        if (scoreCircleBorder != null) scoreCircleBorder.color = placeholderGray;
        if (scoreCircleFill != null) scoreCircleFill.color = lightGrayBackground;

        if (performanceTitle != null)
        {
            performanceTitle.text = "No Results Yet";
            performanceTitle.color = placeholderGray;
        }

        if (performanceSubtitle != null)
        {
            performanceSubtitle.text = "Complete a simulation to see your results";
            performanceSubtitle.color = placeholderGray;
        }
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

        if (accuracyValue != null) { accuracyValue.text = "--%"; accuracyValue.color = placeholderGray; }
        if (accuracyLabel != null) { accuracyLabel.text = "No Score..."; accuracyLabel.color = placeholderGray; }
        if (accuracyIconBg != null) accuracyIconBg.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);

        if (speedValue != null) { speedValue.text = "--%"; speedValue.color = placeholderGray; }
        if (speedLabel != null) { speedLabel.text = "No Score..."; speedLabel.color = placeholderGray; }
        if (speedIconBg != null) speedIconBg.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);

        if (confidenceValue != null) { confidenceValue.text = "--%"; confidenceValue.color = placeholderGray; }
        if (confidenceLabel != null) { confidenceLabel.text = "No Score..."; confidenceLabel.color = placeholderGray; }
        if (confidenceIconBg != null) confidenceIconBg.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);

<<<<<<< HEAD
        if (triageAssessmentContainer != null) triageAssessmentContainer.SetActive(false);
=======
        if (triageAssessmentContainer != null)
            triageAssessmentContainer.SetActive(false);
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

        if (explanationText != null)
            explanationText.text = "No evaluation feedback has been computed yet. Head over to the selection panel and complete a scenario to populate this layout box.";

<<<<<<< HEAD
        if (nextScenarioButton != null) nextScenarioButton.interactable = false;
=======
        if (nextScenarioButton != null)
            nextScenarioButton.interactable = false;
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    string GetPerformanceTitle(int score)
    {
        if (score >= 90) return "Excellent Clinical Readiness!";
        if (score >= 80) return "Very Good Performance!";
        if (score >= 70) return "Satisfactory";
        return "Needs Improvement";
    }

    string GetPerformanceSubtitle(int score, bool triageCorrect)
    {
        if (score >= 90) return "Outstanding triage performance. You're ready for clinical duty.";
        if (score >= 80) return "Strong triage skills. Keep practicing to reach excellence.";
        if (score >= 70)
            return triageCorrect
                ? "You made the correct decision but could improve your response speed."
                : "Review the START protocol to strengthen your triage accuracy.";
        return "Review the START triage algorithm and retry the scenario.";
    }

    string GetTriageLabel(TriageCategory cat) => cat switch
    {
        TriageCategory.Immediate => "Red — Immediate",
        TriageCategory.Delayed => "Yellow — Delayed",
        TriageCategory.Minor => "Green — Minor",
        TriageCategory.Expectant => "Black — Deceased",
        _ => "Unknown"
    };
<<<<<<< HEAD
=======

    Color GetTriagePillColor(TriageCategory cat) => cat switch
    {
        TriageCategory.Immediate => new Color(0.863f, 0.149f, 0.149f),
        TriageCategory.Delayed => new Color(0.996f, 0.878f, 0.282f),
        TriageCategory.Minor => new Color(0.086f, 0.635f, 0.290f),
        TriageCategory.Expectant => new Color(0.118f, 0.161f, 0.231f),
        _ => Color.gray
    };

    Color GetTriagePillTextColor(TriageCategory cat) => cat == TriageCategory.Delayed ? new Color(0.522f, 0.302f, 0.008f) : Color.white;
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
}