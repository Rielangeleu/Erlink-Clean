using UnityEngine;

public enum TriageCategory
{
    Immediate,  // Red
    Delayed,    // Yellow
    Minor,      // Green
    Expectant   // Black
}

public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}

public enum EmergencyCode
{
    CodeBlue,
    CodeRed,
    CodeOrange,
    CodeSilver,
    None
}

[System.Serializable]
public class RPMAssessment
{
    [Header("Respiration")]
    public string respirationQuestion;
    public string[] respirationOptions;
    public int correctRespirationIndex;
    public string respirationFeedback;

    [Header("Perfusion")]
    public string perfusionQuestion;
    public string[] perfusionOptions;
    public int correctPerfusionIndex;
    public string perfusionFeedback;

    [Header("Mental Status")]
    public string mentalStatusQuestion;
    public string[] mentalStatusOptions;
    public int correctMentalStatusIndex;
    public string mentalStatusFeedback;
}

[System.Serializable]
public class EHRAction
{
    public string actionName;
    public string actionDescription;
    public bool isCorrectAction;
}

[CreateAssetMenu(
    fileName = "NewScenario",
    menuName = "ERLink AR/Scenario Data")]
public class ScenarioData : ScriptableObject
{
    [Header("Scenario Identity")]
    public string scenarioID;
    public string scenarioTitle;
    public string scenarioDescription;
    public DifficultyLevel difficulty;
    public EmergencyCode emergencyCode;

<<<<<<< HEAD
    [Header("Order Index (for sorting)")]
    public int orderIndex = 999;  // ADD THIS FIELD

=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    [Header("Time Limit (seconds)")]
    public float timeLimitSeconds = 180f;

    [Header("Patient Info Card")]
    public string patientAge = "~40s";
    public string patientGender = "Male";
    public string patientPresentation;
    public string[] vitalSigns;
    public string difficultyLabel = "EASY";

    [Header("Patient Prefab")]
    public GameObject patientPrefab;
    public Sprite patientImage;

    [Header("RPM Assessment")]
    public RPMAssessment rpmAssessment;

    [Header("Correct Triage Answer")]
    public TriageCategory correctTriageCategory;

    [Header("EHR Actions")]
    public EHRAction[] ehrActions;

    [Header("Scoring")]
    public int maxAccuracyPoints = 5;

    [Header("Feedback")]
    public string correctFeedback;
    public string incorrectFeedback;
    public string clinicalExplanation;

    [Header("Multi-Patient (Medium/Hard)")]
    public bool isMultiPatient = false;
    public ScenarioData[] additionalPatients;
    public int correctPriorityOrder;
<<<<<<< HEAD

    [Header("Priority Order (for Multi-Patient)")]
    public string priorityQuestion = "Which patient requires immediate attention first?";
    public string[] priorityOptions;
    public int correctPriorityIndex = 0;
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
}