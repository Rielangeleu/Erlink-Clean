using UnityEngine;

/// <summary>
/// Implements exact scoring formula from ERLink AR thesis.
/// Score = (Accuracy × 0.6) + (Speed × 0.3) + (Confidence × 0.1)
/// </summary>
public class ScoringSystem : MonoBehaviour
{
    public static ScoringResult LastResult;
    public static ScenarioData LastScenario;

    private int _accuracyPoints = 0;
    private float _timeTaken = 0f;
    private bool _ehrCorrect = false;
    private int _confidenceScore = 3;
    private TriageCategory _selected;
    private TriageCategory _correct;
    private ScenarioData _activeScenario;

    public void SetActiveScenario(ScenarioData scenario)
    {
        _activeScenario = scenario;
    }

    public void RecordTriageDecision(
        TriageCategory selected,
        TriageCategory correct,
        float timeTaken)
    {
        _selected = selected;
        _correct = correct;
        _timeTaken = timeTaken;
        _accuracyPoints = (selected == correct) ? 3 : 0;
    }

    public void RecordPriorityOrder(bool correctOrder)
    {
        if (correctOrder) _accuracyPoints += 1;
    }

    public void RecordEHRAction(bool isCorrect)
    {
        _ehrCorrect = isCorrect;
        if (isCorrect) _accuracyPoints += 1;
    }

    public void SetConfidenceScore(int score)
    {
        _confidenceScore = Mathf.Clamp(score, 1, 5);
    }

    public void ApplyTimerPenalty()
    {
        _timeTaken = float.MaxValue;
        Debug.Log("Delayed Care penalty applied.");
    }

    public void CalculateFinalScore()
    {
        // Accuracy (max 5 pts → normalize to 100)
        float accuracyNormalized = (_accuracyPoints / 5f) * 100f;

        // Speed
        float speedPoints;
        if (_timeTaken >= float.MaxValue) speedPoints = 0f;
        else if (_timeTaken <= 60f) speedPoints = 5f;
        else if (_timeTaken <= 120f) speedPoints = 3f;
        else speedPoints = 1f;
        float speedNormalized = (speedPoints / 5f) * 100f;

        // Confidence (1-5 scale)
        float confidenceNormalized = (_confidenceScore / 5f) * 100f;

        // Final formula
        float finalScore =
            (accuracyNormalized * 0.6f) +
            (speedNormalized * 0.3f) +
            (confidenceNormalized * 0.1f);

        // Interpretation
        string interpretation = finalScore switch
        {
            >= 90f => "Excellent Clinical Readiness",
            >= 80f => "Very Good",
            >= 70f => "Satisfactory",
            _ => "Needs Improvement"
        };

        // Save scenario reference
        LastScenario = _activeScenario;

        // Save result for FeedbackScene
        LastResult = new ScoringResult
        {
            finalScore = Mathf.RoundToInt(finalScore),
            accuracyScore = Mathf.RoundToInt(accuracyNormalized),
            speedScore = Mathf.RoundToInt(speedNormalized),
            confidenceScore = Mathf.RoundToInt(confidenceNormalized),
            timeTaken = _timeTaken,
            selectedCategory = _selected,
            correctCategory = _correct,
            ehrCorrect = _ehrCorrect,
            interpretation = interpretation,
            isCorrect = _selected == _correct
        };

        Debug.Log($"Final Score: {finalScore:F1}% — {interpretation}");
    }

    public void ResetScoring()
    {
        _accuracyPoints = 0;
        _timeTaken = 0f;
        _ehrCorrect = false;
        _confidenceScore = 3;
    }
}

[System.Serializable]
public class ScoringResult
{
    public int finalScore;
    public int accuracyScore;
    public int speedScore;
    public int confidenceScore;
    public float timeTaken;
    public TriageCategory selectedCategory;
    public TriageCategory correctCategory;
    public bool ehrCorrect;
    public string interpretation;
    public bool isCorrect;
}