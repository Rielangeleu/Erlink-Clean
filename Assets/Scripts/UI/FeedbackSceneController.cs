using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

/// <summary>
/// Feedback Scene — ALL values driven by actual performance.
/// Score color, title, subtitle all change based on result.
/// </summary>
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

    [Header("Triage Comparison")]
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

        if (result == null)
        {
            ShowPlaceholder();
            WireButtons();
            return;
        }

        PopulateResults(result);
        AnimateEntrance();
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
        // Get tier based on actual score
        Color mainColor = GetScoreColor(result.finalScore);
        string title = GetPerformanceTitle(result.finalScore);
        string subtitle = GetPerformanceSubtitle(
            result.finalScore, result.isCorrect);

        // ── Score Circle ─────────────────────────────
        if (scoreText != null)
        {
            scoreText.text = "0%"; // starts at 0, animates up
            scoreText.color = mainColor;
        }

        if (scoreCircleFill != null)
            scoreCircleFill.color = new Color(
                mainColor.r, mainColor.g, mainColor.b, 0.15f);

        if (scoreCircleBorder != null)
            scoreCircleBorder.color = new Color(
                mainColor.r, mainColor.g, mainColor.b, 0.5f);

        // ── Performance Text ──────────────────────────
        if (performanceTitle != null)
        {
            performanceTitle.text = title;
            performanceTitle.color = mainColor;
        }

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
            bool correct = result.isCorrect;
            accuracyLabel.text = correct
                ? "Correct triage decision ✓"
                : "Incorrect triage decision ✗";
            accuracyLabel.color = correct
                ? new Color(0.086f, 0.635f, 0.290f)
                : new Color(0.863f, 0.149f, 0.149f);
        }

        if (accuracyIconBg != null)
            accuracyIconBg.color = result.isCorrect
                ? new Color(0.086f, 0.635f, 0.290f, 0.15f)
                : new Color(0.863f, 0.149f, 0.149f, 0.15f);

        // ── Speed Card ────────────────────────────────
        if (speedValue != null)
        {
            speedValue.text = $"{result.speedScore}%";
            speedValue.color = GetScoreColor(result.speedScore);
        }

        if (speedLabel != null)
        {
            if (result.timeTaken >= float.MaxValue)
            {
                speedLabel.text = "Timer expired — Delayed Care penalty";
                speedLabel.color = new Color(0.863f, 0.149f, 0.149f);
            }
            else
            {
                int mins = Mathf.FloorToInt(result.timeTaken / 60f);
                int secs = Mathf.FloorToInt(result.timeTaken % 60f);
                speedLabel.text = mins > 0
                    ? $"Response time: {mins}m {secs}s"
                    : $"Response time: {secs} seconds";
                speedLabel.color = new Color(0.443f, 0.451f, 0.529f);
            }
        }

        if (speedIconBg != null)
            speedIconBg.color = new Color(
                0.145f, 0.337f, 0.922f, 0.15f);

        // ── Triage Comparison ─────────────────────────
        if (yourSelectionPillText != null)
            yourSelectionPillText.text =
                GetTriageLabel(result.selectedCategory);

        if (yourSelectionPill != null)
        {
            yourSelectionPill.color =
                GetTriagePillColor(result.selectedCategory);

            // Red outline if wrong
            if (!result.isCorrect)
            {
                Outline ol = yourSelectionPill
                    .GetComponent<Outline>()
                    ?? yourSelectionPill
                        .gameObject.AddComponent<Outline>();
                ol.effectColor =
                    new Color(0.863f, 0.149f, 0.149f, 0.9f);
                ol.effectDistance = new Vector2(3, 3);
            }
        }

        if (yourSelectionPillText != null)
            yourSelectionPillText.color =
                GetTriagePillTextColor(result.selectedCategory);

        if (correctTriagePillText != null)
            correctTriagePillText.text =
                GetTriageLabel(result.correctCategory);

        if (correctTriagePill != null)
            correctTriagePill.color =
                GetTriagePillColor(result.correctCategory);

        if (correctTriagePillText != null)
            correctTriagePillText.color =
                GetTriagePillTextColor(result.correctCategory);

        // ── Explanation ───────────────────────────────
        if (explanationText != null)
        {
            string explanation =
                ScoringSystem.LastScenario?.clinicalExplanation;

            if (string.IsNullOrEmpty(explanation))
                explanation = result.isCorrect
                    ? "Well done! Your triage decision aligned " +
                      "with the START protocol criteria."
                    : "Review the START triage algorithm. " +
                      "Complete the RPM sequence in order " +
                      "(Respiration → Perfusion → Mental Status) " +
                      "before assigning a triage tag.";

            if (!result.isCorrect)
            {
                bool underTriage = result.selectedCategory >
                    result.correctCategory;
                string prefix = underTriage
                    ? "⚠ Under-triage detected. "
                    : "⚠ Over-triage detected. ";
                explanation = prefix + explanation;
            }

            explanationText.text = explanation;
        }
    }

    void AnimateEntrance()
    {
        ScoringResult result = ScoringSystem.LastResult;
        if (result == null) return;

        // Animate score counting up from 0
        if (scoreText != null)
        {
            int target = result.finalScore;
            DOTween.To(
                () => 0,
                x => scoreText.text = $"{x}%",
                target, 1.2f)
                .SetEase(Ease.OutCubic)
                .SetDelay(0.3f);
        }

        // Scale circle from 0
        if (scoreCircleFill != null)
        {
            Transform parent =
                scoreCircleFill.transform.parent;
            if (parent != null)
            {
                parent.localScale = Vector3.zero;
                parent.DOScale(1f, 0.5f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(0.1f);
            }
        }

        // Performance title fades + slides up
        if (performanceTitle != null)
        {
            CanvasGroup cg =
                performanceTitle.GetComponent<CanvasGroup>()
                ?? performanceTitle.gameObject
                    .AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.DOFade(1f, 0.4f).SetDelay(0.6f);

            RectTransform rt =
                performanceTitle.GetComponent<RectTransform>();
            rt.anchoredPosition += new Vector2(0, -15);
            rt.DOAnchorPosY(
                rt.anchoredPosition.y + 15, 0.4f)
                .SetDelay(0.6f)
                .SetEase(Ease.OutCubic);
        }
    }

    void WireButtons()
    {
        if (nextScenarioButton != null)
            nextScenarioButton.onClick.AddListener(() =>
            {
                ScenarioSelector.Reset();
                ScoringSystem.LastResult = null;
                ScoringSystem.LastScenario = null;
                SceneManager.LoadScene("MainAR");
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
        if (scoreText != null)
        {
            scoreText.text = "--";
            scoreText.color = new Color(0.443f, 0.451f, 0.529f);
        }
        if (performanceTitle != null)
            performanceTitle.text = "No Results Yet";
        if (performanceSubtitle != null)
            performanceSubtitle.text =
                "Complete a simulation to see your results";
        if (nextScenarioButton != null)
            nextScenarioButton.interactable = false;
    }

    // ── Score → Color ─────────────────────────────────
    Color GetScoreColor(int score)
    {
        if (score >= 90)
            return new Color(0.086f, 0.635f, 0.290f); // green
        if (score >= 80)
            return new Color(0.145f, 0.337f, 0.922f); // blue
        if (score >= 70)
            return new Color(0.851f, 0.467f, 0.024f); // yellow
        return new Color(0.863f, 0.149f, 0.149f);     // red
    }

    // ── Score → Title ─────────────────────────────────
    string GetPerformanceTitle(int score)
    {
        if (score >= 90) return "Excellent Clinical Readiness!";
        if (score >= 80) return "Very Good Performance!";
        if (score >= 70) return "Satisfactory";
        return "Needs Improvement";
    }

    // ── Score → Subtitle ──────────────────────────────
    string GetPerformanceSubtitle(int score, bool isCorrect)
    {
        if (score >= 90)
            return "Outstanding triage performance. " +
                   "You're ready for clinical duty.";
        if (score >= 80)
            return "Strong triage skills. " +
                   "Keep practicing to reach excellence.";
        if (score >= 70)
            return isCorrect
                ? "You made the correct decision but " +
                  "could improve your response speed."
                : "Review the START protocol to strengthen " +
                  "your triage accuracy.";
        return "Review the START triage algorithm " +
               "and retry the scenario.";
    }

    // ── Triage helpers ────────────────────────────────
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
        TriageCategory.Immediate =>
            new Color(0.863f, 0.149f, 0.149f),
        TriageCategory.Delayed =>
            new Color(0.996f, 0.878f, 0.282f),
        TriageCategory.Minor =>
            new Color(0.086f, 0.635f, 0.290f),
        TriageCategory.Expectant =>
            new Color(0.118f, 0.161f, 0.231f),
        _ => Color.gray
    };

    Color GetTriagePillTextColor(TriageCategory cat) =>
        cat == TriageCategory.Delayed
            ? new Color(0.522f, 0.302f, 0.008f)
            : Color.white;
}