using UnityEngine;
<<<<<<< HEAD
using System.Collections.Generic;
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

public class ScoringSystem : MonoBehaviour
{
    public static ScoringResult LastResult;
    public static ScenarioData LastScenario;

    // Scoring components
    private int _accuracyPoints = 0;
    private float _timeTaken = 0f;
    private bool _ehrCorrect = false;
<<<<<<< HEAD
    private float _dynamicConfidence = 50f;
=======
    private int _confidenceScore = 5;  // Default to 5 (100%) instead of 3
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    private TriageCategory _selected;
    private TriageCategory _correct;
    private ScenarioData _activeScenario;

    // RPM scoring
    private int _rpmCorrectAnswers = 0;
    private const int MAX_RPM_POINTS = 3;

    // Priority order
    private bool _priorityCorrect = false;
<<<<<<< HEAD
    private bool _triageWasCorrect = false;

    // Accuracy tracking
    private int _maxAccuracyPoints = 5;
    private int _currentPatientAccuracy = 0;
    private List<int> _patientAccuracyScores = new List<int>();
    
    // Speed tracking
    private float _speedPoints = 0;
    private float _speedPercent = 0;
    private float _timeLimitSeconds = 0f;  // NEW: Store time limit for current scenario
=======

    // Track if triage was correct
    private bool _triageWasCorrect = false;

    // Track max accuracy points based on scenario type
    private int _maxAccuracyPoints = 8;  // Default: Triage(3) + RPM(3) + Priority(1) + EHR(1)
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

    public void SetActiveScenario(ScenarioData scenario)
    {
        _activeScenario = scenario;
        _accuracyPoints = 0;
        _rpmCorrectAnswers = 0;
        _priorityCorrect = false;
        _ehrCorrect = false;
        _triageWasCorrect = false;
<<<<<<< HEAD
        _currentPatientAccuracy = 0;
        _patientAccuracyScores.Clear();
        _maxAccuracyPoints = 5;
        
        // NEW: Store time limit from scenario
        _timeLimitSeconds = scenario.timeLimitSeconds;
        
        LoadConfidence();
        Debug.Log($"ScoringSystem: Active scenario set to {scenario?.scenarioTitle}, Time Limit: {_timeLimitSeconds}s");
    }

    public void SetActiveRuntimeScenario(RuntimeScenarioData scenario)
    {
        _accuracyPoints = 0;
        _rpmCorrectAnswers = 0;
        _priorityCorrect = false;
        _ehrCorrect = false;
        _triageWasCorrect = false;
        _currentPatientAccuracy = 0;
        _patientAccuracyScores.Clear();
        _maxAccuracyPoints = 5;
        
        // NEW: Store time limit from runtime scenario
        _timeLimitSeconds = scenario.timeLimitSeconds;
        
        LoadConfidence();
        Debug.Log($"ScoringSystem: Active runtime scenario set to {scenario?.scenarioTitle}, Time Limit: {_timeLimitSeconds}s");
    }

    private void LoadConfidence()
    {
        _dynamicConfidence = PlayerPrefs.GetFloat("DynamicConfidence", 50f);
        Debug.Log($"Loaded previous confidence: {_dynamicConfidence}%");
    }

    private void SaveConfidence()
    {
        PlayerPrefs.SetFloat("DynamicConfidence", _dynamicConfidence);
        PlayerPrefs.Save();
        Debug.Log($"Saved confidence: {_dynamicConfidence}%");
=======
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
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    public void RecordTriageDecision(TriageCategory selected, TriageCategory correct, float timeTaken)
    {
        _selected = selected;
        _correct = correct;
        _timeTaken = timeTaken;
        _triageWasCorrect = (selected == correct);

<<<<<<< HEAD
        if (_triageWasCorrect)
        {
            _currentPatientAccuracy = 3;
            _accuracyPoints += 3;
            Debug.Log($"✅ Triage: CORRECT! +3 points. Patient accuracy: {_currentPatientAccuracy}/5");
        }
        else
        {
            _currentPatientAccuracy = 0;
            Debug.Log($"❌ Triage: INCORRECT. +0 points. Patient accuracy: {_currentPatientAccuracy}/5");
=======
        // Triage accuracy: 3 points for correct, 0 for incorrect
        if (_triageWasCorrect)
        {
            _accuracyPoints += 3;
            Debug.Log($"✅ Triage: CORRECT! +3 points. Total accuracy: {_accuracyPoints}/{_maxAccuracyPoints}");
        }
        else
        {
            Debug.Log($"❌ Triage: INCORRECT. +0 points. Total accuracy: {_accuracyPoints}/{_maxAccuracyPoints}");
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        }
    }

    public void RecordRPMAssessment(int correctAnswers)
    {
        _rpmCorrectAnswers = Mathf.Clamp(correctAnswers, 0, MAX_RPM_POINTS);
<<<<<<< HEAD
        Debug.Log($"📋 RPM: {_rpmCorrectAnswers}/{MAX_RPM_POINTS} correct answers recorded. (No points - mandatory only)");
=======
        _accuracyPoints += _rpmCorrectAnswers;
        Debug.Log($"📋 RPM: {_rpmCorrectAnswers}/{MAX_RPM_POINTS} correct. +{_rpmCorrectAnswers} points. Total accuracy: {_accuracyPoints}/{_maxAccuracyPoints}");
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    public void RecordPriorityOrder(bool correctOrder)
    {
        _priorityCorrect = correctOrder;
        if (correctOrder)
        {
<<<<<<< HEAD
            _currentPatientAccuracy += 1;
            _accuracyPoints += 1;
            Debug.Log($"📊 Priority Order: CORRECT! +1 point. Patient accuracy now: {_currentPatientAccuracy}/5");
        }
        else
        {
            Debug.Log($"📊 Priority Order: INCORRECT. +0 points. Patient accuracy: {_currentPatientAccuracy}/5");
=======
            _accuracyPoints += 1;
            Debug.Log($"📊 Priority Order: CORRECT! +1 point. Total accuracy: {_accuracyPoints}/{_maxAccuracyPoints}");
        }
        else
        {
            Debug.Log($"📊 Priority Order: INCORRECT. +0 points. Total accuracy: {_accuracyPoints}/{_maxAccuracyPoints}");
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        }
    }

    public void RecordEHRAction(bool isCorrect)
    {
        _ehrCorrect = isCorrect;
        if (isCorrect)
        {
<<<<<<< HEAD
            _currentPatientAccuracy += 1;
            _accuracyPoints += 1;
            Debug.Log($"💊 EHR Action: CORRECT! +1 point. Patient accuracy now: {_currentPatientAccuracy}/5");
        }
        else
        {
            Debug.Log($"💊 EHR Action: INCORRECT. +0 points. Patient accuracy: {_currentPatientAccuracy}/5");
        }
        
        _patientAccuracyScores.Add(_currentPatientAccuracy);
        _currentPatientAccuracy = 0;
        Debug.Log($"Patient completed. Total accuracy points so far: {_accuracyPoints}");
=======
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
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    public void ApplyTimerPenalty()
    {
<<<<<<< HEAD
        _speedPoints = 0;
        _speedPercent = 0;
        Debug.Log("⏰ Timer expired penalty applied. Speed score = 0.");
    }

    // NEW: Calculate speed points based on percentage of time limit
    private void CalculateSpeedPoints(float timeSeconds, float timeLimit)
    {
        if (timeSeconds >= timeLimit)
        {
            // Timer expired
            _speedPoints = 0;
            _speedPercent = 0;
            Debug.Log($"Speed: Timer expired at {timeSeconds:F0}s (limit: {timeLimit}s) → 0 points");
            return;
        }
        
        // Calculate percentage of time used
        float percentageUsed = (timeSeconds / timeLimit) * 100f;
        
        // Award points based on percentage of time limit
        if (percentageUsed <= 33f)      // Fast: ≤33% of time limit
        {
            _speedPoints = 5;
            _speedPercent = 100f;
            Debug.Log($"Speed: {timeSeconds:F0}s / {timeLimit}s = {percentageUsed:F1}% (≤33%) → 5 points (100%)");
        }
        else if (percentageUsed <= 66f) // Medium: 34-66% of time limit
        {
            _speedPoints = 3;
            _speedPercent = 60f;
            Debug.Log($"Speed: {timeSeconds:F0}s / {timeLimit}s = {percentageUsed:F1}% (34-66%) → 3 points (60%)");
        }
        else                            // Slow: 67-100% of time limit
        {
            _speedPoints = 1;
            _speedPercent = 20f;
            Debug.Log($"Speed: {timeSeconds:F0}s / {timeLimit}s = {percentageUsed:F1}% (67-100%) → 1 point (20%)");
        }
=======
        _timeTaken = float.MaxValue;
        Debug.Log("⏰ Timer expired penalty applied. Speed score will be 0.");
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    public void CalculateFinalScore()
    {
<<<<<<< HEAD
        int totalPatients = _patientAccuracyScores.Count;
        if (totalPatients == 0) totalPatients = 1;
        
        // Calculate Accuracy Percentage
        int maxTotalAccuracy = _maxAccuracyPoints * totalPatients;
        float accuracyPercent = (_accuracyPoints / (float)maxTotalAccuracy) * 100f;
        
        // NEW: Calculate speed points using difficulty-based thresholds
        float timeSeconds = _timeTaken;
        if (_timeLimitSeconds > 0f)
        {
            CalculateSpeedPoints(timeSeconds, _timeLimitSeconds);
        }
        else
        {
            // Fallback to old method if time limit not set
            if (timeSeconds <= 60f)
                _speedPoints = 5;
            else if (timeSeconds <= 120f)
                _speedPoints = 3;
            else if (timeSeconds < float.MaxValue)
                _speedPoints = 1;
            else
                _speedPoints = 0;
            _speedPercent = (_speedPoints / 5f) * 100f;
        }
        
        // Calculate Current Performance Score
        float currentPerformanceScore = (accuracyPercent + _speedPercent) / 2f;
        
        // Calculate Dynamic Confidence
        float oldConfidence = _dynamicConfidence;
        _dynamicConfidence = (oldConfidence * 0.7f) + (currentPerformanceScore * 0.3f);
        
        Debug.Log($"=== Confidence Calculation ===");
        Debug.Log($"Previous Confidence: {oldConfidence:F1}%");
        Debug.Log($"Current Performance Score: {currentPerformanceScore:F1}%");
        Debug.Log($"New Confidence: {_dynamicConfidence:F1}%");
        
        SaveConfidence();
        
        // Final weighted score
        float finalScore = (accuracyPercent * 0.6f) + (_speedPercent * 0.3f) + (_dynamicConfidence * 0.1f);
        
        // Interpretation
        string interpretation;
        string performanceLevel;
        
=======
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

>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
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
<<<<<<< HEAD
        
        LastScenario = _activeScenario;
        
=======

        LastScenario = _activeScenario;

>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        LastResult = new ScoringResult
        {
            finalScore = Mathf.RoundToInt(finalScore),
            accuracyScore = Mathf.RoundToInt(accuracyPercent),
<<<<<<< HEAD
            speedScore = Mathf.RoundToInt(_speedPercent),
            confidenceScore = Mathf.RoundToInt(_dynamicConfidence),
=======
            speedScore = Mathf.RoundToInt(speedPercent),
            confidenceScore = Mathf.RoundToInt(confidencePercent),
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
            timeTaken = _timeTaken,
            selectedCategory = _selected,
            correctCategory = _correct,
            ehrCorrect = _ehrCorrect,
            interpretation = interpretation,
            isCorrect = _triageWasCorrect,
            rpmCorrectCount = _rpmCorrectAnswers,
            accuracyPoints = _accuracyPoints,
<<<<<<< HEAD
            maxAccuracyPoints = maxTotalAccuracy,
            speedPoints = Mathf.RoundToInt(_speedPoints),
            confidenceValue = Mathf.RoundToInt(_dynamicConfidence),
            performanceLevel = performanceLevel,
            patientCount = totalPatients,
            patientAccuracyScores = new List<int>(_patientAccuracyScores)
        };
        
=======
            maxAccuracyPoints = _maxAccuracyPoints,
            speedPoints = speedPoints,
            confidenceValue = _confidenceScore,
            performanceLevel = performanceLevel
        };

>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        // Detailed debug output
        Debug.Log("═══════════════════════════════════════════════════════════════");
        Debug.Log("                    FINAL SCORE BREAKDOWN                       ");
        Debug.Log("═══════════════════════════════════════════════════════════════");
<<<<<<< HEAD
        Debug.Log($"📊 ACCURACY:  {_accuracyPoints}/{maxTotalAccuracy} points ({accuracyPercent:F1}%) × 60% = {(accuracyPercent * 0.6f):F1}");
        Debug.Log($"  Per Patient Breakdown:");
        for (int i = 0; i < _patientAccuracyScores.Count; i++)
        {
            Debug.Log($"    Patient {i + 1}: {_patientAccuracyScores[i]}/5 points");
        }
        Debug.Log($"");
        Debug.Log($"⏱️ TIME LIMIT: {_timeLimitSeconds}s for this difficulty");
        Debug.Log($"⚡ SPEED:     {_timeTaken:F0}s / {_timeLimitSeconds}s = {(_timeTaken/_timeLimitSeconds)*100:F1}% of limit → {_speedPoints}/5 points ({_speedPercent:F0}%) × 30% = {(_speedPercent * 0.3f):F1}");
        Debug.Log($"");
        Debug.Log($"🎯 CONFIDENCE: {_dynamicConfidence:F1}% (Dynamic) × 10% = {(_dynamicConfidence * 0.1f):F1}");
        Debug.Log($"  └─ Formula: (Previous {oldConfidence:F1}% × 0.7) + (Performance {currentPerformanceScore:F1}% × 0.3)");
=======
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
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        Debug.Log($"───────────────────────────────────────────────────────────────");
        Debug.Log($"🏆 FINAL SCORE: {finalScore:F1}% — {interpretation}");
        Debug.Log("═══════════════════════════════════════════════════════════════");
    }
<<<<<<< HEAD
    
=======

>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    public void ResetScoring()
    {
        _accuracyPoints = 0;
        _timeTaken = 0f;
        _ehrCorrect = false;
<<<<<<< HEAD
        _rpmCorrectAnswers = 0;
        _priorityCorrect = false;
        _triageWasCorrect = false;
        _currentPatientAccuracy = 0;
        _patientAccuracyScores.Clear();
        Debug.Log("ScoringSystem: Reset (confidence preserved)");
    }
    
    public void ResetConfidence()
    {
        _dynamicConfidence = 50f;
        SaveConfidence();
        Debug.Log("Confidence reset to 50%");
=======
        _confidenceScore = 5;  // Reset to 5
        _rpmCorrectAnswers = 0;
        _priorityCorrect = false;
        _triageWasCorrect = false;
        Debug.Log("ScoringSystem: Reset");
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
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
<<<<<<< HEAD
    public int patientCount;
    public List<int> patientAccuracyScores;
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
}