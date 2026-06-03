using UnityEngine;
using System;

/// <summary>
/// Runtime version of ScenarioData - created dynamically from Firestore
/// </summary>
[System.Serializable]
public class RuntimeScenarioData
{
    public string scenarioID;
    public string scenarioTitle;
    public string scenarioDescription;
    public DifficultyLevel difficulty;
    public EmergencyCode emergencyCode;
    public float timeLimitSeconds = 180f;

    // Patient info (for single patient scenarios)
    public string patientAge = "~40s";
    public string patientGender = "Male";
    public string patientPresentation;
    public string[] vitalSigns;
    public string difficultyLabel = "EASY";

    // KEY: Store the prefab name from Firestore
    public string patientModelName = "Patient_Standard";
    
    // Keep for backward compatibility
    public string patientModelUrl;

    // RPM Assessment
    public RPMAssessment rpmAssessment;

    // Correct triage
    public TriageCategory correctTriageCategory;

    // EHR Actions
    public EHRAction[] ehrActions;

    // Feedback
    public string clinicalExplanation;

    // Multi-patient support
    public bool isMultiPatient = false;
    public RuntimeScenarioData[] additionalPatients;
    public int correctPriorityOrder;

    // ─── ADD THIS: Patients array for multi-patient scenarios ───
    public RuntimePatientData[] patients;

    // Priority Order (for Multi-Patient)
    public string priorityQuestion = "Which patient requires immediate attention first?";
    public string[] priorityOptions;
    public int correctPriorityIndex = 0;

    // Order index for sorting
    public int orderIndex = 999;

    // Loaded prefab (set at runtime)
    [NonSerialized] public GameObject patientPrefab;
}

/// <summary>
/// Runtime patient data for multi-patient scenarios
/// </summary>
[System.Serializable]
public class RuntimePatientData
{
    public string patientAge = "~40s";
    public string patientGender = "Male";
    public string patientPresentation;
    public string[] vitalSigns;
    public string patientModelName = "Patient_Standard";
    public TriageCategory correctTriageCategory;
    public RPMAssessment rpmAssessment;
    public EHRAction[] ehrActions;
    public string clinicalExplanation;
    
    [NonSerialized] public GameObject patientPrefab;
}

/// <summary>
/// Extension methods to convert between ScriptableObject and Runtime
/// </summary>
public static class ScenarioDataConverter
{
    public static RuntimeScenarioData ToRuntime(this ScenarioData scriptable)
    {
        if (scriptable == null) return null;

        var runtime = new RuntimeScenarioData
        {
            scenarioID = scriptable.scenarioID,
            scenarioTitle = scriptable.scenarioTitle,
            scenarioDescription = scriptable.scenarioDescription,
            difficulty = scriptable.difficulty,
            emergencyCode = scriptable.emergencyCode,
            timeLimitSeconds = scriptable.timeLimitSeconds,
            patientAge = scriptable.patientAge,
            patientGender = scriptable.patientGender,
            patientPresentation = scriptable.patientPresentation,
            vitalSigns = scriptable.vitalSigns,
            difficultyLabel = scriptable.difficultyLabel,
            rpmAssessment = scriptable.rpmAssessment,
            correctTriageCategory = scriptable.correctTriageCategory,
            ehrActions = scriptable.ehrActions,
            clinicalExplanation = scriptable.clinicalExplanation,
            isMultiPatient = scriptable.isMultiPatient,
            correctPriorityOrder = scriptable.correctPriorityOrder,
            patientPrefab = scriptable.patientPrefab,
            orderIndex = scriptable.orderIndex,
            priorityQuestion = scriptable.priorityQuestion,
            priorityOptions = scriptable.priorityOptions,
            correctPriorityIndex = scriptable.correctPriorityIndex
        };

        // Convert additional patients if present
        if (scriptable.additionalPatients != null && scriptable.additionalPatients.Length > 0)
        {
            runtime.additionalPatients = new RuntimeScenarioData[scriptable.additionalPatients.Length];
            for (int i = 0; i < scriptable.additionalPatients.Length; i++)
            {
                if (scriptable.additionalPatients[i] != null)
                    runtime.additionalPatients[i] = scriptable.additionalPatients[i].ToRuntime();
            }
        }

        return runtime;
    }
    
    // Helper method to convert RuntimePatientData array from Firestore
    public static RuntimePatientData[] ConvertPatientsFromFirestore(object patientsList)
    {
        // This will be populated from FirestoreScenarioLoader
        return new RuntimePatientData[0];
    }
}