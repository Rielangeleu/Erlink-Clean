using UnityEngine;
using System.Collections.Generic;

public class ScoringSystem : MonoBehaviour
{
    public static ScoringResult LastResult;
    public static ScenarioData LastScenario;

    // Scoring components
    private int _accuracyPoints = 0;
    private float _timeTaken = 0f;
    private bool _ehrCorrect = false;
    private float _dynamicConfidence = 50f;
    private TriageCategory _selected;
    private TriageCategory _correct;
    private ScenarioData _activeScenario;

    // RPM scoring
    private int _rpmCorrectAnswers = 0;
    private const int MAX_RPM_POINTS = 3;

    // Priority order
    private bool _priorityCorrect = false;
    private bool _triageWasCorrect = false;

    // Accuracy tracking
    private int _maxAccuracyPoints = 5;
    private int _currentPatientAccuracy = 0;
    private List<int> _patientAccuracyScores = new List<int>();
    
    // Speed tracking
    private float _speedPoints = 0;
    private float _speedPercent = 0;
    private float _timeLimitSeconds = 0f;  // NEW: Store time limit for current scenario

    public void SetActiveScenario(ScenarioData scenario)
    {
        _activeScenario = scenario;
        _accuracyPoints = 0;
        _rpmCorrectAnswers = 0;
        _priorityCorrect = false;
        _ehrCorrect = false;
        _triageWasCorrect = false;
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
    }

    public void RecordTriageDecision(TriageCategory selected, TriageCategory correct, float timeTaken)
    {
        _selected = selected;
        _correct = correct;
        _timeTaken = timeTaken;
        _triageWasCorrect = (selected == correct);

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
        }
    }

    public void RecordRPMAssessment(int correctAnswers)
    {
        _rpmCorrectAnswers = Mathf.Clamp(correctAnswers, 0, MAX_RPM_POINTS);
        Debug.Log($"📋 RPM: {_rpmCorrectAnswers}/{MAX_RPM_POINTS} correct answers recorded. (No points - mandatory only)");
    }

    public void RecordPriorityOrder(bool correctOrder)
    {
        _priorityCorrect = correctOrder;
        if (correctOrder)
        {
            _currentPatientAccuracy += 1;
            _accuracyPoints += 1;
            Debug.Log($"📊 Priority Order: CORRECT! +1 point. Patient accuracy now: {_currentPatientAccuracy}/5");
        }
        else
        {
            Debug.Log($"📊 Priority Order: INCORRECT. +0 points. Patient accuracy: {_currentPatientAccuracy}/5");
        }
    }

    public void RecordEHRAction(bool isCorrect)
    {
        _ehrCorrect = isCorrect;
        if (isCorrect)
        {
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
    }

    public void ApplyTimerPenalty()
    {
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
    }

    public void CalculateFinalScore()
    {
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
            speedScore = Mathf.RoundToInt(_speedPercent),
            confidenceScore = Mathf.RoundToInt(_dynamicConfidence),
            timeTaken = _timeTaken,
            selectedCategory = _selected,
            correctCategory = _correct,
            ehrCorrect = _ehrCorrect,
            interpretation = interpretation,
            isCorrect = _triageWasCorrect,
            rpmCorrectCount = _rpmCorrectAnswers,
            accuracyPoints = _accuracyPoints,
            maxAccuracyPoints = maxTotalAccuracy,
            speedPoints = Mathf.RoundToInt(_speedPoints),
            confidenceValue = Mathf.RoundToInt(_dynamicConfidence),
            performanceLevel = performanceLevel,
            patientCount = totalPatients,
            patientAccuracyScores = new List<int>(_patientAccuracyScores)
        };
        
        // Detailed debug output
        Debug.Log("═══════════════════════════════════════════════════════════════");
        Debug.Log("                    FINAL SCORE BREAKDOWN                       ");
        Debug.Log("═══════════════════════════════════════════════════════════════");
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
        Debug.Log($"───────────────────────────────────────────────────────────────");
        Debug.Log($"🏆 FINAL SCORE: {finalScore:F1}% — {interpretation}");
        Debug.Log("═══════════════════════════════════════════════════════════════");
    }
    
    public void ResetScoring()
    {
        _accuracyPoints = 0;
        _timeTaken = 0f;
        _ehrCorrect = false;
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
    public int patientCount;
    public List<int> patientAccuracyScores;
}