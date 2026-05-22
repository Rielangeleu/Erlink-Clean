using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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

    [Header("Triage Comparison Panel")]
    public GameObject triageAssessmentContainer;
    public Image yourSelectionPill;
    public TextMeshProUGUI yourSelectionPillText;
    public Image correctTriagePill;
    public TextMeshProUGUI correctTriagePillText;

    [Header("Explanation")]
    public TextMeshProUGUI explanationText;

    [Header("Buttons")]
    public Button nextScenarioButton;
    public Button dashboardButton;

    void Start()
    {
        ScoringResult result = ScoringSystem.LastResult;

        if (result == null || result.timeTaken == 0)
        {
            ShowPlaceholder();
            WireButtons();
            return;
        }

        PopulateResults(result);
        WireButtons();
        SaveResultToFirebase(result);
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

    void PopulateResults(ScoringResult result)
    {
        // Calculate triage correctness directly from selected vs correct
        bool triageCorrect = (result.selectedCategory == result.correctCategory);

        string title = GetPerformanceTitle(result.finalScore);
        string subtitle = GetPerformanceSubtitle(result.finalScore, triageCorrect);

        ApplyScoreCircleColors(result.finalScore);

        if (scoreText != null)
            scoreText.text = $"{result.finalScore}%";

        if (performanceTitle != null)
            performanceTitle.text = title;

        if (performanceSubtitle != null)
            performanceSubtitle.text = subtitle;

        // ── Accuracy Card ─────────────────────────────
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
            accuracyIconBg.color = triageCorrect
                ? new Color(0.086f, 0.635f, 0.290f, 0.12f)
                : new Color(0.863f, 0.149f, 0.149f, 0.12f);

        // ── Speed Card ────────────────────────────────
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

        if (speedIconBg != null)
            speedIconBg.color = new Color(0.145f, 0.337f, 0.922f, 0.12f);

        // ── Confidence Card ────────────────────────────────
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

    // Color coding for scores (0-100)
    Color GetScoreColor(int score)
    {
        if (score >= 85) return new Color(0.086f, 0.635f, 0.290f);     // Green
        if (score >= 70) return new Color(0.851f, 0.467f, 0.024f);     // Yellow/Orange
        return new Color(0.863f, 0.149f, 0.149f);                       // Red
    }

    void WireButtons()
    {
        if (nextScenarioButton != null)
            nextScenarioButton.onClick.AddListener(() =>
            {
                ScenarioSelector.Reset();
                ScoringSystem.LastResult = null;
                ScoringSystem.LastScenario = null;
                SceneManager.LoadScene("ScenarioSelectScene");
            });

        if (dashboardButton != null)
            dashboardButton.onClick.AddListener(() =>
            {
                ScenarioSelector.Reset();
                ScoringSystem.LastResult = null;
                ScoringSystem.LastScenario = null;
                SceneManager.LoadScene("DashboardScene");
            });
    }

    void ShowPlaceholder()
    {
        Color placeholderGray = new Color(0.443f, 0.451f, 0.529f);
        Color lightGrayBackground = new Color(0.94f, 0.94f, 0.95f);

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

        if (accuracyValue != null) { accuracyValue.text = "--%"; accuracyValue.color = placeholderGray; }
        if (accuracyLabel != null) { accuracyLabel.text = "No Score..."; accuracyLabel.color = placeholderGray; }
        if (accuracyIconBg != null) accuracyIconBg.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);

        if (speedValue != null) { speedValue.text = "--%"; speedValue.color = placeholderGray; }
        if (speedLabel != null) { speedLabel.text = "No Score..."; speedLabel.color = placeholderGray; }
        if (speedIconBg != null) speedIconBg.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);

        if (confidenceValue != null) { confidenceValue.text = "--%"; confidenceValue.color = placeholderGray; }
        if (confidenceLabel != null) { confidenceLabel.text = "No Score..."; confidenceLabel.color = placeholderGray; }
        if (confidenceIconBg != null) confidenceIconBg.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);

        if (triageAssessmentContainer != null)
            triageAssessmentContainer.SetActive(false);

        if (explanationText != null)
            explanationText.text = "No evaluation feedback has been computed yet. Head over to the selection panel and complete a scenario to populate this layout box.";

        if (nextScenarioButton != null)
            nextScenarioButton.interactable = false;
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

    Color GetTriagePillColor(TriageCategory cat) => cat switch
    {
        TriageCategory.Immediate => new Color(0.863f, 0.149f, 0.149f),
        TriageCategory.Delayed => new Color(0.996f, 0.878f, 0.282f),
        TriageCategory.Minor => new Color(0.086f, 0.635f, 0.290f),
        TriageCategory.Expectant => new Color(0.118f, 0.161f, 0.231f),
        _ => Color.gray
    };

    Color GetTriagePillTextColor(TriageCategory cat) => cat == TriageCategory.Delayed ? new Color(0.522f, 0.302f, 0.008f) : Color.white;
}