using UnityEngine;

public class ScoringSystem : MonoBehaviour
{
    public static ScoringResult LastResult;
    public static ScenarioData LastScenario;

    // Scoring components
    private int _accuracyPoints = 0;
    private float _timeTaken = 0f;
    private bool _ehrCorrect = false;
    private int _confidenceScore = 5;  // Default to 5 (100%) instead of 3
    private TriageCategory _selected;
    private TriageCategory _correct;
    private ScenarioData _activeScenario;

    // RPM scoring
    private int _rpmCorrectAnswers = 0;
    private const int MAX_RPM_POINTS = 3;

    // Priority order
    private bool _priorityCorrect = false;

    // Track if triage was correct
    private bool _triageWasCorrect = false;

    // Track max accuracy points based on scenario type
    private int _maxAccuracyPoints = 8;  // Default: Triage(3) + RPM(3) + Priority(1) + EHR(1)

    public void SetActiveScenario(ScenarioData scenario)
    {
        _activeScenario = scenario;
        _accuracyPoints = 0;
        _rpmCorrectAnswers = 0;
        _priorityCorrect = false;
        _ehrCorrect = false;
        _triageWasCorrect = false;
        _confidenceScore = 5;  // Reset to 5 for new scenario
        _maxAccuracyPoints = 8;  // Default max

        // Adjust max points based on scenario difficulty
        if (scenario != null)
        {
            if (scenario.difficulty == DifficultyLevel.Easy)
                _maxAccuracyPoints = 8;  // Triage(3) + RPM(3) + EHR(1) + Priority(1)
            else if (scenario.difficulty == DifficultyLevel.Medium)
                _maxAccuracyPoints = 16; // 2 patients × 8 points each
            else if (scenario.difficulty == DifficultyLevel.Hard)
                _maxAccuracyPoints = 24; // 3 patients × 8 points each
        }

        Debug.Log($"ScoringSystem: Active scenario set to {scenario?.scenarioTitle}, Max Accuracy: {_maxAccuracyPoints}");
    }

    public void RecordTriageDecision(TriageCategory selected, TriageCategory correct, float timeTaken)
    {
        _selected = selected;
        _correct = correct;
        _timeTaken = timeTaken;
        _triageWasCorrect = (selected == correct);

        // Triage accuracy: 3 points for correct, 0 for incorrect
        if (_triageWasCorrect)
        {
            _accuracyPoints += 3;
            Debug.Log($"✅ Triage: CORRECT! +3 points. Total accuracy: {_accuracyPoints}/{_maxAccuracyPoints}");
        }
        else
        {
            Debug.Log($"❌ Triage: INCORRECT. +0 points. Total accuracy: {_accuracyPoints}/{_maxAccuracyPoints}");
        }
    }

    public void RecordRPMAssessment(int correctAnswers)
    {
        _rpmCorrectAnswers = Mathf.Clamp(correctAnswers, 0, MAX_RPM_POINTS);
        _accuracyPoints += _rpmCorrectAnswers;
        Debug.Log($"📋 RPM: {_rpmCorrectAnswers}/{MAX_RPM_POINTS} correct. +{_rpmCorrectAnswers} points. Total accuracy: {_accuracyPoints}/{_maxAccuracyPoints}");
    }

    public void RecordPriorityOrder(bool correctOrder)
    {
        _priorityCorrect = correctOrder;
        if (correctOrder)
        {
            _accuracyPoints += 1;
            Debug.Log($"📊 Priority Order: CORRECT! +1 point. Total accuracy: {_accuracyPoints}/{_maxAccuracyPoints}");
        }
        else
        {
            Debug.Log($"📊 Priority Order: INCORRECT. +0 points. Total accuracy: {_accuracyPoints}/{_maxAccuracyPoints}");
        }
    }

    public void RecordEHRAction(bool isCorrect)
    {
        _ehrCorrect = isCorrect;
        if (isCorrect)
        {
            _accuracyPoints += 1;
            Debug.Log($"💊 EHR Action: CORRECT! +1 point. Total accuracy: {_accuracyPoints}/{_maxAccuracyPoints}");
        }
        else
        {
            Debug.Log($"💊 EHR Action: INCORRECT. +0 points. Total accuracy: {_accuracyPoints}/{_maxAccuracyPoints}");
        }
    }

    public void SetConfidenceScore(int score)
    {
        _confidenceScore = Mathf.Clamp(score, 1, 5);
        Debug.Log($"🎯 Confidence Score set to: {_confidenceScore}/5 ({(_confidenceScore / 5f) * 100f}%)");
    }

    public void ApplyTimerPenalty()
    {
        _timeTaken = float.MaxValue;
        Debug.Log("⏰ Timer expired penalty applied. Speed score will be 0.");
    }

    public void CalculateFinalScore()
    {
        // Calculate accuracy percentage
        float accuracyPercent = (_accuracyPoints / (float)_maxAccuracyPoints) * 100f;

        // Calculate speed points (0-5 scale)
        int speedPoints;
        if (_timeTaken >= float.MaxValue)
            speedPoints = 0;
        else if (_timeTaken <= 60f)
            speedPoints = 5;
        else if (_timeTaken <= 120f)
            speedPoints = 3;
        else
            speedPoints = 1;
        float speedPercent = (speedPoints / 5f) * 100f;

        // Calculate confidence percentage (1-5 scale to 0-100)
        float confidencePercent = (_confidenceScore / 5f) * 100f;

        // Final weighted score (per thesis formula)
        float finalScore = (accuracyPercent * 0.6f) + (speedPercent * 0.3f) + (confidencePercent * 0.1f);

        // Interpretation
        string interpretation;
        string performanceLevel;

        if (finalScore >= 90)
        {
            interpretation = "Excellent Clinical Readiness";
            performanceLevel = "Excellent";
        }
        else if (finalScore >= 80)
        {
            interpretation = "Very Good";
            performanceLevel = "Very Good";
        }
        else if (finalScore >= 70)
        {
            interpretation = "Satisfactory";
            performanceLevel = "Satisfactory";
        }
        else
        {
            interpretation = "Needs Improvement";
            performanceLevel = "Needs Improvement";
        }

        LastScenario = _activeScenario;

        LastResult = new ScoringResult
        {
            finalScore = Mathf.RoundToInt(finalScore),
            accuracyScore = Mathf.RoundToInt(accuracyPercent),
            speedScore = Mathf.RoundToInt(speedPercent),
            confidenceScore = Mathf.RoundToInt(confidencePercent),
            timeTaken = _timeTaken,
            selectedCategory = _selected,
            correctCategory = _correct,
            ehrCorrect = _ehrCorrect,
            interpretation = interpretation,
            isCorrect = _triageWasCorrect,
            rpmCorrectCount = _rpmCorrectAnswers,
            accuracyPoints = _accuracyPoints,
            maxAccuracyPoints = _maxAccuracyPoints,
            speedPoints = speedPoints,
            confidenceValue = _confidenceScore,
            performanceLevel = performanceLevel
        };

        // Detailed debug output
        Debug.Log("═══════════════════════════════════════════════════════════════");
        Debug.Log("                    FINAL SCORE BREAKDOWN                       ");
        Debug.Log("═══════════════════════════════════════════════════════════════");
        Debug.Log($"📊 ACCURACY:  {_accuracyPoints}/{_maxAccuracyPoints} points ({accuracyPercent:F1}%) × 60% = {(accuracyPercent * 0.6f):F1}");
        Debug.Log($"  ├─ Triage:    {(_triageWasCorrect ? "✓ +3" : "✗ +0")}");
        Debug.Log($"  ├─ RPM Quiz:  {_rpmCorrectAnswers}/3 correct (+{_rpmCorrectAnswers})");
        Debug.Log($"  ├─ Priority:  {(_priorityCorrect ? "✓ +1" : "✗ +0")}");
        Debug.Log($"  └─ EHR:       {(_ehrCorrect ? "✓ +1" : "✗ +0")}");
        Debug.Log($"");
        Debug.Log($"⚡ SPEED:     {speedPoints}/5 points ({speedPercent:F1}%) × 30% = {(speedPercent * 0.3f):F1}");
        Debug.Log($"  └─ Time taken: {(_timeTaken >= float.MaxValue ? "EXPIRED" : $"{_timeTaken:F0} seconds")}");
        Debug.Log($"");
        Debug.Log($"🎯 CONFIDENCE: {_confidenceScore}/5 ({confidencePercent:F1}%) × 10% = {(confidencePercent * 0.1f):F1}");
        Debug.Log($"───────────────────────────────────────────────────────────────");
        Debug.Log($"🏆 FINAL SCORE: {finalScore:F1}% — {interpretation}");
        Debug.Log("═══════════════════════════════════════════════════════════════");
    }

    public void ResetScoring()
    {
        _accuracyPoints = 0;
        _timeTaken = 0f;
        _ehrCorrect = false;
        _confidenceScore = 5;  // Reset to 5
        _rpmCorrectAnswers = 0;
        _priorityCorrect = false;
        _triageWasCorrect = false;
        Debug.Log("ScoringSystem: Reset");
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
    public int rpmCorrectCount;
    public int accuracyPoints;
    public int maxAccuracyPoints;
    public int speedPoints;
    public int confidenceValue;
    public string performanceLevel;
}